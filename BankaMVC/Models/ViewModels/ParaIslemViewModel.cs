
using BankaMVC.Models.Somut;
using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.ViewModels
{
    public class ParaIslemViewModel
    {
        [Required(ErrorMessage = "İşlem türü seçilmelidir")]
        public string IslemTuru { get; set; } // "cek" veya "yatir"

        [Required(ErrorMessage = "Araç türü seçilmelidir")]
        public string AracTuru { get; set; } // "hesap" veya "kart"

        public int? SecilenHesapId { get; set; }

        public string? SecilenKartId { get; set; }
        [Range(10, 100000, ErrorMessage = "Yatırılacak tutar 10 TL ile 100.000 TL arasında olmalıdır.")]
        public decimal? YTutar { get; set; }

        [Range(10, 100000, ErrorMessage = "Çekilecek tutar 10 TL ile 100.000 TL arasında olmalıdır.")]
        public decimal? Tutar { get; set; }

        public List<Hesap>? Hesaplar { get; set; } = new List<Hesap>();
        public List<Kart>? Kartlar { get; set; } = new List<Kart>();
    }
}
