using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestConsole;

namespace MESDBHelper
{
    public class OracleDBPoolUtil
    {
        public Dictionary<string, OleExecPool> DBPools;

        public OracleDBPoolUtil() {
            DBPools = new Dictionary<string, OleExecPool>();
            ConnectionManager.Init();
            String sfcConnectString = ConnectionManager.GetConnString("SFCORACLEDB");
            OleExecPool sfcPool = new OleExecPool(sfcConnectString);
            DBPools.Add("SFCDB", sfcPool);
     

        }

        public void testProcedure(OleExec SFCDB)
        {
            OleDbParameter[] para = new OleDbParameter[]
           {
                new OleDbParameter(":IN_BU",OleDbType.VarChar,300),
                new OleDbParameter(":IN_TYPE",OleDbType.VarChar,300),
                new OleDbParameter(":OUT_RES",OleDbType.VarChar,500)
           };
            para[0].Value = "HWD";
            para[1].Value = "C_SKU";
            para[2].Direction = ParameterDirection.Output;
            SFCDB.ExecProcedureNoReturn("SFC.GET_ID", para);
            System.Console.Out.WriteLine(para[2].Value);
        }
        public void testQueryTable(OleExec SFCDB)
        {

            String skuno = "03054639";

            String sql = $@"select bu,skuno from C_SKU WHERE SKUNO ='{skuno.ToUpper()}'";

            DataTable table = SFCDB.ExecSelect(sql).Tables[0];


            List<SKU> skus = new List<SKU>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                // table.Load(sku);
                SKU sku = new SKU();
                sku.Bu = table.Rows[i][0].ToString();
                sku.Skuno = table.Rows[i][1].ToString();
                System.Console.Out.WriteLine(table.Rows[i][0].ToString());
                System.Console.Out.WriteLine(table.Rows[i][1].ToString());
                skus.Add(sku);

            }
            System.Console.Out.WriteLine("bu:" + skus[0].Bu + ",skuno:" + skus[0].Skuno);

        }


    }

    
}
