using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    public class GetTokenCommandHandler : IRequestHandler<GetTokenCommand, string>
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<GetTokenCommandHandler> logger;
        private const int defaultTokenExpirationTimeInMins = 120;

        public GetTokenCommandHandler(IConfiguration _configuration,
                                      ILogger<GetTokenCommandHandler> _logger)
        {
            this.configuration = _configuration;
            this.logger = _logger;
        }

        /// <summary>
        /// Command for generating a bearer token for authorization during testing.
        /// This is not meant to replace proper Oauth for authentication purposes.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> Handle(GetTokenCommand request, CancellationToken cancellationToken)
        {
            // Create token key
            var IssuerSigningKey = this.configuration["IssuerSigningKey"];
            var TokenIssuer = this.configuration["TokenIssuer"];
            var TokenAudience = this.configuration["TokenAudience"];
            var TokenLifetimeInMins = this.configuration["TokenLifetimeInMins"];

            logger.LogInformation($"Key Length: {IssuerSigningKey.Length }");
            SymmetricSecurityKey securityKey =
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey));

            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Set token expiration
            DateTime startTime = DateTime.UtcNow;
            DateTime expiryTime;
            if (double.TryParse(TokenLifetimeInMins, out double tokenExpiration))
            {
                expiryTime = startTime.AddMinutes(tokenExpiration);
            }
            else
            {
                expiryTime = startTime.AddMinutes(defaultTokenExpirationTimeInMins);
            }

            // Generate the token
            JwtSecurityToken token =
                 new JwtSecurityToken(
                     TokenIssuer,
                     TokenAudience,
                     null,
                     notBefore: startTime,
                     expires: expiryTime,
                     signingCredentials: credentials);

            string result = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(result);
        }
    }
}