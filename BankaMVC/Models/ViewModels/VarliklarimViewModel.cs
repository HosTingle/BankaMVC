

using BankaMVC.Models.DTOs;
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class VarliklarimViewModel
    {
        public List<HesapDto> Hesaplar { get; set; } = new List<HesapDto>();
        public List<KartDto> Kartlar { get; set; } = new List<KartDto>();
        public string SecilenParaBirimi { get; set; } = "TRY";
        public decimal NetVarlik { get; set; }
        public decimal ToplamVarlik { get; set; }
        public decimal ToplamBorc { get; set; }
        public string AktifBolum { get; set; } = ""; // varliklar veya borclar
    }

    public class DovizKuru
    {
        public decimal USD_TRY { get; set; } = 30.50m;
        public decimal EUR_TRY { get; set; } = 33.20m;
        public decimal TRY_USD { get; set; } = 0.0328m;
        public decimal TRY_EUR { get; set; } = 0.0301m;
        public decimal EUR_USD { get; set; } = 1.09m;
        public decimal USD_EUR { get; set; } = 0.92m;
    }


}
