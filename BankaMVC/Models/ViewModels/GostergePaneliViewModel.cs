using BankaMVC.Models.DTOs;
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class GostergePaneliViewModel
    {
        public List<Hesap> Hesaplar { get; set; } = new List<Hesap>();
        public List<Kart> Kartlar { get; set; } = new List<Kart>();
        public KullaniciBilgileriDto Kullanici { get; set; } = new KullaniciBilgileriDto();
    }
}
