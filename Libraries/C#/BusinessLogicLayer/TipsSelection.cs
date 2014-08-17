using System; 
using System.Collections.Generic;
using System.Data;
using System.Linq; 

namespace BusinessLogicLayer
{
    public class TipsSelection
    {

        // in rows[], TipTicker, MATURITY, date
        //
        // pick a record with the closest distance in maturity comparing with baseDate
        public static int PickRightTipByTerm(string term, DataRow[] rows, string baseDate)
        {
            var refDate = Utilities.DateTimeHelper.String2DateTime(baseDate);

            string[] strs = term.Split('Y');
            var years = int.Parse(strs[0]);
            var termDate = refDate.AddYears(years);
            var termDays = (termDate - refDate).Days;
            // DataRow[] rows pick the nearest 
            // construct an array of dates, make the difference to the baseDate
            //

            //Console.WriteLine("termDays = {0}: ",  termDays);
            var arrList = new List<DateTime>(40);
            var diffList = new List<int>(40);
            for (var i = 0; i < rows.Count(); i++)
            {
                var maturity = rows[i][2].ToString();
                var maturityDate = Utilities.DateTimeHelper.String2DateTime(maturity);
                arrList.Add(maturityDate);
                
                diffList.Add(Math.Abs((maturityDate - refDate).Days - termDays)); 
                //Console.WriteLine("DiffList[{0}] = {1}",i, diffList[i]);
            }

            return FindIndexOfMin(diffList);
        }

        private static int FindIndexOfMin(List<int> list)
        {
            var idx = 0;
            var min = 30*365;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                {
                    idx = i;
                    min = list[i];
                }
            }

            return idx;
        }
    }
}
