using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public struct MessageHeader
    {
        public byte Header;
        public byte Kind;
        public short Length;
    }

    public struct Message
    {
        public MessageHeader MessageHeader;
        public byte[] Content;
        public const int MessageHeaderLength = 4;

        public static Message ConvertToMessage(byte[] bytes)
        {
            Message message;
            message.MessageHeader.Header = bytes[0];
            message.MessageHeader.Kind = bytes[1];
            message.MessageHeader.Length = BitConverter.ToInt16(bytes, 2);
            message.Content = new byte[message.MessageHeader.Length - 4];
            for (int i = 0; i < message.Content.Length; i++)
            {
                message.Content[i] = bytes[i + 4];
            }
            return message;
        }
    }
}
