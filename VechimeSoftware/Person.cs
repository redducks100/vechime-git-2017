using System.Collections.Generic;
using Itenso.TimePeriod;
using System;

namespace VechimeSoftware
{

    public class Transa
    {
        public string TransaString = "";

        public enum TransaType
        {
            MUNCA,
            INVATAMANT
        }

        public static string[] TransaMuncaStrings =
        {
           "0-3 ANI",
           "3-5 ANI",
           "5-10 ANI",
           "10-15 ANI",
           "15-20 ANI",
           "PESTE 20 DE ANI"
        };

        public static string[] TransaInvatamantStrings =
       {
           "0-1 ANI",
           "1-6 ANI",
           "6-10 ANI",
           "10-14 ANI",
           "14-18 ANI",
           "18-22 ANI",
           "22-25 ANI",
           "25-30 ANI",
           "30-35 ANI",
           "35-40 ANI",
           "PESTE 40 DE ANI"
        };

        public static string GetCurrentTransaString(TimePeriod period, TransaType type)
        {
            if(type == TransaType.INVATAMANT)
            {
                if (period.Years >= 0 && period.Years < 1)
                    return TransaInvatamantStrings[0];
                if (period.Years >= 1 && period.Years < 6)
                    return TransaInvatamantStrings[1];
                if (period.Years >= 6 && period.Years < 10)
                    return TransaInvatamantStrings[2];
                if (period.Years >= 10 && period.Years < 14)
                    return TransaInvatamantStrings[3];
                if (period.Years >= 14 && period.Years < 18)
                    return TransaInvatamantStrings[4];
                if (period.Years >= 18 && period.Years < 22)
                    return TransaInvatamantStrings[5];
                if (period.Years >= 22 && period.Years < 25)
                    return TransaInvatamantStrings[6];
                if (period.Years >= 25 && period.Years < 30)
                    return TransaInvatamantStrings[7];
                if (period.Years >= 30 && period.Years < 35)
                    return TransaInvatamantStrings[8];
                if (period.Years >= 35 && period.Years < 40)
                    return TransaInvatamantStrings[9];

                return TransaInvatamantStrings[10];
            }
            else
            {
                if (period.Years >= 0 && period.Years < 3)
                    return TransaInvatamantStrings[0];
                if (period.Years >= 3 && period.Years < 5)
                    return TransaInvatamantStrings[1];
                if (period.Years >= 5 && period.Years < 10)
                    return TransaInvatamantStrings[2];
                if (period.Years >= 10 && period.Years < 15)
                    return TransaInvatamantStrings[3];
                if (period.Years >= 15 && period.Years < 20)
                    return TransaInvatamantStrings[4];

                return TransaInvatamantStrings[5];
            }
        }

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

        private TimePeriod _perioadaMunca;
        private TimePeriod _perioadaInv;

        public TimePeriod PerioadaMunca
        {
            get
            {
                if (_perioadaMunca == null)
                {
                    TimePeriod total = new TimePeriod();
                    int ani = 0, luni = 0, zile = 0;
                    for (int i = 0; i < Perioade.Count; i++)
                    {
                        ani += Perioade[i].Difference.ElapsedYears;
                        luni += Perioade[i].Difference.ElapsedMonths;
                        zile += Perioade[i].Difference.ElapsedDays;
                    }
                    total.Days = Convert.ToInt32(zile % 30);
                    total.Months = (luni + Convert.ToInt32(zile / 30)) % 12;
                    total.Years = ani + (luni + Convert.ToInt32(zile / 30)) / 12;
                    _perioadaMunca = total;
                }
                return _perioadaMunca;
            }
        }
        public TimePeriod PerioadaInv
        {
            get
            {
                if (_perioadaInv == null)
                {
                    TimePeriod total = new TimePeriod();
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

                    total.Days = Convert.ToInt32(zile % 30);
                    total.Months = (luni + Convert.ToInt32(zile / 30)) % 12;
                    total.Years = ani + (luni + Convert.ToInt32(zile / 30)) / 12;
                    _perioadaInv = total;
                }
                return _perioadaInv;
            }
        }

        public Transa CurrentTransa { get; set; }

    }
}
