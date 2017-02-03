using Itenso.TimePeriod;
using System;
using System.Collections.Generic;

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
            if (type == TransaType.INVATAMANT)
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
                    return TransaMuncaStrings[0];
                if (period.Years >= 3 && period.Years < 5)
                    return TransaMuncaStrings[1];
                if (period.Years >= 5 && period.Years < 10)
                    return TransaMuncaStrings[2];
                if (period.Years >= 10 && period.Years < 15)
                    return TransaMuncaStrings[3];
                if (period.Years >= 15 && period.Years < 20)
                    return TransaMuncaStrings[4];

                return TransaMuncaStrings[5];
            }
        }

        public Transa(TimePeriod period, TransaType type)
        {
            TransaString = GetCurrentTransaString(period, type);
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

        public TimePeriod PerioadaMunca
        {
            get
            {
                TimePeriodSum total = TimePeriodSum.CalculateIndividualTime(Perioade);
                TimePeriod period = new TimePeriod();
                period.Days = total.Days;
                period.Months = total.Months;
                period.Years = total.Years;
                return period;
            }
        }
        public TimePeriod PerioadaInv
        {
            get
            {
                TimePeriodSum total = TimePeriodSum.CalculateIndividualTime(Perioade);
                TimePeriod period = new TimePeriod();
                period.Days = total.DaysInv;
                period.Months = total.MonthsInv;
                period.Years = total.YearsInv;
                return period;
            }
        }

        public Transa CurrentTransaMunca
        {
            get
            {
                return new Transa(PerioadaMunca, Transa.TransaType.MUNCA);
            }
        }
        public Transa CurrentTransaInv
        {

            get
            {
                return new Transa(PerioadaInv, Transa.TransaType.INVATAMANT);
            }
        }

        public string PreviousTransaMunca { get; set; }
        public string PreviousTransaInv { get; set; }
    }
}
