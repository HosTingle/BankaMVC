using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class GirisOlayi
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        [Required]
        [StringLength(45)] // IPv6 için yeterli uzunluk
        public string IpAdresi { get; set; }

        public bool Basarili { get; set; }

        public DateTime Zaman { get; set; } = DateTime.Now;
    }

}
