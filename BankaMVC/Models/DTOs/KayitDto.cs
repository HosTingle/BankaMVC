using BankaMVC.Models.Somut;

namespace BankaMVC.Models.DTOs
{
    public class KayitDto 
    {
        public string AdSoyad { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }

        public string Sifre { get; set; }

        public SubeDto Sube { get; set; }

        public bool aktif { get; set; }  
    }
}
