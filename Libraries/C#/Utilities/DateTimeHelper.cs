using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class DateTimeHelper 
    {
        // given yyyy-mm-dd, convert it to DateTime var
        public static DateTime String2DateTime(string yyyymmdd) 
        {
            var separator = new char[] { '-' };
            string[] dateStrs = yyyymmdd.Split(separator);
            if (dateStrs.Length < 3)
            {
                throw new Exception("Date string expecting yyyy-mm-dd format."); 
            }

            int year, month, day;
            int.TryParse(dateStrs[0], out year);
            int.TryParse(dateStrs[1], out month);
            int.TryParse(dateStrs[2], out day);
            return (new DateTime(year, month, day));
        }
    }
}
