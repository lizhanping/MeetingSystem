using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeetingSystemServer
{
    public partial class selectForm : Form
    {
        public selectForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 时间不限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                label1.Visible = false;
            }
        }
        /// <summary>
        /// 时间区间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;
                label1.Visible = true;
            }
        }
        /// <summary>
        /// 主题不限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                topicKey.Enabled = false;
            }
        }
        /// <summary>
        /// 主题关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                topicKey.Enabled = true;
            }
        }
        /// <summary>
        /// 部门不限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                depart.Enabled = false;
            }
        }
        /// <summary>
        /// 部门关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                depart.Enabled = true;
            }
        }
        /// <summary>
        /// 办会人员不限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                creater.Enabled = false;
            }
        }
        /// <summary>
        /// 办会人员关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                creater.Enabled = true;
            }
        }
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBtn_Click(object sender, EventArgs e)
        {
            OleDbConnection oc = GlobalInfo.GlobalConnection;
            oc.Open();
            //根据条件进行添加
            string createTimeStr = "";
            bool createTimeFlag = false;
            string topicStr = "";
            string departStr = "";
            string createrStr = "";
            if (radioButton2.Checked)
            {
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                {
                    MessageBox.Show("时间配置错误，结束时间不能小于开始时间！");
                    return;
                }
                createTimeStr = " and createtime>@time1 and createtime<@time2 ";
                createTimeFlag = true;
            }
            if (radioButton4.Checked)
            {
                if (topicKey.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("请输入关键字！");
                    return;
                }
                string[] tmp = topicKey.Text.Trim().Split('；');//使用中文分号
                string list = "";
                foreach (string str in tmp)
                {
                    list = list + "topic like '%" + str + "%'" + " or ";
                }
                list = "(" + list.Substring(0,list.Length-3) + ")";
                topicStr = " and "+list;
            }
            if (radioButton6.Checked)
            {
                if (depart.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("请输入关键字！");
                    return;
                }
                string[] tmp = depart.Text.Trim().Split('；');
                string list = "";
                foreach (string str in tmp)
                {
                    list = list + "department like '%" + str + "%'" + " or ";
                }
                list = "(" + list.Substring(0, list.Length - 3) + ")";
                departStr = " and " + list;
            }
            if (radioButton8.Checked)
            {
                if (creater.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("请输入关键字！");
                    return;
                }
                string[] tmp = creater.Text.Trim().Split('；');
                string list = "";
                foreach (string str in tmp)
                {
                    list = list + "creater like '%" + str + "%'" + " or ";
                }
                list = "(" + list.Substring(0, list.Length - 3) + ")";
                createrStr = " and " + list;
            }

            string sql = "select topic as 会议主题,department as 办会部门,creater as 办会人, createtime as 会议开始时间, endtime as 会议结束时间 from meetingtable where 1=1 "+ createTimeStr + topicStr+departStr+createrStr;
            OleDbCommand ocmd = new OleDbCommand(sql, oc);
            if (createTimeFlag)
            {
                ocmd.Parameters.Add("time1", OleDbType.Date);
                ocmd.Parameters.Add("time2",OleDbType.Date);
                ocmd.Parameters["time1"].Value = dateTimePicker1.Value;
                ocmd.Parameters["time2"].Value = dateTimePicker2.Value;
            }
            OleDbDataAdapter oda = new OleDbDataAdapter(ocmd);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("未查到有效数据！");
                return;
            }
            GlobalInfo.DataTableToExcel(dt, "所有会议记录");
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancleBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
