
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka.Varlıklar.DTOs
{
    public class ParaGondermeDto
    {
        public int KullaniciId { get; set; } 
        public string GonderenHesapId { get; set; }

        public string AliciHesapId { get; set; }

        public decimal Tutar { get; set; }

        public string IslemTipi { get; set; }

        public string Aciklama { get; set; }

        public string OdemeAraci { get; set; }

    }
}
