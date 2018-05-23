using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            testOracle();
            testSqlServer();

            System.Console.Out.WriteLine("END");
        }

        public static  void testOracle() {
            OracleDBPoolUtil c1 = new OracleDBPoolUtil();
            OleExec SFCDB = c1.DBPools["SFCDB"].Borrow();
            c1.testQueryTable(SFCDB);
            c1.testProcedure(SFCDB);
            c1.DBPools["SFCDB"].Return(SFCDB);

        }
        public static void testSqlServer() {
            SqlServerExec exec = new SqlServerExec();
            exec.test();
        }



    }
}
