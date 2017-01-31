using System.Collections.Generic;
using Itenso.TimePeriod;
using System;

namespace VechimeSoftware
{
    public struct PerioadaTotal
    {
        public int ANI, LUNI, ZILE;
    }

    public class Person
    {
        public int ID { get; set; }
        public string CNP { get; set; }
        public string Serie { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public List<Perioada> Perioade { get; set; }
        public string NumeIntreg
        {
            get
            {
                return Nume.ToUpper() + " " + Prenume.ToUpper();
            }
        }
        public PerioadaTotal perioadaMunca
        {
            get
            {
                PerioadaTotal total = new PerioadaTotal();
                int ani = 0, luni = 0, zile = 0;
                for (int i=0;i<Perioade.Count;i++)
                {
                    if(Perioade[i].IOM.ToUpper() == "MUNCA")
                    {
                        ani += Perioade[i].Difference.ElapsedYears;
                        luni += Perioade[i].Difference.ElapsedMonths;
                        zile += Perioade[i].Difference.ElapsedDays;
                    }   
                }

                total.ZILE = Convert.ToInt32(zile % 30.0);
                total.LUNI = (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) % 12;
                total.ANI = ani + (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) / 12;
                return total;
            }
        }
        public PerioadaTotal perioadaInv
        {
            get
            {
                PerioadaTotal total = new PerioadaTotal();
                int ani = 0, luni = 0, zile = 0;
                for (int i = 0; i < Perioade.Count; i++)
                {
                    if (Perioade[i].IOM.ToUpper() == "INVATAMANT")
                    {
                        ani += Perioade[i].Difference.ElapsedYears;
                        luni += Perioade[i].Difference.ElapsedMonths;
                        zile += Perioade[i].Difference.ElapsedDays;
                    }
                }

                total.ZILE = Convert.ToInt32(zile % 30.0);
                total.LUNI = (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) % 12;
                total.ANI = ani + (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) / 12;
                return total;
            }
        }

        public PerioadaTotal perioadaTotal
        {
            get
            {
                PerioadaTotal total = new PerioadaTotal();
                PerioadaTotal inv = perioadaInv;
                PerioadaTotal munca = perioadaMunca;
                int ani = 0, luni = 0, zile = 0;
                ani = inv.ANI + munca.ANI;
                luni = inv.LUNI + munca.LUNI;
                zile = inv.ZILE + munca.ZILE;
                total.ZILE = Convert.ToInt32(zile % 30.0);
                total.LUNI = (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) % 12;
                total.ANI = ani + (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) / 12;
                return total;

            }
        }
    }
}
