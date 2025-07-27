using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka.Varlıklar.DTOs
{
    public class DestekTalebiOlusturDto
    {
        public int Id { get; set; } 
        public int KullaniciId { get; set; }

        public string Konu { get; set; }

        public string Mesaj { get; set; }

        public string? Kategori { get; set; }

        public string? AdSoyad { get; set; }

        public DateTime? Tarih { get; set; }
        public string? Yanit { get; set; }
        public string? Durum { get; set; }
    }
}
