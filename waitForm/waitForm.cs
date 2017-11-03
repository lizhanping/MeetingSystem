using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace waitForm
{
    public partial class waitForm : Form
    {
        private Thread monitThread = null;
        private object para = null;
        public waitForm()
        {
            InitializeComponent();
            Console.WriteLine("test1111111111111");
        }

        /// <summary>
        /// 设置显示文本
        /// </summary>
        /// <param name="text"></param>
        public void setText(string text)
        {
            info.Text = text;          
        }
        /// <summary>
        /// 设置监控
        /// </summary>
        public void setMonit(Thread monitThread)
        {
            this.monitThread = monitThread;
        }
        /// <summary>
        /// 启动监控
        /// </summary>
        public void setMonit(Thread monitThread, object para)
        {
            this.monitThread = monitThread;
            this.para = para;
        }
        /// <summary>
        /// 加载初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waitForm_Load(object sender, EventArgs e)
        {
            Thread proThread = new Thread(new ThreadStart(process));
            proThread.Start();
        }
        /// <summary>
        /// 处理
        /// </summary>
        private void process()
        {
            if (para != null)
                monitThread.Start(para);
            else
                monitThread.Start();
            Console.WriteLine("test22222222222");
            bool flag = true;
            while (flag)
            {
                if (!monitThread.IsAlive)
                {
                    flag = false;
                }
            }
            Thread.Sleep(1000);
            Console.WriteLine("test3333333333");
            //Application.Exit();
            waitFormClose();
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void waitFormClose()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object>(x=> {
                    this.Close();
                }),0);
            }
        }
    }
}
