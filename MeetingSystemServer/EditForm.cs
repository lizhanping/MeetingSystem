using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeetingSystemServer
{
    public partial class EditForm : Form
    {
        private List<string> list = null;
        public EditForm(List<string> list)
        {
            InitializeComponent();
            this.list = list;
        }
        /// <summary>
        /// 返回值
        /// </summary>
        public List<string> Value
        {
            get { return list; }
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
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBtn_Click(object sender, EventArgs e)
        {
            //解析textbox值,写入list
            list.Clear();
            foreach (string line in items.Lines)
            {
               if(line.Trim()!="")
                   list.Add(line);
            }
            this.Close();        
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditForm_Load(object sender, EventArgs e)
        {
            foreach (string line in list)
            {
                items.AppendText(line + "\r\n");
            }
        }
    }
}
