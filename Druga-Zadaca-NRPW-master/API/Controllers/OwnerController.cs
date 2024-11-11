using API.Models;
using Blazorise.Captcha;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase {

        private HttpClient _httpClient;
        private NrppwDrugaContext _context;
        private IConfiguration _config;
        private Jwt _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OwnerController(NrppwDrugaContext context, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _jwt = new Jwt(_config, _httpContextAccessor, _context);
        }

        [HttpGet]
        public async Task<ActionResult<List<Owner>>> GetOwner(int id) {
            var owners = await _context.Owners
                                       .Where(a => a.OwnerId == id)
                                       .Include(p => p.Pets)
                                       .ToListAsync();
            return Ok(owners);
        }

        [HttpGet("login")]
        public async Task<ActionResult<int>> LoginOwner(string? username, string? password, bool sqlInjection) {
            if(sqlInjection == true) {
                var commandText = $"SELECT * FROM owner WHERE Username = '{username}' AND Password = '{password}'";
                var owner = await _context.Owners
                                          .FromSqlRaw(commandText)
                                          .ToListAsync();
                if (owner.Any()) {
                    var ownerWithId = owner[0].OwnerId;
                    return Ok(ownerWithId);
                }
                return Ok(0);
            }
            var ownerWithoutSqlInjection =  await _context.Owners.FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
            if (ownerWithoutSqlInjection == null) {
                return Ok(0);
            }
            _jwt.GenerateJWT(ownerWithoutSqlInjection);
            return Ok(ownerWithoutSqlInjection.OwnerId);
        }


        [HttpPost("validate-captcha")]
        public async Task<IActionResult> ValidateCaptcha([FromBody] CaptchaState state) {
            bool isValid = await Validate(state);
            return Ok(new { Success = isValid });
        }

        private async Task<bool> Validate(CaptchaState state) {
            var content = new FormUrlEncodedContent(
            [
            new KeyValuePair<string, string>("secret", "6Lc1-HUqAAAAABxcpP4m-JSX-5hrNk8ibwJBMiuL"),
            new KeyValuePair<string, string>("response", state.Response)
            ]);

            var response = await _httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var result = await response.Content.ReadAsStringAsync();
            var googleResponse = JsonSerializer.Deserialize<GoogleResponse>(result, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return googleResponse.Success;
        }
    }
}
