using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
namespace BankaMVC.Controllers
{
    public class GirisController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GirisController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (roleClaim != null)
                {
                    var userRole = roleClaim.Value;

                    if (userRole.Equals("Yönetici", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "AdminPanel");
                    }
                    else if (userRole.Equals("Müşteri", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "GostergePaneli");
                    }
                }
                return RedirectToAction("Index", "GostergePaneli");
            }

            return View(); 
        }






   
        public class ApiErrorResponse
        {
            public Dictionary<string, string[]> Errors { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                 
                    var (isSuccess, userRole, errorMessage) = await KullaniciDogrulaAsync(model.Telefon, model.Sifre);

                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Giriş başarılı! Yönlendiriliyorsunuz...";

                        if (!string.IsNullOrEmpty(userRole))
                        {
                            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Telefon),
            new Claim(ClaimTypes.Role, userRole) // "Yönetici" veya "Müşteri"
        };

                            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                            var principal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync("MyCookieAuth", principal);

                            if (userRole.Equals("Yönetici", StringComparison.OrdinalIgnoreCase))
                            {
                                return RedirectToAction("Index", "adminpanel");
                            }
                            else if (userRole.Equals("Müşteri", StringComparison.OrdinalIgnoreCase))
                            {
                                return RedirectToAction("Index", "GostergePaneli");
                            }
                        }

                        return RedirectToAction("Index", "GostergePaneli");
                    }

                    else
                    {
                        ModelState.AddModelError("", errorMessage?.Trim('"') ?? "Telefon numarası veya şifre hatalı.");
                    }
                }
                catch (Exception ex)
                {
        
                    ModelState.AddModelError("", "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyiniz.");
                }
            }

            return View("Index", model);
        }

        private async Task<(bool isSuccess, string userRole, string errorMessage)> KullaniciDogrulaAsync(string telefon, string sifre)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                var apiUrl = StaticSettings.ApiBaseUrl + "Auth/giris";
                string ipAdres = HttpContext.Connection.RemoteIpAddress?.ToString();
                var payload = new { telefon = "0" + telefon, sifre = sifre, ipAdres=ipAdres };  
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResult = JsonConvert.DeserializeObject<TokenResultViewModel>(responseContent);


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(tokenResult.Token);

                    var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role || claim.Type == "role");
                    string userRole = roleClaim?.Value;

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = tokenResult.Expiration
                    };

                    _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", tokenResult.Token, cookieOptions);

             
                    return (true, userRole, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ApiErrorResponse>(errorContent);
                        if (errorObj?.Errors != null && errorObj.Errors.ContainsKey("Telefon"))
                        {
                            return (false, null, string.Join(", ", errorObj.Errors["Telefon"]));
                        }
                    }
                    catch { }

           
                    return (false, null, errorContent);
                }
            }
        }

    }
}



   
