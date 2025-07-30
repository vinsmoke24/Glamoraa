using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Tokens;

public class JwtValidationHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization;

        if (authHeader != null && authHeader.Scheme == "Bearer")
        {
            var token = authHeader.Parameter;
            var key = Encoding.ASCII.GetBytes(System.Configuration.ConfigurationManager.AppSettings["JwtSecret"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var claimsIdentity = ((JwtSecurityToken)validatedToken).Claims;
                Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity, "jwt"));
                HttpContext.Current.User = Thread.CurrentPrincipal;
            }
            catch
            {
                return request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
