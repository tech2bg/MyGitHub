using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataLayer
{
    public class TreasuryData
    {
       public static Dictionary<string, string> TreasuryMap = new Dictionary<string, string>()
	        {  {"1M",   "CBM Govt"},  
               {"3M",   "CB3 Govt"},
               {"6M",   "CB6 Govt"},
               {"12M",  "CB12 Govt"}, 
               {"2Y",   "CT2 Govt"},
               {"3Y",   "CT3 Govt"},
               {"5Y",   "CT5 Govt"}, 
               {"7Y",   "CT7 Govt"},
               {"10Y",  "CT10 Govt"}, 
               {"30Y",  "CT30 Govt"}
            };

        public static DataRow[] GetTreasuryDataPerTicker(string asOfDate, string sql, string ticker)
        { 
            var db = new OracleData();
            var table = db.GetDataTable(sql);
            var dataRows = db.SelectDataRows(table, " Base_Name = '" + ticker 
                                + "' AND (FIELD = 'MATURITY' OR  FIELD = 'ISSUE DATE' or FIELD = 'CALC TYPE' or FIELD = 'QUOTE' OR FIELD = 'COUPON') ");
            return dataRows;
        } 

        public static DataRow[] GetTreasuryDataPerTicker(string asOfDate, OracleData db, DataTable table, string ticker)
        { 
            var dataRows = db.SelectDataRows(table, " Base_Name = '" + ticker
                                + "' AND (FIELD = 'MATURITY' OR  FIELD = 'ISSUE DATE' or FIELD = 'CALC TYPE' or FIELD = 'QUOTE' OR FIELD = 'COUPON') "); 
            return dataRows;
        }

        // retrieving all data for Treasury from TSR and keep them in a dictionary: dict,  for inputs to InflationCurve
        public static Dictionary<string, string> GetTreasuryData4ModelInput(string asOfDate, string sql)
        {
            var db = new OracleData();
            var table = db.GetDataTable(sql);
            var dict = new Dictionary<string, string>();
            foreach (var key in TreasuryMap.Keys)
            {
                var ticker = TreasuryMap[key];
                var valStr = "";
                var dataRows = GetTreasuryDataPerTicker(asOfDate, db, table, ticker);  // tenor => ticker mapping
                // var dataRows = GetTreasuryDataPerTicker(asOfDate, sql, ticker);  // avoid do database access every time
                for (var i = 0; i < dataRows.Count(); i++)
                {
                    valStr = valStr + (dataRows[i][1] + ":" + dataRows[i][2] + "|");
                }

                dict[key] = "Ticker:" + ticker + "|" + valStr;
            } 
            return dict;
        }   
       
        public static string TsrSql4Treasury = @"select base_name,
                case when field = 'CPN' then 'COUPON' 
                        when field = 'PX_LAST' then 'QUOTE'
                        when field = 'ISSUE_DT' then 'ISSUE DATE'
                        when field = 'CALC_TYP_DES' then 'CALC TYPE'
                        else FIELD 
                end as Field,  
                case when field in('ISSUE_DT', 'MATURITY') then to_char(to_date(string_value, 'yyyymmdd'), 'yyyy-mm-dd') 
                        else string_value
                end as value  
                from tsr.tsr_data
                where Base_Name in 
                ('CBM Govt',  
                 'CB3 Govt', 
                 'CB6 Govt', 
                 'CB12 Govt', 
                 'CT2 Govt', 
                 'CT3 Govt', 
                 'CT5 Govt', 
                 'CT7 Govt', 
                 'CT10 Govt', 
                 'CT30 Govt')
                and src_cd = 'BBG'
                and as_of_date < sysdate and sysdate <= dep_date
                and (field in ('ISSUE_DT', 'MATURITY', 'CPN', 'CALC_TYP_DES') or (src_date = '16-jul-2014' and field = 'PX_LAST'))
                --and (field in ('ISSUE_DT', 'MATURITY'))
                order by  field, string_value, base_name
                ";


        /***
        InflationCurveString treasuryData = new InflationCurveString();
        treasuryData["1M"] =
            "Ticker:CBM Govt|CALC TYPE:DISCOUNT|ISSUE DATE:2014-02-13|MATURITY:2014-08-14|COUPON:0.0|QUOTE:0.015";
        treasuryData["3M"] =
            "Ticker:CB3 Govt|CALC TYPE:DISCOUNT|ISSUE DATE:2013-10-17|MATURITY:2014-10-16|COUPON:0.0|QUOTE:0.015";
        treasuryData["6M"] =
            "Ticker:CB6 Govt|CALC TYPE:DISCOUNT|ISSUE DATE:2014-07-07|MATURITY:2015-01-15|COUPON:0.0|QUOTE:0.055";
        treasuryData["12M"] =
            "Ticker:CB12 Govt|CALC TYPE:DISCOUNT|ISSUE DATE:2014-06-26|MATURITY:2015-06-25|COUPON:0.0|QUOTE:0.09";
        treasuryData["2Y"] =
            "Ticker:CT2 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-06-30|MATURITY:2016-06-30|COUPON:0.5|QUOTE:100.039063";
        treasuryData["3Y"] =
            "Ticker:CT3 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-07-15|MATURITY:2017-07-15|COUPON:0.875|QUOTE:99.703125";
        treasuryData["5Y"] =
            "Ticker:CT5 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-06-30|MATURITY:2019-06-30|COUPON:1.625|QUOTE:99.710938";
        treasuryData["7Y"] =
            "Ticker:CT7 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-06-30|MATURITY:2021-06-30|COUPON:2.125|QUOTE:99.796875";
        treasuryData["10Y"] =
            "Ticker:CT10 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-05-15|MATURITY:2024-05-15|COUPON:2.5|QUOTE:99.8125";
        treasuryData["30Y"] =
            "Ticker:CT30 Govt|CALC TYPE:STREET CONVENTION|ISSUE DATE:2014-05-15|MATURITY:2044-06-30|COUPON:3.375|QUOTE:100.78125";
        ****/


    }
}
