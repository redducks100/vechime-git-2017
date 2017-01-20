using System;

namespace VechimeSoftware
{
    public class Perioada
    {
        public int ID { get; set; }
        public int ListNumber { get; set; }
        public DateTime DTInceput { get; set; }
        public DateTime DTSfarsit { get; set; }
        public int CFSZile_Personal { get; set; }
        public int CFSLuni_Personal { get; set; }
        public int CFSAni_Personal { get; set; }
        public int CFSZile_Studii { get; set; }
        public int CFSLuni_Studii { get; set; }
        public int CFSAni_Studii { get; set; }
        public string Norma { get; set; }
        public string Functie { get; set; }
        public string IOM { get; set; }
        public string LocMunca { get; set; }
        public bool Lucreaza { get; set; }
        public bool Somaj { get; set; }
    }
}
