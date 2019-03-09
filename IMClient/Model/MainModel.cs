using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public static class MainModel
    {
        public static void PackageFriendList(out Message message)
        {
            message.Information.Header = MessageHeader.FriendListReq;
            message.Information.Kind = MessageKind.NonKind;
            message.Information.Length = 4;
            message.Content = null;
        }
    }
}
