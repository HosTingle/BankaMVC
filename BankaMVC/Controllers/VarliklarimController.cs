
using BankaMVC.Filters;
using BankaMVC.Models.Result;
using BankaMVC.Models.Somut;
using BankaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BankaMVC.Controllers
{
    public class VarliklarimController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VarliklarimController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private readonly DovizKuru _dovizKuru = new DovizKuru();
        [RoleAuthorize("Müşteri")]
        public async Task<IActionResult> Index(string paraBirimi, string bolum = "")
        {
            if (string.IsNullOrEmpty(paraBirimi))
            {
                paraBirimi = "TRY"; 
            }
            var sonuc = await VarliklarimiGetirAsync();
            KartBorcHesapla(sonuc);

            var dovizKuru = new DovizKuru(); // buradaki oranlar elle girilmiş ya da başka bir yerden alınıyor olmalı

            var toplamPara = ConvertCurrency(sonuc.ToplamPara, "TRY", paraBirimi, dovizKuru);
            var toplamBorc = ConvertCurrency(sonuc.ToplamBorc, "TRY", paraBirimi, dovizKuru);
            foreach (var kart in sonuc.Kartlar)
            {
                kart.KartBorc = ConvertCurrency(kart.KartBorc, "TRY", paraBirimi, dovizKuru);
            }
            foreach (var hesap in sonuc.Hesaplar) 
            {
                hesap.Bakiye = ConvertCurrency(hesap.Bakiye, "TRY", paraBirimi, dovizKuru);
            }
            var viewModel = new VarliklarimViewModel
            {
                Hesaplar = sonuc.Hesaplar,
                Kartlar = sonuc.Kartlar,
                SecilenParaBirimi = paraBirimi,
                AktifBolum = bolum,
                ToplamVarlik = toplamPara,
                ToplamBorc = toplamBorc,
                NetVarlik = toplamPara - toplamBorc
            };

            return View(viewModel);
        }


        private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, DovizKuru kur)
        {
            if (fromCurrency == toCurrency) return amount;

            return (fromCurrency, toCurrency) switch
            {
                ("TRY", "USD") => amount * kur.TRY_USD,
                ("TRY", "EUR") => amount * kur.TRY_EUR,
                ("USD", "TRY") => amount * kur.USD_TRY,
                ("USD", "EUR") => amount * kur.USD_EUR,
                ("EUR", "TRY") => amount * kur.EUR_TRY,
                ("EUR", "USD") => amount * kur.EUR_USD,
                _ => amount
            };
        }
        private void KartBorcHesapla(VarliklarViewModel sonuc)
        {
            foreach (var kart in sonuc.Kartlar)
            {
                kart.KartBorc = 5000 - kart.Limit;
            }
        }
        private async Task<VarliklarViewModel> VarliklarimiGetirAsync() 
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(token))
                {
                    return new VarliklarViewModel(); // Token yoksa boş döner
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = StaticSettings.ApiBaseUrl + "Hesap/varliklargetir";

                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuccessDataResult<VarliklarViewModel>>(json,new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

                    return result?.Data ?? new VarliklarViewModel();
                }

                return new VarliklarViewModel();
            }
        }
    }
}
