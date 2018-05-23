using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace MESDBHelper
{
    public class OleExec
    {
        string _connStr;
        OleDbConnection _conn;
        OleDbCommand _comm;
        OleDbDataAdapter _adp;
        OleDbTransaction _Train;
        public bool ThrowSqlExeception = false;
        public int CMD_TIME_OUT = 30;
        OleExecPool Pool;
        public OleExec()
        {
        }
        /// <summary>
        /// 構造一個OleExec對象,該對象用於執行SQL語句等任務
        /// </summary>
        /// <param name="strConn"></param>
        public OleExec(string strConn , OleExecPool _Pool)
        {
            _connStr = strConn;
            _conn = new OleDbConnection(_connStr);
            _conn.Open();
            _comm = new OleDbCommand();
            _adp = new OleDbDataAdapter(_comm);
            Pool = _Pool;
        }

        /// <summary>
        /// DB連接對象
        /// </summary>
        /// <param name="DBConntionConfigName">連接配置名稱</param>
        /// <param name="ReadXMLConfig">是否從XML配置檔讀取連接字符串</param>
        public OleExec(string DBConntionConfigName, bool ReadXMLConfig)
        {
            if (Pool!=null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                _connStr = ConnectionManager.GetConnString(DBConntionConfigName);
            }
            else
            {
                _connStr = ConfigurationManager.AppSettings[DBConntionConfigName];
            }
            _conn = new OleDbConnection(_connStr);
            _conn.Open();
            _comm = new OleDbCommand();
            _adp = new OleDbDataAdapter(_comm);
        }

        /// <summary>
        /// 執行SQL語句
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public string ExecSQL(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = strSQL;
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            try
            {
                _comm.Transaction = _Train;
                int ret = _comm.ExecuteNonQuery();
                return ret.ToString();
            }
            catch (Exception e)
            {
                if (ThrowSqlExeception)
                {
                    throw e;
                }
                return "執行SQL異常\r\nstrSQL:\"" + strSQL + "\"\r\n異常信息:" + e.Message;
            }
        }

        /// <summary>
        /// 執行一條插入語句未調試,請不要使用該方法
        /// 請注意傳入的語句只插入1條記錄的
        /// 只有SQLServer數據庫支持該方法
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>自動ID</returns>
        public object ExecInsertSQL(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            string strSelectID = " select @@identity as ID";
            strSQL += strSelectID;
            _comm.CommandText = strSQL;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            object res = Int32.Parse(_comm.ExecuteScalar().ToString());
            return res;
        }

        public object ExecSelectOneValue(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            _comm.CommandText = strSQL;
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            return _comm.ExecuteScalar();
        }

        /// <summary>
        /// 执行Oracle语句，并返回第一行第一列结果,wuq/by20140626
        /// </summary>
        /// <param name="strSql">Oracle语句</param>
        /// <returns></returns>
        public string ExecSqlReturn(string strSql)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            string strReturn = "";
            checkConn();
            _comm.CommandText = strSql;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            strReturn = _comm.ExecuteScalar().ToString();
            return strReturn;
        }

        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        //public DataSet ExecSelect(string strSQL)
        //{
        //    checkConn();
        //    try
        //    {
        //        return _RunSelect(strSQL);
        //    }
        //    catch
        //    {
        //        checkConn();
        //        return _RunSelect(strSQL);
        //    }
        //}
        public DataSet RunSelect(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            _comm.CommandText = strSQL;
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _adp.SelectCommand = _comm;
            _comm.Parameters.Clear();
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;
        }
        public DataTable ExecSelect_b(int rowcount, int pageindex, string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            try
            {
                return _RunSelect_b(rowcount, pageindex, strSQL);
            }
            catch
            {
                checkConn();
                return _RunSelect_b(rowcount, pageindex, strSQL);
            }
        }
        private DataTable _RunSelect_b(int rowcount, int pageindex, string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            int sindex = rowcount * pageindex - rowcount;
            //int eindex = rowcount * pageindex-1;
            _comm.CommandText = strSQL;
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _adp.SelectCommand = _comm;
            DataTable dt = new DataTable();
            _adp.Fill(sindex, rowcount, dt);

            return dt;
            //_RetReader["ID"].ToString();
            //_RetReader.NextResult();

        }
        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSql">SQL語句</param>
        /// <param name="Paras">參數數組,使用 params 聲明表示該參數可以省略</param>
        /// <returns></returns>
        public DataSet ExecSelect(string strSql,params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length>0)
            {
                foreach (OleDbParameter para in Paras)
                {
                    _comm.Parameters.Add(para);
                }
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public DataSet ExecProcedure(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = SPName;
            _comm.CommandType = CommandType.StoredProcedure;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.Parameters.Clear();
            _comm.CommandTimeout = CMD_TIME_OUT;
            if (Paras != null && Paras.Length > 0)
            {
                foreach (OleDbParameter para in Paras)
                {
                    _comm.Parameters.Add(para);
                }
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;
        }
        /// <summary>
        /// 執行SQL過程,返回影響行數
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="Paras"></param>
        public int ExecSqlNoReturn(string strSql, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = CommandType.Text;
            _comm.Transaction = _Train;
            _comm.Connection = _conn;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length > 0)
            {
                foreach (OleDbParameter para in Paras)
                {
                    _comm.Parameters.Add(para);
                }
            }
            return _comm.ExecuteNonQuery();
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public string ExecProcedureNoReturn(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = SPName;
            _comm.CommandType = CommandType.StoredProcedure;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.Parameters.Clear();
            _comm.CommandTimeout = CMD_TIME_OUT;
            if (Paras != null && Paras.Length > 0)
            { 
                foreach (OleDbParameter para in Paras)
                {
                  _comm.Parameters.Add(para);
                }
            }
            _comm.ExecuteNonQuery();

            string result = "";
            for (int i = 0; i < _comm.Parameters.Count; i++)
            {
                if (_comm.Parameters[i].Direction == ParameterDirection.Output)
                {
                    result = _comm.Parameters[i].Value.ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public Dictionary<string, object> ExecProcedureReturnDic(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = SPName;
            _comm.CommandType = CommandType.StoredProcedure;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.Parameters.Clear();
            _comm.CommandTimeout = CMD_TIME_OUT;
            if (Paras != null && Paras.Length > 0)
            {
                foreach (OleDbParameter para in Paras)
                {
                    _comm.Parameters.Add(para);
                }
            }
            _comm.ExecuteNonQuery();

            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < _comm.Parameters.Count; i++)
            {
                if (_comm.Parameters[i].Direction == ParameterDirection.Output)
                {
                    result.Add(_comm.Parameters[i].ParameterName, _comm.Parameters[i].Value.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 执行SQL语句或者存儲過程，并返回第一行第一列结果
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>string</returns>
        public string ExecuteScalar(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            string strReturn = "";
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = cmmType;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length > 0)
            {
                for (int i = 0; i < Paras.Length; i++)
                {
                    if (Paras[i] != null)
                    {
                        _comm.Parameters.Add(Paras[i]);
                    }
                }
            }
            strReturn = _comm.ExecuteScalar().ToString();
            return strReturn;
        }

        /// <summary>
        /// 執行SQL語句或存儲過程，返回受影響行數
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>int</returns>
        public int ExecuteNonQuery(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            int intReturn = 0;
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = cmmType;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length > 0)
            {
                for (int i = 0; i < Paras.Length; i++)
                {
                    if (Paras[i] != null)
                    {
                        _comm.Parameters.Add(Paras[i]);
                    }
                }
            }
            intReturn = _comm.ExecuteNonQuery();
            return intReturn;
        }

        /// <summary>
        /// 執行SQL語句或者存儲過程，返回DataTable
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = cmmType;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length > 0)
            {
                for (int i = 0; i < Paras.Length; i++)
                {
                    if (Paras[i] != null)
                    {
                        _comm.Parameters.Add(Paras[i]);
                    }
                }
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet dataset = new DataSet();
            _adp.Fill(dataset);
            return dataset.Tables[0];
        }

        /// <summary>
        /// 執行SQL語句或者存儲過程，返回DataSet
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            _comm.CommandText = strSql;
            _comm.CommandType = cmmType;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _comm.Parameters.Clear();
            if (Paras != null && Paras.Length > 0)
            {
                for (int i = 0; i < Paras.Length; i++)
                {
                    if (Paras[i] != null)
                    {
                        _comm.Parameters.Add(Paras[i]);
                    }
                }
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet dataset = new DataSet();
            _adp.Fill(dataset);
            return dataset;
        }

        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_Train == null)
            {
                _Train = this._conn.BeginTransaction();
                _comm.Transaction = _Train;
            }
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_Train != null)
            {
                _Train.Commit();
                _Train = null;
                _comm.Transaction = null;
            }
        }
        /// <summary>
        /// 事務回滾
        /// </summary>
        public void RollbackTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_Train != null)
            {
                _Train.Rollback();
                _Train = null;
                _comm.Transaction = null;
            }
        }
        /// <summary>
        /// 用於檢查連接是否正常.
        /// </summary>
        private void checkConn()
        {
            if (_Train == null)
            {
                _conn.ResetState();
                if (_conn.State != ConnectionState.Open)
                {
                    try
                    {
                        _conn.Close();
                    }
                    catch
                    { }
                    _conn = new OleDbConnection(this._connStr);
                    _conn.Open();
                }
            }
        }
        ~OleExec()
        {
            try
            {
                _conn.Close();
            }
            catch
            { }
        }
        public void FreeMe()
        {
            try
            {
                _Train.Rollback();
            }
            catch
            { }
            try
            {
                this._comm.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Dispose();
            }
            catch
            { }
            try
            {
                this._adp.Dispose();
            }
            catch
            { }
        }

        /// <summary>
        /// test
        /// </summary>
        public void CloseMe()
        {
            try
            {
                _Train.Rollback();
            }
            catch
            { }
            try
            {
                this._comm.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Close();
            }
            catch
            { }
            try
            {
                this._adp.Dispose();
            }
            catch
            { }
        }
    }
}
