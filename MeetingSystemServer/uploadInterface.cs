using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeetingSystemServer
{
    interface uploadInterface
    {
        void setSrc(string path);
        void setDes(string path);
        int upload();
        int getTag();
        string getFileName();
    }
}
