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

        public void test1() {

            SqlConnection conn = new SqlConnection();

            ConnectionManager.Init();
            conn.ConnectionString = ConnectionManager.GetConnString("SFCSQLSERVERDB");
            conn.Open();
            SqlCommand cmd = new SqlCommand("",conn);
            cmd.Parameters.Add("@param1",SqlDbType.VarChar);
            cmd.Parameters["param1"].Value = "";
            cmd.Parameters["param1"].Direction = ParameterDirection.Input;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) {

            }

            dr.Close();
            conn.Close();
        }
    }
}
