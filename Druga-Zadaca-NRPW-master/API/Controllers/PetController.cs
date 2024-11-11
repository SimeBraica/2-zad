using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase {

        private HttpClient _httpClient;
        private NrppwDrugaContext _context;
        private readonly Jwt _jwt;
        private IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PetController(NrppwDrugaContext context, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpClient = httpClient;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _jwt = new Jwt(_config, _httpContextAccessor, _context);
        }

        //[Authorize]
        [HttpGet]
        [Route("api/pets/authorized")]
        public async Task<ActionResult<List<PetDTO>>> GetPetsAuthorized() {
            //var claims = _jwt.DecodeToken();
           // int id = int.Parse(claims[0]);
            var pets = await _context.Pets
                                        .Where(a => a.OwnerId == 1)
                                        .ToListAsync();

            List<PetDTO> petsDTO = new();
            foreach (var pet in pets) {
                PetDTO petDTO = new PetDTO {
                    PetName = pet.PetName,
                    Animal = pet.Animal
                };
                petsDTO.Add(petDTO);
            }
            return petsDTO;
        }

        [HttpGet]
        public async Task<ActionResult<List<PetDTO>>> GetPets(int id) {
            var pets = await _context.Pets
                                       .Where(a => a.OwnerId == id)
                                       .ToListAsync();

            List<PetDTO> petsDTO = new();
            foreach (var pet in pets) {
                PetDTO petDTO = new PetDTO {
                    PetName = pet.PetName,
                    Animal = pet.Animal
                };
                petsDTO.Add(petDTO);
            }
            return petsDTO;
        }
    }
}
