using Itenso.TimePeriod;
using System;

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
        public bool LucreazaUnitateaCurenta { get; set; }
        public bool Somaj { get; set; }

        public bool Modified { get; set; }

        public DateDiff Difference
        {
            get
            {
                return Utils.DateDiffFixed(DTInceput, DTSfarsit);
            }
        }
    }
}
