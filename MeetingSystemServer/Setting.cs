using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace MeetingSystemServer
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBtn_Click(object sender, EventArgs e)
        {
            int num = 40;
            try
            {
                num = Int32.Parse(clientMaxNum.Text);
                if (num > 254 || num < 1)
                {
                    MessageBox.Show("客户端设置数目范围为1～254！");
                    return;
                }

            }
            catch
            {
                MessageBox.Show("客户端设置数目必须为数字，且范围为1～254");
                return;
            }
            string cfgFilePath = Application.StartupPath + "//config.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(cfgFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件config.xml缺失！请联系管理人员");
                this.Close();
                return;
            }
            XmlNode rootNode = xmlDoc.SelectSingleNode("config");
            foreach (XmlNode xn in rootNode.ChildNodes)
            {
                if (xn.Name == "clientMaxNum")
                {
                    xn.InnerText = clientMaxNum.Text;
                }
                if (xn.Name == "waterMark")
                {
                    xn.InnerText = waterMark.Checked.ToString();
                }
                if (xn.Name == "encrypt")
                {
                    xn.InnerText = encrypt.Checked.ToString();
                }
                if (xn.Name == "defaultTopic")
                {
                    //xn.InnerText= defaultTopic.Text ;
                    xn.RemoveAll();
                    foreach (string str in defaultTopic.Items)
                    {
                        XmlNode cn = xmlDoc.CreateElement("topic");
                        cn.InnerText = str;
                        xn.AppendChild(cn);                        
                    }
                    if (xn.Attributes.Count == 0) //属性被删除了
                    {
                        XmlAttribute xa = xmlDoc.CreateAttribute("default");
                        xn.Attributes.Append(xa);
                    }
                    xn.Attributes["default"].Value = defaultTopic.SelectedIndex.ToString();
                }
                if (xn.Name == "createByDate")
                {
                    xn.InnerText = createByDate.Checked.ToString();
                }
                if (xn.Name == "defaultDepart")
                {
                    xn.RemoveAll();
                    foreach (string str in defaultDepart.Items)
                    {
                        XmlNode cn = xmlDoc.CreateElement("part");
                        cn.InnerText = str;
                        xn.AppendChild(cn);
                    }
                    if (xn.Attributes.Count == 0) //属性被删除了
                    {
                        XmlAttribute xa = xmlDoc.CreateAttribute("default");
                        xn.Attributes.Append(xa);
                    }
                    xn.Attributes["default"].Value = defaultDepart.SelectedIndex.ToString();
                }
                if (xn.Name == "defaultCreater")
                {
                    xn.RemoveAll();
                    foreach (string str in defaultCreater.Items)
                    {
                        XmlNode cn = xmlDoc.CreateElement("creater");
                        cn.InnerText = str;
                        xn.AppendChild(cn);
                    }
                    if (xn.Attributes.Count == 0) //属性被删除了
                    {
                        XmlAttribute xa = xmlDoc.CreateAttribute("default");
                        xn.Attributes.Append(xa);
                    }
                    xn.Attributes["default"].Value = defaultCreater.SelectedIndex.ToString();
                }
                if (xn.Name == "clearLevel")
                {
                    xn.InnerText = (lower.Checked ? 0 : normal.Checked ? 1 : higher.Checked ? 2 : 1).ToString();
                }
                if (xn.Name == "history")
                {
                    xn.FirstChild.InnerText = displayRange.SelectedIndex.ToString();
                    xn.LastChild.InnerText = displayMode.SelectedIndex.ToString();
                }
                if (xn.Name == "backup")
                {
                    xn.InnerText = (XmlRB.Checked ? 0 : 1).ToString();
                }
                if (xn.Name == "mainForm")
                {
                    foreach (XmlNode cxn in xn.ChildNodes)
                    {
                        if (cxn.Name == "maximize")
                        {
                            cxn.InnerText = maximize.Checked.ToString();
                        }
                        if (cxn.Name == "controlBox")
                        {
                            cxn.InnerText = controlBox.Checked.ToString();
                        }
                        if (cxn.Name == "background")
                        {
                            cxn.InnerText = backgroundImg.Text;
                        }
                    }
                }
            }
            try
            {
                xmlDoc.Save(cfgFilePath);
                MessageBox.Show("保存设置成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存设置异常！");
            }
            finally
            {
                this.Close();
            }

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
        /// 读取配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Load(object sender, EventArgs e)
        {
            readConfig();            
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        private void readConfig()
        {
            string cfgFilePath = Application.StartupPath + "//config.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(cfgFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件config.xml缺失！请联系管理人员");
                this.Close();
                return;
            }

            XmlNode rootNode = xmlDoc.SelectSingleNode("config");
            foreach (XmlNode xn in rootNode.ChildNodes)
            {
                if (xn.Name == "clientMaxNum")
                {
                    clientMaxNum.Text = xn.InnerText;
                }
                if (xn.Name == "waterMark")
                {
                    waterMark.Checked = (xn.InnerText == "True");
                }
                if (xn.Name == "encrypt")
                {
                    encrypt.Checked = (xn.InnerText == "True");
                }
                if (xn.Name == "defaultTopic")
                {
                    defaultTopic.Items.Clear();
                    foreach (XmlNode cn in xn.ChildNodes)
                    {
                        defaultTopic.Items.Add(cn.InnerText);
                    }
                    string def = xn.Attributes["default"].Value;
                    defaultTopic.Text = defaultTopic.Items[Int32.Parse(def)].ToString();                    
                }
                if (xn.Name == "createByDate")
                {
                    createByDate.Checked = (xn.InnerText == "True");
                }
                if (xn.Name == "defaultDepart")
                {
                    defaultDepart.Items.Clear();
                    foreach (XmlNode cn in xn.ChildNodes)
                    {
                        defaultDepart.Items.Add(cn.InnerText);
                    }
                    defaultDepart.Text = defaultDepart.Items[Int32.Parse(xn.Attributes["default"].Value)].ToString();
                }
                if (xn.Name == "defaultCreater")
                {
                    defaultCreater.Items.Clear();
                    foreach (XmlNode cn in xn.ChildNodes)
                    {
                        defaultCreater.Items.Add(cn.InnerText);
                    }
                    defaultCreater.Text = defaultCreater.Items[Int32.Parse(xn.Attributes["default"].Value)].ToString();
                }
                if (xn.Name == "clearLevel")
                {
                    switch (xn.InnerText)
                    {
                        case "0":lower.Checked = true;break;
                        case "1":normal.Checked = true;break;
                        case "2":higher.Checked = true;break;
                        default:normal.Checked = true;break;
                    }
                }
                if (xn.Name == "history")
                {
                    displayRange.SelectedIndex = Int32.Parse(xn.FirstChild.InnerText);
                    displayMode.SelectedIndex = Int32.Parse(xn.LastChild.InnerText);
                }
                if (xn.Name == "backup")
                {
                    XmlRB.Checked = xn.InnerText == "0";
                    XlsBR.Checked = xn.InnerText == "1";
                }
                if (xn.Name == "mainForm")
                {
                    foreach (XmlNode cxn in xn.ChildNodes)
                    {
                        if (cxn.Name == "maximize")
                        {
                            maximize.Checked = cxn.InnerText == "True";
                        }
                        if (cxn.Name == "controlBox")
                        {
                            controlBox.Checked = cxn.InnerText == "True";
                        }
                        if (cxn.Name == "background")
                        {
                            backgroundImg.Text = cxn.InnerText;
                        }
                    }
                }
            }
        }        
        /// <summary>
        /// 为集合调用编辑窗口
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        private List<string> editFormInvoke(ComboBox cb)
        {
            List<string> list = new List<string>();
            foreach (string str in cb.Items)
            {
                list.Add(str);
            }
            //启动编辑界面
            EditForm ef = new EditForm(list);
            ef.ShowDialog();
            return ef.Value;
        }
        /// <summary>
        /// 编辑默认的会议主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBtn1_Click(object sender, EventArgs e)
        {
            //先记载改变前的值
            string valueBefore = defaultTopic.Text;
            string valueAfter = "";
            List<string> list = editFormInvoke(defaultTopic);
            defaultTopic.Items.Clear();
            foreach (string str in list)
            {
                defaultTopic.Items.Add(str);
            }
            if (list.Contains(valueBefore))
            {
                valueAfter = valueBefore;
            }
            else if(list.Count>0)
            {
                valueAfter = list[0];
            }
            defaultTopic.Text = valueAfter;
        }
        /// <summary>
        /// 编辑默认的办会部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBtn2_Click(object sender, EventArgs e)
        {
            string valueBefore = defaultDepart.Text;
            string valueAfter = "";
            List<string> list = editFormInvoke(defaultDepart);
            defaultDepart.Items.Clear();
            foreach (string str in list)
            {
                defaultDepart.Items.Add(str);
            }
            if (list.Contains(valueBefore))
            {
                valueAfter = valueBefore;
            }
            else if (list.Count > 0)
            {
                valueAfter = list[0];
            }
            defaultDepart.Text = valueAfter;
        }
        /// <summary>
        /// 编辑默认的组会人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBtn3_Click(object sender, EventArgs e)
        {
            string valueBefore = defaultCreater.Text;
            string valueAfter = "";
            List<string> list = editFormInvoke(defaultCreater);
            defaultCreater.Items.Clear();
            foreach (string str in list)
            {
                defaultCreater.Items.Add(str);
            }
            if (list.Contains(valueBefore))
            {
                valueAfter = valueBefore;
            }
            else if (list.Count > 0)
            {
                valueAfter = list[0];
            }
            defaultCreater.Text = valueAfter;
        }
        /// <summary>
        /// 浏览图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "(图片)|*.bmp|(图片)|*.jpg|(图片)|*.png";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                backgroundImg.Text = ofd.FileName;
            }
        }
    }
}
