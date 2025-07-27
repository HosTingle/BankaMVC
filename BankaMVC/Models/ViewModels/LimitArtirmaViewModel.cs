using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.ViewModels
{
    public class LimitArtirmaViewModel
    {
        [Required(ErrorMessage = "Kart ID gereklidir.")]
        public int KartId { get; set; }

        [Required(ErrorMessage = "Yeni limit miktarı gereklidir.")]
        [Range(1000, 100000, ErrorMessage = "Limit 1.000 ₺ ile 100.000 ₺ arasında olmalıdır.")]
        public decimal YeniLimit { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Aciklama { get; set; }
    }
}
