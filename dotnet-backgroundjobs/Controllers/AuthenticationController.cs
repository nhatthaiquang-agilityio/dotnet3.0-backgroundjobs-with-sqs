using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace dotnet_backgroundjobs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly string _clientId;
        private readonly string _poolId;
        private readonly AmazonCognitoIdentityProviderClient _cognito;

        public AuthenticationController(IConfiguration configuration)
        {
            _cognito = new AmazonCognitoIdentityProviderClient(
               configuration.GetValue<string>("AWS_ACCESS_KEY_ID"),
               configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY"),
               configuration.GetValue<string>("AWS_DEFAULT_REGION"));
            _clientId = configuration.GetValue<string>("AWS_COGNITO_CLIENT_ID");
            _poolId = configuration.GetValue<string>("AWS_COGNITO_POOL_ID");
        }

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register(User user)
        {
            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            await _cognito.SignUpAsync(request);

            return Ok();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<string>> SignIn(User user)
        {
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _poolId,
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await _cognito.AdminInitiateAuthAsync(request);

            return Ok(response.AuthenticationResult.IdToken);
        }

    }
}
