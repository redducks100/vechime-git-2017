using System;
using Itenso.TimePeriod;

namespace VechimeSoftware
{
    public class Perioada
    {
        public int ID { get; set; }
        public int ListNumber { get; set; }
        public DateTime DTInceput { get; set; }
        public DateTime DTSfarsit { get; set; }
        public bool CFS { get; set; }
        public string TipCFS { get; set; }
        public string Norma { get; set; }
        public string Functie { get; set; }
        public string IOM { get; set; }
        public string LocMunca { get; set; }
        public bool Lucreaza { get; set; }
        public bool Somaj { get; set; }

        public DateDiff Difference {
            get
            {
                return new DateDiff(DTInceput.Subtract(new TimeSpan(1,0,0,0,0)), DTSfarsit.Subtract(new TimeSpan(1, 0, 0, 0, 0)));
            }
        }
    }
}
