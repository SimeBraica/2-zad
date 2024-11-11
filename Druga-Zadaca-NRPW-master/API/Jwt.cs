using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using API.Models;

namespace API {
    public class Jwt {

        private IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NrppwDrugaContext _context;

        public Jwt(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, NrppwDrugaContext context) {
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public string GenerateJWT(Owner owner) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var audience = _config["Jwt:Audience"];
            var issuer = _config["Jwt:Issuer"];
            TimeZoneInfo croatiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime expiresLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, croatiaTimeZone).AddMinutes(60);

            string id = owner.OwnerId.ToString();

            var jwt_description = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {new Claim("id", id) }),
                Expires = expiresLocalTime,
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var token = new JwtSecurityTokenHandler().CreateToken(jwt_description);
            var encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", encryptedToken,
                new CookieOptions {
                    Expires = expiresLocalTime,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });

            var response = new { token = encryptedToken, id = owner.OwnerId };
            return JsonSerializer.Serialize(response);
        }


        public List<string> DecodeToken() {
            var cookie = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadToken(cookie);
            var jwtS = jwt as JwtSecurityToken;


            var jti1 = jwtS.Claims.First(claim => claim.Type == "id").Value;
            var listOfClaims = new List<string> {
                jti1
            };
            return listOfClaims;
        }
    }
}
