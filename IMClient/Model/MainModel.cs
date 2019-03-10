using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

    public class MessageView
    {
        public string UserName { get; set; }
        public string Time { get; set; }
        public string Content { get; set; }
        public Brush MessageInformationForeground { get; set; }
    }
}
