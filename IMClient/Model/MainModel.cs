using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public static class MainModel
    {
        public static void PackageFriendList(out byte[] package)
        {
            package = new byte[4];
            package[0] = 0x13;
            package[1] = 0x00;
            package[2] = 0x04;
            package[3] = 0x00;
        }
    }
}
