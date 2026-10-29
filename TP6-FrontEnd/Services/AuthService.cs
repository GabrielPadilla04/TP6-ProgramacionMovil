using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TP6_Shared.DTOs.Auth;

namespace TP6_FrontEnd.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string TokenKey = "jwt_token";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(LoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
                if (!response.IsSuccessStatusCode)
                    return false;

                var resultado = await response.Content.ReadFromJsonAsync<LoginResultDto>();
                if (resultado == null || string.IsNullOrWhiteSpace(resultado.Token))
                    return false;

                await SecureStorage.Default.SetAsync(TokenKey, resultado.Token);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Login: {ex.Message}");
                return false;
            }
        }

        public Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            return Task.CompletedTask;
        }

        public async Task<int?> ObtenerUsuarioIdAsync()
        {
            var claims = await ObtenerClaimsDeTokenAsync();
            if (claims == null) return null;

            if (claims.TryGetValue(ClaimTypes.NameIdentifier, out var idObj) ||
                claims.TryGetValue("nameid", out idObj) ||
                claims.TryGetValue("sub", out idObj))
            {
                if (int.TryParse(idObj?.ToString(), out int id))
                    return id;
            }

            return null;
        }
        public async Task<string?> ObtenerTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }

        public async Task<string?> ObtenerRolAsync()
        {
            var claims = await ObtenerClaimsDeTokenAsync();
            if (claims == null) return null;

            if (claims.TryGetValue(ClaimTypes.Role, out var roleObj) ||
                claims.TryGetValue("role", out roleObj))
            {
                return roleObj?.ToString();
            }

            return null;
        }

        public async Task<string?> ObtenerEmailAsync()
        {
            var claims = await ObtenerClaimsDeTokenAsync();
            if (claims == null) return null;

            if (claims.TryGetValue(ClaimTypes.Email, out var emailObj) ||
                claims.TryGetValue("email", out emailObj))
            {
                return emailObj?.ToString();
            }

            return null;
        }

        public async Task<bool> EstaAutenticadoAsync()
        {
            var token = await ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return false;

            var claims = await ObtenerClaimsDeTokenAsync();
            if (claims != null && claims.TryGetValue("exp", out var expObj))
            {
                if (long.TryParse(expObj?.ToString(), out long expSeconds))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                    return expDate > DateTime.UtcNow;
                }
            }

            return true;
        }

        private async Task<Dictionary<string, object>?> ObtenerClaimsDeTokenAsync()
        {
            var token = await ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return null;

            var partes = token.Split('.');
            if (partes.Length < 2) return null;

            var payload = partes[1];
            // Normalizar padding Base64
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);

            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}