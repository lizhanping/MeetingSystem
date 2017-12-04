using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace MeetingSystemServer
{
    class xlsImport:historyImportInterface
    {
        public int import(string sourceName)
        {
            if (!File.Exists(sourceName))
            {
                return -1;//文件不存在
            }
            DataTable dt = GlobalInfo.xlsToDataTable(sourceName);//将xls转化成dt
            Console.WriteLine(dt.Rows.Count+"//"+dt.Columns.Count);

            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "";
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            foreach (DataRow dr in dt.Rows)
            {
                sql = "select count(*) from meetingtable where uuid='" + dr["标识"].ToString() + "'";
                ocmd.CommandText = sql;
                if (Int32.Parse(ocmd.ExecuteScalar().ToString()) != 0) //存在
                {
                    continue;
                }
                else
                {
                    try
                    {
                        sql = "insert into meetingtable(topic,department,creater,createtime,endtime,uuid) values(@topic,@deparment,@creater,@createtime,@endtime,@uuid)";
                        ocmd.CommandText = sql;
                        ocmd.Parameters.Clear();
                        ocmd.Parameters.Add("topic", OleDbType.Char);
                        ocmd.Parameters.Add("department", OleDbType.Char);
                        ocmd.Parameters.Add("creater", OleDbType.Char);
                        ocmd.Parameters.Add("createtime", OleDbType.Date);
                        ocmd.Parameters.Add("endtime", OleDbType.Date);
                        ocmd.Parameters.Add("uuid", OleDbType.Char);

                        ocmd.Parameters["topic"].Value = dr["会议主题"].ToString();
                        ocmd.Parameters["department"].Value = dr["办会部门"].ToString();
                        ocmd.Parameters["creater"].Value = dr["办会人"].ToString();
                        ocmd.Parameters["createtime"].Value = Convert.ToDateTime(dr["会议开始时间"].ToString());
                        ocmd.Parameters["uuid"].Value = dr["标识"].ToString();
                    }
                    catch
                    {
                        ocmd.Dispose();
                        oc.Close();
                        return -2;
                    }
                    if (dr["会议结束时间"].ToString() != "")
                    {
                        ocmd.Parameters["endtime"].Value = Convert.ToDateTime(dr["会议结束时间"].ToString());
                    }
                    else
                    {
                        ocmd.Parameters["endtime"].Value = DBNull.Value;
                    }
                    try
                    {
                        ocmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("导入失败！" + ex.Message);
                        ocmd.Dispose();
                        oc.Close();
                        return -3; //导入失败！
                    }
                }
            }
            return 0;
        }

    }
}
