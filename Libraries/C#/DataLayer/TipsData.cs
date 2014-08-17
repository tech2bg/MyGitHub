using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogicLayer;

namespace DataLayer
{
    public class TipsData
    {
        public static Dictionary<string, string> TipsMap = new Dictionary<string, string>()
	        {  {"1Y",   "GTII1TBD Govt"},  
               {"2Y",   "GTII2TBD Govt"},
               {"3Y",   "GTII3TBD Govt"},
               {"5Y",   "GTII5 Govt"}, 
               {"7Y",   "GTII7TBD Govt"},
               {"10Y",  "GTII10 Govt"},
               {"20Y",  "GTII20 Govt"}, 
               {"30Y",  "GTII30 Govt"} 
            };

        /*** The goal is to produce the following list:
            tipsData["1Y"] = "Ticker:912828EA Govt|ISSUE DATE:2005-07-15|MATURITY:2015-07-15|COUPON:1.875|QUOTE:103.539063"; 
            tipsData["2Y"] = "Ticker:912828FL Govt|ISSUE DATE:2006-07-17|MATURITY:2016-07-15|COUPON:2.5|QUOTE:108.109375"; 
            tipsData["3Y"] = "Ticker:912828GX Govt|ISSUE DATE:2007-07-16|MATURITY:2017-07-15|COUPON:2.625|QUOTE:111.289063"; 
            tipsData["5Y"] = "Ticker:GTII5 Govt|ISSUE DATE:2014-04-30|MATURITY:2019-04-15|COUPON:0.125|QUOTE:102.351563";
            tipsData["7Y"] = "Ticker:912828QV Govt|ISSUE DATE:2011-07-29|MATURITY:2021-07-15|COUPON:.625|QUOTE:105.039063"; 
            tipsData["10Y"] ="Ticker:GTII10 Govt|ISSUE DATE:2014-01-31|MATURITY:2024-01-15|COUPON:0.625|QUOTE:103.300781";
            tipsData["20Y"] ="Ticker:GTII20 Govt|ISSUE DATE:2001-10-15|MATURITY:2032-04-15|COUPON:3.375|QUOTE:144.796875";
            tipsData["30Y"] ="Ticker:GTII30 Govt|ISSUE DATE:2014-02-28|MATURITY:2044-02-15|COUPON:1.375|QUOTE:109.921875";
          ***/

        public static DataRow[] GetTipsDataByMaturity(string asOfDate, string sql)
        { 
            var db = new OracleData();

            var ds = db.GetDataSet(sql);
            var table = db.GetDataTable(ds);
            var dataRows = db.SelectDataRows(table, " FIELD = 'MATURITY' "); 

            return dataRows;
        }

        // this off-run TIPS list could be changed each month. Dawn's team helps to maintain the list of the off-the-run TIPS securities
        // so that we can search and pick the candidates for 1Y, 2Y, 3Y, and 7Y TIPS.  
        // Other on-the-run TIPS (5Y, 10Y, 20Y and 30Y) are available as generic TIPS in Bloomberg
        public static string TsrSql4OffRunTips = @"select base_name,
                case when field = 'CPN' then 'COUPON' 
                        when field = 'PX_LAST' then 'PRICE'
                        when field = 'ISSUE_DT' then 'ISSUE DATE'
                        when field = 'CALC_TYP_DES' then 'CALC TYPE'
                        else FIELD 
                end as Field,  
                case when field in('ISSUE_DT', 'MATURITY') then to_char(to_date(string_value, 'yyyymmdd'), 'yyyy-mm-dd') 
                        else string_value
                end as value  
                from tsr.tsr_data
                where Base_Name in (
                '912828DH Govt',
                '912828MY Govt',
                '912828EA Govt',
                '912828ET Govt',
                '912828QD Govt',
                '912828FL Govt',
                '912828GD Govt',
                '912828SQ Govt',
                '912828GX Govt',
                '912828HN Govt',
                '912828UX Govt',
                '912828JE Govt',
                '912828JX Govt',
                '912828C9 Govt',
                '912828LA Govt',
                '912828MF Govt',
                '912828NM Govt',
                '912828PP Govt',
                '912828QV Govt',
                '912828SA Govt',
                '912828TE Govt',
                '912828UH Govt',
                '912828VM Govt',
                '912828B2 Govt',
                '912828WU Govt',
                '912810FR Govt',
                '912810FS Govt',
                '912810PS Govt',
                '912810PV Govt',
                '912810FD Govt',
                '912810PZ Govt',
                '912810FH Govt',
                '912810FQ Govt',
                '912810QF Govt',
                '912810QP Govt',
                '912810QV Govt',
                '912810RA Govt',
                '912810RF Govt')
                and src_cd = 'BBG'
                and as_of_date < sysdate and sysdate <= dep_date
                and (field in ('ISSUE_DT', 'MATURITY', 'CPN', 'CACL_TYP_DES')
                or (src_date = '16-jul-2014' and field = 'PX_LAST'))
                --and (field in ('ISSUE_DT', 'MATURITY'))
                order by  field, string_value, base_name
                ";

        // ****************************
        // Once we have the candidates of Tips, and override the TipsMap
        // 
        public static Dictionary<string, string> PickOffRunTipsAndSetupAll(string asOfDate)
        {

            var result = GetTipsDataByMaturity(asOfDate, TsrSql4OffRunTips);

            var tipsPickList = new List<string>() { "1Y", "2Y", "3Y", "7Y" };

            foreach (var tenor in tipsPickList)
            {
                var minIndex = TipsSelection.PickRightTipByTerm(tenor, result, asOfDate);
                // Console.WriteLine("{0} : {1} ==> {2}, {3}, {4} ", tenor, minIndex, result[minIndex][0], result[minIndex][1], result[minIndex][2]); 
                // where we need: tenor, result[idx][0] is tikcer, result[idx][1] is MATURTY, and  result[idx][2] is yyyy-mm-dd

                // replace TipsMap
                TipsMap[tenor] = result[minIndex][0].ToString();  // ticker 
            }
 
            return TipsMap;
        }

        public static string UpdateTsrSql4Tips(string baseNamelist)
        { 
            return @"select base_name,
                case when field = 'CPN' then 'COUPON' 
                        when field = 'PX_LAST' then 'QUOTE'
                        when field = 'ISSUE_DT' then 'ISSUE DATE'
                        when field = 'CALC_TYP_DES' then 'CALC TYPE'
                        else FIELD 
                end as Field,  
                case when field in('ISSUE_DT', 'MATURITY') then to_char(to_date(string_value, 'yyyymmdd'), 'yyyy-mm-dd') 
                        else string_value
                end as Quote  
                from tsr.tsr_data
                where Base_Name in 
                (" + baseNamelist + @")
                and src_cd = 'BBG'
                and as_of_date < sysdate and sysdate <= dep_date
                and (field in ('ISSUE_DT', 'MATURITY', 'CPN', 'CACL_TYP_DES') or (src_date = '16-jul-2014' and field = 'PX_LAST'))
                -- and (field in ('ISSUE_DT', 'MATURITY'))
                order by  field, string_value, base_name
                ";
        }

        // Prepapring TIPS Inputs for Model
        public static DataRow[] GetTipsDataPerTicker(string asOfDate, string sql, string ticker)
        {
            var db = new OracleData();

            // prepare baseNameList:  'GTII5 Govt, GTII10 Govt, GTII30 Govt'
            var baseNameList = TipsMap.Keys.Aggregate("'", (current, key) => current + TipsMap[key] + ",");
            baseNameList = baseNameList.TrimEnd(',');

            // dynamically get the baseNameList for the query
            var mySql = UpdateTsrSql4Tips(baseNameList);
            var table = db.GetDataTable(mySql);
            var dataRows = db.SelectDataRows(table, " Base_Name = '" + ticker
                                + "' AND (FIELD = 'MATURITY' OR FIELD = 'CALC TYPE' OR  FIELD = 'ISSUE DATE' or  FIELD = 'QUOTE' OR FIELD = 'COUPON') ");
            return dataRows;
        } 

        public static DataRow[] GetTipsDataPerTicker(string asOfDate, OracleData db, DataTable table, string ticker)
        {
            var dataRows = db.SelectDataRows(table, " Base_Name = '" + ticker
                                + "' AND (FIELD = 'MATURITY' OR FIELD = 'CALC TYPE' OR  FIELD = 'ISSUE DATE' or  FIELD = 'QUOTE' OR FIELD = 'COUPON') ");
            return dataRows;
        }

        // retrieving all data for Tips from TSR and keep them in a dictionary: dict,  for inputs to InflationCurve
        public static Dictionary<string, string> GetTipsData4ModelInput(string asOfDate)
        {
            var db = new OracleData();

            // prepare baseNameList:  'GTII5 Govt, GTII10 Govt, GTII30 Govt'
            var baseNameList = TipsMap.Keys.Aggregate("'", (current, key) => current + TipsMap[key] + "', '");
            baseNameList = baseNameList.TrimEnd(new char[]{',', ' ', '\''});
            baseNameList = baseNameList + "'";

            // dynamically get the baseNameList for the query
            var mySql = UpdateTsrSql4Tips(baseNameList);

            // dynamically get the baseNameList for the query
            var table = db.GetDataTable(mySql);
            var dict = new Dictionary<string, string>();
            foreach (var key in TipsMap.Keys)
            {
                var ticker = TipsMap[key];
                var valStr = "";
                var dataRows = GetTipsDataPerTicker(asOfDate, db, table, ticker);  // tenor => ticker mapping
                // var dataRows = GetTipsDataPerTicker(asOfDate, sql, ticker);  // avoid do database access every time
                for (var i = 0; i < dataRows.Count(); i++)
                {
                    valStr = valStr + (dataRows[i][1] + ":" + dataRows[i][2] + "|");
                }

                dict[key] = "Ticker:" + ticker + "|" + valStr;
            }
            return dict;
        }   
       
    }
}
