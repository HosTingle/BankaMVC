using BankaMVC.Models.DTOs;
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class SonHareketlerViewModel
    {
        public string AktifTab { get; set; } = "vadesiz";
        public List<SonHareketlerDto> HesapIslemler  = new List<SonHareketlerDto>(); 
        public List<SonHareketlerDto> KrediKartiIslemleri = new List<SonHareketlerDto>();
    }
}
