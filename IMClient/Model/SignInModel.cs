using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public static class SignInModel
    {
        public static void PackUserName(string userName, out byte[] package)
        {
            byte[] sendContent = Encoding.Default.GetBytes(userName);
            package = new byte[32];
            package[0] = 0x11;
            package[1] = 0x00;
            package[2] = 0x20;
            package[3] = 0x00;
            for (int i = 0; i < sendContent.Length; i++)
            {
                package[i + 4] = sendContent[i];
            }
        }

        public static void PackPassword(string password, out byte[] package)
        {
            byte[] sendContent = Encoding.Default.GetBytes(password);
            package = new byte[32];
            package[0] = 0x11;
            package[1] = 0x01;
            package[2] = 0x20;
            package[3] = 0x00;
            for (int i = 0; i < sendContent.Length; i++)
            {
                package[i + 4] = sendContent[i];
            }
        }

        public static VerifyResult VerifyUserName(this Message message)
        {
            //Message message = socket.RecvMessages.Dequeue();
            if (message.MessageHeader.Kind == 0x00)
            {
                return VerifyResult.Success;
            }
            else
            {
                return VerifyResult.Failed;
            }
        }

        public static VerifyResult VerifyPassword(this Message message)
        {
            //Message message = socket.RecvMessages.Dequeue();
            if (message.MessageHeader.Kind == 0x02)
            {
                return VerifyResult.Success;
            }
            else
            {
                return VerifyResult.Failed;
            }
        }
    }

    public enum VerifyResult
    {
        Success,
        SetPassword,
        Failed
    }
}
