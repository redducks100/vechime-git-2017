using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                addDays = 7;
            else if (time.Months % 4 == 2)
                addDays = 15;
            else if (time.Months % 4 == 3)
                addDays = 23;

            time.Months /= 4;
            time.Days /= 4;

            time.Months += addMonths;
            time.Days += addDays;


            return time;
        }

    }

}
