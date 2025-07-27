using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BankaMVC.Models.Somut
{
    public class KartIslem 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kart bilgisi zorunludur.")]
        public int KartId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Güncel bakiye negatif olamaz.")]
        public decimal GuncelBakiye { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Tutar { get; set; }

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Durum boş olamaz.")]
        [StringLength(30, ErrorMessage = "Durum en fazla 30 karakter olabilir.")]
        public string Durum { get; set; } = string.Empty;

        [Required(ErrorMessage = "İşlem türü boş olamaz.")]
        [StringLength(50, ErrorMessage = "İşlem türü en fazla 50 karakter olabilir.")]
        public string IslemTuru { get; set; } = string.Empty;

        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        public Kart? Kart { get; set; }
    }
}
