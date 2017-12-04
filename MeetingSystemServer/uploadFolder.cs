using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MeetingSystemServer
{
    class uploadFolder:uploadInterface
    {
        /// <summary>
        /// 源目录
        /// </summary>
        private string srcFolder;
        /// <summary>
        /// 目标目录
        /// </summary>
        private string desFolder;
        /// <summary>
        /// 标签
        /// </summary>
        private int tag = 0;//文件夹
        /// <summary>
        /// 设置源
        /// </summary>
        /// <param name="path"></param>
        public void setSrc(string path)
        {
            srcFolder = path;
        }
        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="path"></param>
        public void setDes(string path)
        {
            desFolder = path;
        }
        public int getTag()
        {
            return tag;
        }
       /// <summary>
       /// 获取文件夹名
       /// </summary>
        public string getFileName()
        {
          return Path.GetFileName(srcFolder);
        }
      /// <summary>
      /// 上传
      /// </summary>
        public int upload()
        {
           return folderCopy(srcFolder, desFolder);
        }
        /// <summary>
        /// 文件夹拷贝
        /// </summary>
        /// <param name="sDir">欲拷贝文件夹</param>
        /// <param name="dDir">拷贝目录</param>
        private int folderCopy(string sDir,string dDir)
        {
            if (!Directory.Exists(sDir))
            {
                MessageBox.Show("文件夹:" + sDir + "不存在！");
                return -1;
            }
            if (!Directory.Exists(dDir))
            {
                MessageBox.Show("文件夹:" + dDir + "不存在！");
                return -1;
            }
            //检查dDir下是否存在sDir的名字，存在提示，不存在，则新建
            string sFoldName = Path.GetFileName(sDir);
            string dFoldName = Path.Combine(dDir, sFoldName);
            if (Directory.Exists(dFoldName))
            {
                if (MessageBox.Show("已存在相同文件夹，是否覆盖？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    MessageBox.Show("已结束上传操作！");
                    return -1;
                }
                else
                {
                    try
                    {
                        Directory.Delete(dFoldName, true);
                        Directory.CreateDirectory(dFoldName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + "创建目录失败！");
                        return -1;
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(dFoldName);
            }
            //遍历源路径下所有文件，进行复制
            string[] fileList = Directory.GetFileSystemEntries(sDir);
            try
            {
                foreach (string str in fileList)
                {
                    if (File.Exists(str))//文件
                    {
                        try
                        {
                            File.Copy(str, Path.Combine(dFoldName, Path.GetFileName(str)));
                        }
                        catch
                        {
                            return -1;
                        }
                        
                    }
                    else//文件夹
                    {
                      folderCopy(str, dFoldName);
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "子目录拷贝失败！");
                return -1;
            }
        }
    }
}
