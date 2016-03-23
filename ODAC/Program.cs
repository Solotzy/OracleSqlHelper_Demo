using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ODAC
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化OracleSqlHelper
            OracleSqlHelper.Initialize();

            //调用静态方法获取服务器时间
            string sDataTimeNow = OracleSqlHelper.GetServerDate().ToLongDateString();
            Console.WriteLine(sDataTimeNow);

            //调用静态方法执行SQL，并传入参数，用DataSet保存返回值
            DataSet ds1 = OracleSqlHelper.UpdateStatus("Student", "10", "2");

            string sMaxID = string.Empty;
            //调用静态方法执行存储过程
            OracleSqlHelper.GetMaxID("Student", ref sMaxID);
            Console.WriteLine(sMaxID);
            
            Console.Read();
        }
    }
}
