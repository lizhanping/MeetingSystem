using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MeetingSystemServer
{
    /// <summary>
    /// 比较器
    /// </summary>
    public class myCompare : IComparer<TreeNode>
    {
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(TreeNode x, TreeNode y)
        {
            DirectoryInfo dix = new DirectoryInfo(x.Name);
            DirectoryInfo diy = new DirectoryInfo(y.Name);
            long xSize = getDirectoryLength(dix);
            long ySize = getDirectoryLength(diy);
            return xSize.CompareTo(ySize);
        }

        /// <summary>
        /// 获得文件夹大小
        /// </summary>
        /// <returns></returns>
        public long getDirectoryLength(DirectoryInfo di)
        {
            long diSize = 0;
            try
            {
                FileSystemInfo[] fsi = di.GetFileSystemInfos();
                foreach (FileSystemInfo fs in fsi)
                {
                    if (fs is FileInfo)
                    {
                        diSize += ((FileInfo)fs).Length;
                    }
                    else
                    {
                        diSize += getDirectoryLength((DirectoryInfo)fs);
                    }
                }
            }
            catch
            {
                diSize = 0;//目录不存在，默认为0；
            }
            return diSize;
        }
    }
}
