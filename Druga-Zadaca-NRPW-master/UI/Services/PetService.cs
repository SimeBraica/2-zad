using System.Net.Http.Json;
using UI.DTO;

namespace UI.Services {
    public class PetService {

        private HttpClient _httpClient;
        public PetService(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<List<PetDTO>> GetPetsNotAuth(int id) {
            var res = await _httpClient.GetFromJsonAsync<List<PetDTO>>($"https://localhost:7128/api/Pet?id={id}");
            return res;
        }

        public async Task<List<PetDTO>> GetPetsAuth() {
            var auth = await _httpClient.GetFromJsonAsync<List<PetDTO>>($"https://localhost:7128/api/Pet/api/pets/authorized");
            return auth;
        }
    }
}
