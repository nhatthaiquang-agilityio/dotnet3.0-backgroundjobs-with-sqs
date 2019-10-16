using Amazon.SQS;
using Amazon.SimpleNotificationService;
using dotnet_backgroundjobs.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using dotnet_backgroundjobs.Aws;

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

            services.AddAWSService<IAmazonSQS>();
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddSingleton<SqsMessage>();
            services.AddSingleton<SnsMessage>();

            // Queuer Reader for Background Service
            services.AddHostedService<QueueReaderService>();
            services.AddHostedService<RecurringJobsService>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // hangfire server and dashboard
            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }

    }
}
