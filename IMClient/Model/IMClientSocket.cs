using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMClient.Model
{
    public class IMClientSocket : Socket
    {
        //private Queue<Message> _recvMessages = new Queue<Message>();
        private byte[] _recvBuffer = new byte[1050];
        private int _bufferCurrentPosition = 0;

        public string UserName { get; set; }

        //public Queue<Message> RecvMessages => _recvMessages;
        public event EventHandler<MessageArrivedEventArgs> MessageArrived;

        public IMClientSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
        }

        public void ReceiveMessage()
        {
            if (_bufferCurrentPosition < Message.MessageInformationLength)
            {
                this.BeginReceive(_recvBuffer, _bufferCurrentPosition,
                    Message.MessageInformationLength - _bufferCurrentPosition,
                    SocketFlags.None, RecvCallback, this);
            }
            else
            {
                short messageLength = BitConverter.ToInt16(_recvBuffer, 2);
                this.BeginReceive(_recvBuffer, _bufferCurrentPosition,
                    messageLength - _bufferCurrentPosition,
                    SocketFlags.None, RecvCallback, this);
            }
        }

        private void RecvCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int length = socket.EndReceive(ar);
            if (length < Message.MessageInformationLength)
            {
                _bufferCurrentPosition += length;
            }
            else
            {
                short messageLength = BitConverter.ToInt16(_recvBuffer, 2);
                if (messageLength == _bufferCurrentPosition + length)
                {
                    //RecvMessages.Enqueue(Message.ConvertToMessage(_recvBuffer));
                    MessageArrived?.Invoke(this, new MessageArrivedEventArgs(Message.GetMessage(_recvBuffer)));
                    _bufferCurrentPosition = 0;
                }
                else
                {
                    _bufferCurrentPosition += length;
                }
            }
            ReceiveMessage();
        }
    }

    public class MessageArrivedEventArgs : EventArgs
    {
        public Message Message { get; }
        public MessageArrivedEventArgs(Message message)
        {
            Message = message;
        }
    }
}
