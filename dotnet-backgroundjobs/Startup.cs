using Amazon.SQS;
using Amazon.SimpleNotificationService;
using dotnet_backgroundjobs.Aws;
using dotnet_backgroundjobs.Data;
using dotnet_backgroundjobs.Enties;
using dotnet_backgroundjobs.Email;
using dotnet_backgroundjobs.Models;
using dotnet_backgroundjobs.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using System;
using Microsoft.AspNetCore.Identity;

namespace dotnet_backgroundjobs
{
    public class Startup
    {
        public static StackExchange.Redis.ConnectionMultiplexer Redis;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Other codes / configurations are omitted for brevity.
            Redis = StackExchange.Redis.ConnectionMultiplexer.Connect(Configuration["Redis"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHangfire(configuration =>
            {
                configuration.UseRedisStorage(Redis);
            });
            services.AddHangfireServer();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSQS>();
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddSingleton<SqsMessage>();
            services.AddSingleton<SnsMessage>();

            // Queuer Reader for Background Service
            services.AddHostedService<QueueReaderService>();
            services.AddHostedService<RecurringJobsService>();

            string clientId = Configuration.GetValue<string>("AWS_COGNITO_CLIENT_ID");
            string poolId = Configuration.GetValue<string>("AWS_COGNITO_POOL_ID");
            string region = Configuration.GetValue<string>("AWS_DEFAULT_REGION");
            string address = "https://cognito-idp." + region + ".amazonaws.com/" + poolId +
                "/.well-known/openid-configuration";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.RequireHttpsMetadata = false;
                options.ResponseType = Configuration["Authentication:Cognito:ResponseType"];
                options.MetadataAddress = address;
                options.ClientId = clientId;
            });
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //})
            //.AddJwtBearer(options =>
            //{
            //    options.Audience = Configuration["Authentication:Cognito:ClientId"];
            //    options.Authority = Configuration["Authentication:Cognito:Authority"];
            //    options.RequireHttpsMetadata = false;
            //});

            // Add framework services.
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(Configuration["ConnectionString"])
            );
            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<ApplicationUser>);
            var emailTokenProviderType = typeof(EmailTokenProvider<ApplicationUser>);
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();

            InitData(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // hangfire server and dashboard
            app.UseHangfireServer();
            app.UseHangfireDashboard();


        }

        // Seed Data
        private void InitData(IServiceCollection services)
        {
            ServiceProvider sp = services.BuildServiceProvider();
            var context = sp.GetRequiredService<ApplicationDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Migration Database");
                Console.WriteLine(e);
            }

            new ApplicationContextSeed().SeedAsync(context).Wait();
        }
    }
}
