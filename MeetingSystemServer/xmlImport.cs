using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Xml;

namespace MeetingSystemServer
{
    class xmlImport:historyImportInterface
    {
        public int import(string sourceName)
        {
            //Console.WriteLine("xml导入！");
            if (!File.Exists(sourceName))
            {
                return -1; //文件不存在
            }
            //判断XML文件格式是否正确
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(sourceName);
            XmlNode rootNode=xmlDoc.SelectSingleNode("DocumentElement");
            if (rootNode == null)
            {
                return -2;//文件格式不正确
            }
            XmlNodeList nodeList = rootNode.ChildNodes;
            Console.WriteLine(nodeList.Count);
            if (nodeList == null || nodeList.Count == 0)
                return -2;
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            string sql = "";
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            foreach (XmlNode xn in nodeList)
            {
                if (xn.Name != "meetinghistory")
                {
                    ocmd.Dispose();
                    oc.Close();
                    return -2;//文件格式不正确
                }
                //获取xn的节点个数
                if (xn.ChildNodes.Count<4|| xn.ChildNodes.Count > 5)
                {
                    ocmd.Dispose();
                    oc.Close();
                    return -2;
                }
                sql = "select count(*) from meetingtable where uuid='" + xn.SelectSingleNode("标识").InnerText+"'";
                ocmd.CommandText = sql;
                if (Int32.Parse(ocmd.ExecuteScalar().ToString()) != 0) //存在
                {
                    continue;
                }
                else
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

                    ocmd.Parameters["topic"].Value = xn.SelectSingleNode("会议主题").InnerText;
                    ocmd.Parameters["department"].Value = xn.SelectSingleNode("办会部门").InnerText;
                    ocmd.Parameters["creater"].Value = xn.SelectSingleNode("办会人").InnerText;
                    ocmd.Parameters["createtime"].Value = Convert.ToDateTime(xn.SelectSingleNode("会议开始时间").InnerText);
                    ocmd.Parameters["uuid"].Value = xn.SelectSingleNode("标识").InnerText;

                    if (xn.SelectSingleNode("会议结束时间") != null)
                    {
                        ocmd.Parameters["endtime"].Value = Convert.ToDateTime(xn.SelectSingleNode("会议结束时间").InnerText);
                    }
                    else
                    {
                        ocmd.Parameters["endtime"].Value = DBNull.Value;
                    }
                    try
                    {
                        ocmd.ExecuteNonQuery();

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("导入失败！"+ex.Message);
                        ocmd.Dispose();
                        oc.Close();
                        return -3; //导入失败！
                    }
                }
            }
            ocmd.Dispose();
            oc.Close();
            return 0;
        }
    }
}
