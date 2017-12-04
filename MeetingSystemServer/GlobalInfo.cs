using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Net.Sockets;
using MSExcel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace MeetingSystemServer
{
    class GlobalInfo
    {
        public static string MeetingTopic = "";//会议主题
        public static string MeetingDepart = "";//组会部门
        public static string MeetingCreater = "";//组会人员
        public static DateTime MeetingCreatTime = DateTime.Now;
        public static string localSpace = "";

        private static OleDbConnection conn = null;//数据库连接

        public static string localIP = "";//IP地址
        public static string localUserName = "";//本地用户名
        public static int Port = 2001;//设置通信端口
        public static int Port1 = 2002;//设置接受回传数据端口
        public static Dictionary<string,string> remoteIPList = new Dictionary<string, string>();//存放远程(终端地址,用户名)

        public static Socket clientSocket = null;

        public static Boolean waterMark = true;//水印
        public static Boolean encrypt = false;//加密
        #region 数据库连接
        /// <summary>
        /// 全局连接
        /// </summary>
        public static OleDbConnection GlobalConnection
        {
            get
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                return conn;
            }
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OleDbConnection getOleDbConnection(string connectionString)
        {
            if (conn == null)
            {
                conn = new OleDbConnection(connectionString);
            }
            else
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                else
                    conn = new OleDbConnection(connectionString);
            }
            return conn;
        }
        #endregion

        /// <summary>
        /// datatable输出excel
        /// </summary>
        /// <param name="dt"></param>
        public static void DataTableToExcel(DataTable dt, string fileName)
        {
            int rowNumer = dt.Rows.Count;
            int columnNumber = dt.Columns.Count;
            int rowIndex = 0;
            int colIndex = 0;

            if (rowNumer == 0)
            {                
                return;
            }

            //创建excel对象
            MSExcel._Application excel = new MSExcel.Application();
            MSExcel.Workbook workbook = excel.Workbooks.Add(MSExcel.XlWBATemplate.xlWBATWorksheet);
            MSExcel.Worksheet worksheet = (MSExcel.Worksheet)workbook.Worksheets[1];
            excel.Visible = true;

            excel.Cells[1, 1] = "序号";
            foreach (DataColumn col in dt.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex + 1] = col.ColumnName;
            }
            //填充数据
            foreach (DataRow dr in dt.Rows)
            {
                rowIndex++;
                colIndex = 1;
                excel.Cells[rowIndex + 1, 1] = rowIndex;
                foreach (DataColumn dc in dt.Columns)
                {
                    colIndex++;
                    excel.Cells[rowIndex + 1, colIndex] = dr[dc.ColumnName];
                }
            }
            //保存
            try
            {
                workbook.SaveAs(fileName);
            }
            catch
            {
                return;
            }

        }
        /// <summary>
        /// datatable输出xml
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void DataTableToXml(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            dt.WriteXml(fs);
            fs.Flush();
            fs.Close();
        }
        /// <summary>
        /// 读取xls文件至datatable
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable xlsToDataTable(string fileName)
        {
            string strConn;
            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=" + fileName + ";" +
                "Extended Properties=Excel 12.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter myCommand = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", strConn);
            DataTable dt = new DataTable();
            try
            {
                myCommand.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }
    }
}
