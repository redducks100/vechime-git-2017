using Itenso.TimePeriod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VechimeSoftware
{
    public class TimePeriod
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }

        public static TimePeriod HalfTime(TimePeriod time)
        {
            int addMonths = 0, addDays = 0;
            if (time.Years % 2 == 1)
                addMonths = 6;

            time.Years /= 2;

            if (time.Months % 2 == 1)
                addDays = 15;

            time.Months /= 2;

            if (time.Days % 2 == 1)
                time.Days += 1;

            time.Days /= 2;

            time.Months += addMonths;
            time.Days += addDays;

            return time;
        }

        public static TimePeriod QuarterTime(TimePeriod time)
        {
            int addMonths = 0, addDays = 0;
            if (time.Years % 4 == 1)
                addMonths = 3;
            else if (time.Years % 4 == 2)
                addMonths = 6;
            else if (time.Years % 4 == 3)
                addMonths = 9;

            time.Years /= 4;

            if (time.Months % 4 == 1)
                addDays = 8;
            else if (time.Months % 4 == 2)
                addDays = 15;
            else if (time.Months % 4 == 3)
                addDays = 23;

            time.Months /= 4;


            if (time.Days % 4 == 2)
                time.Days += 2;
            else if (time.Days % 4 == 3)
                time.Days++;

            time.Days /= 4;

            time.Months += addMonths;
            time.Days += addDays;

            return time;
        }

        public static TimePeriod CalculatePeriodTime(Perioada perioada)
        {
            // Suma timpului
            int ani = 0, luni = 0, zile = 0;

            if(perioada.CFS && perioada.TipCFS.ToUpper() == "PT NEVOI PERSONALE")
            {
               TimePeriod zeroPer = new TimePeriod();
               zeroPer.Days = zeroPer.Months = zeroPer.Years = 0;
               return zeroPer;
            }

            // Doar timpul in invatamant
            int aniInv = 0, luniInv = 0, zileInv = 0;

            DateDiff diff = perioada.Difference;

            DateTime changeSomaj = new DateTime(2002, 03, 01);

            DateTime changeNorma = new DateTime(2005, 07, 05); //ORDONANTA 65 - 05 IULIE 2005

            DateTime changeConcediu = new DateTime(2011, 02, 07);

            if (!perioada.Somaj || perioada.DTInceput.CompareTo(changeSomaj) < 0)
            {
                if (perioada.DTSfarsit.CompareTo(changeSomaj) > 0 && perioada.Somaj)
                    diff =  Utils.DateDiffFixed(perioada.DTInceput, changeSomaj);

                TimePeriod np = new TimePeriod();
                np.Years = diff.ElapsedYears;
                np.Months = diff.ElapsedMonths;
                np.Days = diff.ElapsedDays;

                // aplic norma
                //Somajul este calculat mereu 1/1
                if (perioada.DTInceput.CompareTo(changeNorma) < 0 && perioada.Norma != "1/1" && !perioada.Somaj && !perioada.CFS)
                {
                    if (perioada.DTSfarsit.CompareTo(changeNorma) > 0)
                    {
                        diff = Utils.DateDiffFixed(perioada.DTInceput, changeNorma);

                        np.Years = diff.ElapsedYears;
                        np.Months = diff.ElapsedMonths;
                        np.Days = diff.ElapsedDays;

                        if (perioada.Norma == "1/2")
                            np = HalfTime(np);
                        else if (perioada.Norma == "1/4")
                            np = QuarterTime(np);

                        diff = Utils.DateDiffFixed(changeNorma, perioada.DTSfarsit);

                        np.Years += diff.ElapsedYears;
                        np.Months += diff.ElapsedMonths;
                        np.Days += diff.ElapsedDays;
                    }
                    else
                    {
                        if (perioada.Norma == "1/2")
                            np = HalfTime(np);
                        else if (perioada.Norma == "1/4")
                            np = QuarterTime(np);
                    }
                }

                if(perioada.CFS && perioada.TipCFS.ToUpper() == "PT STUDII")
                {
                    if(perioada.DTInceput.CompareTo(changeConcediu) < 0 && perioada.DTSfarsit.CompareTo(changeConcediu) > 0)
                    {
                        diff = Utils.DateDiffFixed(perioada.DTInceput, changeConcediu);
                        np.Years = diff.ElapsedYears;
                        np.Months = diff.ElapsedMonths;
                        np.Days = diff.ElapsedDays+1;
                    }
                    else if(perioada.DTInceput.CompareTo(changeConcediu) > 0 && perioada.DTSfarsit.CompareTo(changeConcediu) > 0)
                    {
                        np.Years = 0;
                        np.Months = 0;
                        np.Days = 0;
                    }
                }

                ani += np.Years;
                luni += np.Months;
                zile += np.Days;
            }

            TimePeriod periodSum = new TimePeriod();

            periodSum.Days = Convert.ToInt32(zile % 30.0);
            periodSum.Months = (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) % 12;
            periodSum.Years = ani + (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) / 12;

            return periodSum;
        }
    }

    public class TimePeriodSum : TimePeriod
    {
        public int YearsInv { get; set; }
        public int MonthsInv { get; set; }
        public int DaysInv { get; set; }

        public static TimePeriodSum CalculateIndividualTime(List<Perioada> perioade)
        {
            // Suma timpului
            int ani = 0, luni = 0, zile = 0;

            // Doar timpul in invatamant
            int aniInv = 0, luniInv = 0, zileInv = 0;

            perioade = perioade.OrderBy(c => c.DTSfarsit).ToList();

            for (int i = 0; i < perioade.Count - 1; i++)
            {
                if (perioade[i].DTSfarsit.CompareTo(perioade[i + 1].DTInceput) == 0)
                    perioade[i + 1].DTInceput = perioade[i + 1].DTInceput.AddDays(1);
            }

            for (int i = 0; i < perioade.Count; i++)
            {
                DateDiff diff = perioade[i].Difference;

                DateTime changeSomaj = new DateTime(2002, 03, 01);

                DateTime changeNorma = new DateTime(2005, 07, 05);

                DateTime changeConcediu = new DateTime(2011, 02, 07);

                if ((!perioade[i].Somaj || perioade[i].DTInceput.CompareTo(changeSomaj) < 0) && perioade[i].CFS == false)
                {
                    if (perioade[i].DTSfarsit.CompareTo(changeSomaj) > 0 && perioade[i].Somaj)
                        diff = Utils.DateDiffFixed(perioade[i].DTInceput, changeSomaj);

                    TimePeriod np = new TimePeriod();


                    np.Years = diff.ElapsedYears;
                    np.Months = diff.ElapsedMonths;
                    np.Days = diff.ElapsedDays;

                    // aplic norma
                    if (perioade[i].DTInceput.CompareTo(changeNorma) < 0 && perioade[i].Norma != "1/1" && !perioade[i].Somaj)
                    {
                        if (perioade[i].DTSfarsit.CompareTo(changeNorma) > 0)
                        {
                            diff = Utils.DateDiffFixed(perioade[i].DTInceput, changeNorma);

                            np.Years = diff.ElapsedYears;
                            np.Months = diff.ElapsedMonths;
                            np.Days = diff.ElapsedDays;

                            if (perioade[i].Norma == "1/2")
                                np = HalfTime(np);
                            else if (perioade[i].Norma == "1/4")
                                np = QuarterTime(np);

                            diff = Utils.DateDiffFixed(changeNorma, perioade[i].DTSfarsit);

                            np.Years += diff.ElapsedYears;
                            np.Months += diff.ElapsedMonths;
                            np.Days += diff.ElapsedDays;
                        }
                        else
                        {
                            if (perioade[i].Norma == "1/2")
                                np = HalfTime(np);
                            else if (perioade[i].Norma == "1/4")
                                np = QuarterTime(np);
                        }
                    }

                    if (perioade[i].CFS && perioade[i].TipCFS.ToUpper() == "PT STUDII")
                    {
                        if (perioade[i].DTInceput.CompareTo(changeConcediu) < 0 && perioade[i].DTSfarsit.CompareTo(changeConcediu) > 0)
                        {
                            diff = Utils.DateDiffFixed(perioade[i].DTInceput, changeConcediu);
                            np.Years = diff.ElapsedYears;
                            np.Months = diff.ElapsedMonths;
                            np.Days = diff.ElapsedDays+1;
                        }
                        else if (perioade[i].DTInceput.CompareTo(changeConcediu) > 0 && perioade[i].DTSfarsit.CompareTo(changeConcediu) > 0)
                        {
                            np.Years = 0;
                            np.Months = 0;
                            np.Days = 0;
                        }
                    }

                    if (perioade[i].TipCFS.ToUpper() != "PT NEVOI PERSONALE")
                    {
                        ani += np.Years;
                        luni += np.Months;
                        zile += np.Days;
                    }

                    if (perioade[i].IOM.ToUpper() == "INVATAMANT")
                    {
                        aniInv += np.Years;
                        luniInv += np.Months;
                        zileInv += np.Days;
                    }
                }
            }

            TimePeriodSum periodsSum = new TimePeriodSum();

            // Clalculez timp in invatamant
            periodsSum.DaysInv = Convert.ToInt32(zileInv % 30);
            periodsSum.MonthsInv = (luniInv + Convert.ToInt32(Math.Floor(zileInv / 30.0))) % 12;
            periodsSum.YearsInv = aniInv + (luniInv + Convert.ToInt32(Math.Floor(zileInv / 30.0))) / 12;

            // Calculez timp total
            periodsSum.Days = Convert.ToInt32(zile % 30.0);
            periodsSum.Months = (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) % 12;
            periodsSum.Years = ani + (luni + Convert.ToInt32(Math.Floor(zile / 30.0))) / 12;

            return periodsSum;
        }
    }
}
