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
        public static void PackUserName(string userName, out Message message)
        {
            message.Information.Header = MessageHeader.UserVerifyReq;
            message.Information.Kind = MessageKind.UserNameVerify;
            message.Information.Length = 32;
            message.Content = new byte[message.Information.Length - Message.MessageInformationLength];
            Encoding.GetEncoding("gbk").GetBytes(userName, 0, userName.Length, message.Content, 0);
        }

        public static void PackPassword(string password, out Message message)
        {
            message.Information.Header = MessageHeader.UserVerifyReq;
            message.Information.Kind = MessageKind.PasswordVerify;
            message.Information.Length = 32;
            message.Content= new byte[message.Information.Length - Message.MessageInformationLength];
            Encoding.GetEncoding("gbk").GetBytes(password, 0, password.Length, message.Content, 0);
        }

        public static VerifyResult VerifyUserName(this Message message)
        {
            switch (message.Information.Kind)
            {
                case MessageKind.UserNameVerifyCorrect:
                    return VerifyResult.Success;
                case MessageKind.UserNameVerifyCorrectWithoutPassword:
                    return VerifyResult.SetPassword;
                default:
                    return VerifyResult.Failed;
            }
        }

        public static VerifyResult VerifyPassword(this Message message)
        {
            if (message.Information.Kind == MessageKind.PasswordVerifyCorrect)
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
