using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace DataLayer
{
    public class OracleData
    {
        private const string DataSource = "IDWQA.World";
        private const string ConnString = "Data Source=" + DataSource + "; Pooling=True;User ID=ALGO_AIG; Password=ALG_345p$";
        private string StrConnection { get; set; }

        public OracleData()
        {
            StrConnection = ConnString;
        }

        /// <summary>
        /// Create the object using this connection string
        /// </summary>
        /// <param name="connectionString" />
        public OracleData(string connectionString)
        {
            StrConnection = connectionString;
        }
        /// <summary>
        /// Get dataset from SQL String
        /// </summary>
        /// <param name="strSqlQuery" />
        /// <returns></returns>
        /// 
        public DataSet GetDataSet(string strSqlQuery)
        {
            DataSet ds;
            using (var conn = new OracleConnection(StrConnection))
            { 
                ds = new DataSet();
                var da = new OracleDataAdapter(strSqlQuery, conn); 
                da.Fill(ds);
            }
            return ds; 
        }

        /****
        public DataSet GetDataSet(string strSqlQuery, string baseNameList)
        {
            DataSet ds;
            using (var conn = new OracleConnection(StrConnection))
            {
                ds = new DataSet(); 
                var da = new OracleDataAdapter();
                da.SelectCommand = new OracleCommand(strSqlQuery, conn);
                // passing the parameter of tipsList into the command
                da.SelectCommand.Parameters.Add("pBaseNameList", OracleDbType.Varchar2, ParameterDirection.Input);
                da.Fill(ds);
            }
            return ds; 
        }
         *****/

        public DataTable GetDataTable(DataSet ds)
        {
            if (ds != null)
            {
                return ds.Tables[0];
            }
            
            return null;
        }

        public DataRow[] SelectDataRows(DataTable table, string criteria)
        {
            if (table == null)
                return null;

            // criteria is something like:  Field = 'MATURITY'
            DataRow[] result = table.Select(criteria);

            return result;
        }

        public DataTable GetDataTable(string sql)
        { 
            var ds = this.GetDataSet(sql);
            return this.GetDataTable(ds); 
        } 
    }
}
