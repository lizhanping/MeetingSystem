using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MeetingSystemServer
{
    class uploadFile:uploadInterface
    {
        /// <summary>
        /// 文件名
        /// </summary>
        private string filePath;
        /// <summary>
        /// 文件夹名
        /// </summary>
        private string folderPath;
        /// <summary>
        /// 标签
        /// </summary>
        private int tag=1;
        /// <summary>
        /// 设置源
        /// </summary>
        /// <param name="path"></param>
        public void setSrc(string path)
        {
            filePath = path;
        }
        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="path"></param>
        public void setDes(string path)
        {
            folderPath = path;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <returns></returns>
        public int getTag()
        {
            return tag;
        }
        /// <summary>
        /// 获取最后的名称
        /// </summary>
        /// <returns></returns>
        public string getFileName()
        {
            return Path.GetFileName(filePath);
        }   
        /// <summary>
        /// 上传文件
        /// </summary>
        public int upload()
        {
            if (File.Exists(Path.Combine(folderPath, Path.GetFileName(filePath))))
            {
                DialogResult dr = MessageBox.Show("文件已存在，是否覆盖？", "提示！", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    File.Copy(filePath, Path.Combine(folderPath, Path.GetFileName(filePath)), true);
                    return 0;
                }
                else if (dr == DialogResult.No)
                {
                    File.Copy(filePath, Path.Combine(folderPath, Path.GetFileName(filePath).Split('.')[0] +"_"+ DateTime.Now.ToString("HHmmss") + Path.GetExtension(filePath)), false);
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                File.Copy(filePath, Path.Combine(folderPath, Path.GetFileName(filePath)), false);
                return 0;
            }
           
        }
    }
}
