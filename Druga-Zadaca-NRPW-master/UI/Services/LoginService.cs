using System.Net.Http.Json;
using UI.Services;

namespace UI.Services {
    public class LoginService {
        private HttpClient _httpClient;
        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> LoginUser(string? username, string? password, bool sqlInjection) {
            var res =  await _httpClient.GetFromJsonAsync<int>($"https://localhost:7128/api/Owner/login?username={username}&password={password}&sqlInjection={sqlInjection}");
            return res;
        }
    }
}
