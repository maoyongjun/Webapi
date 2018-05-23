using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDBHelper
{
    public class SqlServerExec
    {
        public SqlServerExec() {

        }
        public void test() {
            SqlConnection conn = new SqlConnection();
      
            ConnectionManager.Init();
            conn.ConnectionString = ConnectionManager.GetConnString("SFCSQLSERVERDB");
            conn.Open();
            String location = "TJ1725002250";
            String sql = $@"SELECT * FROM mfsysproduct WHERE location ='{location.ToUpper()}'";
            SqlDataAdapter myda = new SqlDataAdapter(sql,conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);

            foreach (DataRow row in dt.Rows) {
                System.Console.Out.WriteLine(row[0]);
            }
           
        }
    }
}
