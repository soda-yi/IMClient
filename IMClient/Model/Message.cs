using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public struct MessageInformation
    {
        public MessageHeader Header;
        public MessageKind Kind;
        public short Length;
    }

    public struct Message
    {
        public MessageInformation Information;
        public byte[] Content;
        public const short MessageInformationLength = 4;

        public static Message GetMessage(byte[] bytes)
        {
            Message message;
            message.Information.Header = (MessageHeader)bytes[0];
            message.Information.Kind = (MessageKind)bytes[1];
            message.Information.Length = BitConverter.ToInt16(bytes, 2);
            message.Content = message.Information.Length == MessageInformationLength ? null :
                new byte[message.Information.Length - MessageInformationLength];
            for (int i = MessageInformationLength; i < message.Information.Length; i++)
            {
                message.Content[i - MessageInformationLength] = bytes[i];
            }
            return message;
        }

        public static byte[] ToBytes(Message message)
        {
            byte[] bytes = new byte[message.Information.Length];
            bytes[0] = (byte)message.Information.Header;
            bytes[1] = (byte)message.Information.Kind;
            byte[] length = BitConverter.GetBytes(message.Information.Length);
            bytes[2] = length[0];
            bytes[3] = length[1];
            for (int i = MessageInformationLength; i < message.Information.Length; i++)
            {
                bytes[i] = message.Content[i - MessageInformationLength];
            }
            return bytes;
        }
    }

    public enum MessageHeader : byte
    {
        UserVerifyReq = 0x11,
        MessageSendReq,
        FriendListReq,
        HistoryReq,
        FileReceive,
        UserVerifyAck = 0x71,
        MessageSendAck,
        RemoteSignIn,
        MessageForwardAck,
        FriendListAck,
        FileReceiveAck
    }

    public enum MessageKind : byte
    {
        NonKind,

        UserNameVerify=0x00,
        PasswordVerify,

        TextSend=0x00,
        FileSend,
        FileBeginSend,
        FileEndSend,

        FileReceiveAccpet=0x00,
        FileReceiveRefuse=0xff,

        UserNameVerifyCorrect=0x00,
        UserNameVerifyCorrectWithoutPassword,
        PasswordVerifyCorrect,
        UserNameVerifyIncorrect=0xff,
        PasswordVerifyIncorrect=0xfe,

        RemoteSignOut=0x00,
        RemoteSignIn,

        TextForward=0x00,
        FileForward,
        FileBeginForward,
        FileEndForward,
        HistoryForward
    }
}
