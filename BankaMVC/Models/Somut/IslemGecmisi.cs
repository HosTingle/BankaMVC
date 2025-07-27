using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class IslemGecmisi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Id zorunludur.")]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "İşlem türü zorunludur.")]
        [StringLength(50, ErrorMessage = "İşlem türü en fazla 50 karakter olabilir.")]
        public string IslemTuru { get; set; }

        [Required(ErrorMessage = "Tutar zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Tutar negatif olamaz.")]
        public decimal Tutar { get; set; }

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Bakiye zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Bakiye negatif olamaz.")]
        public decimal Bakiye { get; set; }

        [Required(ErrorMessage = "Hesap tipi zorunludur.")]
        [RegularExpression("^(Vadesiz|KrediKarti)$", ErrorMessage = "Hesap tipi sadece 'Vadesiz' veya 'KrediKarti' olabilir.")]
        public string HesapTipi { get; set; }
    }
}
