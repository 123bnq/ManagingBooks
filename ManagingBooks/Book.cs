using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingBooks
{
    class Book
    {
        private int m_NoAuthor;
        private int m_NoSignature;

        public int Number { get; set; }
        public string[] Signatures { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public string Publisher { get; set; }
        public int Version { get; set; }
        public int Year { get; set; }
        public string DayBought { get; set; }
        public int Pages { get; set; }
        public string Price { get; set; }
    }
}
