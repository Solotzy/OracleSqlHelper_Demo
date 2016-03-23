using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using System.Data;

namespace ODAC
{
    class OracleSqlHelper
    {
        private static string g_sConnection = string.Empty;

        public static void Initialize()
        {
            string sDBIP = "127.0.0.1";   //你的数据库地址
            string sDBName = "OneDb";       //你的数据库名称
            string sUserName = "username";   //你的数据库登录名
            string sPassword = "password";    //你的数据库密码
            g_sConnection = string.Format("User Id={0};Password={1};Server={2};Direct=True;Sid={3}"
                , sUserName, sPassword, sDBIP, sDBName);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sSQL">SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>查询到的数据DataSet</returns>
        public static DataSet ExecuteSQL(string sSQL, object[][] Params)
        {
            try
            {
                DataSet ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(sSQL, g_sConnection);
                if (Params != null)
                {
                    for (int i = 0; i <= Params.Length - 1; i++)
                    {
                        OracleParameter oraclePar = new OracleParameter();
                        oraclePar.Direction = ParameterDirection.Input;
                        try
                        {
                            oraclePar.OracleDbType = (OracleDbType)Params[i][1];
                        }
                        catch
                        {
                            switch (Params[i][1].ToString())
                            {
                                case "1": oraclePar.OracleDbType = OracleDbType.VarChar; break;
                                case "2": oraclePar.OracleDbType = OracleDbType.Integer; break;
                                case "3": oraclePar.OracleDbType = OracleDbType.Date; break;
                                default: oraclePar.OracleDbType = OracleDbType.VarChar; break;
                            }
                        }
                        oraclePar.ParameterName = Params[i][2].ToString().ToUpper();
                        oraclePar.Value = Params[i][3];
                        adapter.SelectCommand.Parameters.Add(oraclePar);
                    }
                }
                try
                {
                    adapter.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                    return null;
                }
                finally
                {
                    ds.Dispose();
                    adapter.Dispose();
                }
            }
            finally
            {

            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="sProcName">存储过程名称</param>
        /// <param name="Params">参数列表</param>
        /// <returns>返回的数据</returns>
        public static DataSet ExecuteProc(string sProcName, object[][] Params)
        {
            try
            {
                OracleConnection OraConnection = new OracleConnection(g_sConnection);
                OracleCommand oraCmd = new OracleCommand("", OraConnection);
                oraCmd.CommandText = sProcName;

                oraCmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                DataTable dsTable = new DataTable();
                DataRow dr = dsTable.NewRow();
                OracleDataAdapter adapter = null;
                for (int i = 0; i <= Params.Length - 1; i++)
                {
                    OracleParameter oraclePar = new OracleParameter();
                    try
                    {
                        oraclePar.Direction = (ParameterDirection)Params[i][0];
                    }
                    catch
                    {
                        switch (Params[i][0].ToString().ToUpper())
                        {
                            case "INPUT": oraclePar.Direction = ParameterDirection.Input; break;
                            case "OUTPUT": oraclePar.Direction = ParameterDirection.Output; break;
                            default: oraclePar.Direction = ParameterDirection.Input; break;
                        }
                    }

                    try
                    {
                        oraclePar.OracleDbType = (OracleDbType)Params[i][1];
                    }
                    catch
                    {
                        switch (Params[i][1].ToString())
                        {
                            case "1": oraclePar.OracleDbType = OracleDbType.VarChar; break;
                            case "2": oraclePar.OracleDbType = OracleDbType.Integer; break;
                            case "3": oraclePar.OracleDbType = OracleDbType.Date; break;
                            default: oraclePar.OracleDbType = OracleDbType.VarChar; break;
                        }
                    }
                    oraclePar.ParameterName = Params[i][2].ToString().ToUpper();
                    oraclePar.Size = Params[i][3].ToString().Length;
                    if (oraclePar.Direction == ParameterDirection.Output)
                    {
                        oraclePar.Size = 1000;
                    }
                    oraclePar.Value = Params[i][3].ToString();
                    oraCmd.Parameters.Add(oraclePar);
                }

                adapter = new OracleDataAdapter(oraCmd);
                try
                {
                    adapter.Fill(ds);
                    for (int i = 0; i <= adapter.SelectCommand.Parameters.Count - 1; i++)
                    {
                        if (adapter.SelectCommand.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            dsTable.Columns.Add(adapter.SelectCommand.Parameters[i].ParameterName, typeof(string));
                            dr[adapter.SelectCommand.Parameters[i].ParameterName] = adapter.SelectCommand.Parameters[i].Value;
                        }

                    }
                    dsTable.Rows.Add(dr);
                    ds.Tables.Clear();
                    ds.Tables.Add(dsTable);
                    return ds;
                }
                finally
                {
                    OraConnection.Dispose();
                    oraCmd.Dispose();
                    ds.Dispose();
                    dsTable.Dispose();
                    adapter.Dispose();
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 示例1 ：执行SQL语句 使用ExecuteSQL 附加参数列表Params为空
        /// 获取服务器时间，
        /// </summary>
        /// <returns>返回服务器时间</returns>
        public static DateTime GetServerDate()
        {
            DateTime dtNow;
            string sSQL = @" SELECT SYSDATE FROM DUAL ";
            dtNow = (DateTime)(ExecuteSQL(sSQL, null).Tables[0].Rows[0][0]); //
            return dtNow;
        }

        /// <summary>
        /// 示例2： 执行SQL语句 使用ExecuteSQL 附加参数列表Params不为空
        /// 更新数据 
        /// </summary>
        /// <param name="sTable"></param>
        /// <param name="sRackLoc"></param>
        /// <param name="sStatus"></param>
        /// <returns></returns>
        public static DataSet UpdateStatus(string sTable, string sLocation, string sStatus)
        {
            object[][] Params = new object[3][];
            string sSQL = string.Format(@" UPDATE {0} 
                                     SET STATUS = :STATUS
                                       WHERE ID = :ID 
                                        AND LOC = :LOC ", sTable);
            Params[0] = new object[] { ParameterDirection.Input, OracleDbType.VarChar, "RACK_STATUS", sStatus };
            Params[1] = new object[] { ParameterDirection.Input, OracleDbType.VarChar, "PDLINE_ID", "1" };
            Params[2] = new object[] { ParameterDirection.Input, OracleDbType.VarChar, "CRANE_LOC", sLocation };
            return ExecuteSQL(sSQL, Params);
        }

        /// <summary>
        /// 示例3： 调用存储过程，使用ExecuteProc 
        /// 获取最大ID
        /// </summary>
        /// <param name="sTable">表名</param>
        /// <param name="MaxID">最大ID</param>
        /// <returns></returns>
        public static void GetMaxID(string sTable, ref string MaxID)
        {
            try
            {
                object[][] Params = new object[2][];
                Params[0] = new object[] { ParameterDirection.Input, OracleDbType.VarChar, "TTABLE", sTable }; //输入
                Params[1] = new object[] { ParameterDirection.Output, OracleDbType.VarChar, "T_MAXID", "" };   //输出
                DataSet dsTemp = ExecuteProc("PROC_GET_MAXID", Params);

                string sRes = dsTemp.Tables[0].Rows[0]["TRES"].ToString();
                dsTemp.Dispose();

                if (sRes != "OK")
                {
                    MaxID = "0";
                }
                else
                    MaxID = dsTemp.Tables[0].Rows[0]["T_MAXID"].ToString();
                dsTemp.Dispose();

            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
