
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka.Varlıklar.DTOs
{
    public class KullaniciKayitDto
    {

        public string AdSoyad { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }
        public string Sifre { get; set; }
        public int Sube { get; set; }  
        public bool Aktif { get; set; }
    }
}
