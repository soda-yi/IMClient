using GalaSoft.MvvmLight;
using IMClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows;
using System;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _friendChanged = new RelayCommand<string>(OnFriendChanged);
            _messageSendButton_Click = new RelayCommand<string>(SendMessage, o => o != null && o != string.Empty);
            MainModel.PackageFriendList(out Message message);
            _workSocket = (Application.Current as App).ClientSocket;
            _workSocket.Send(Message.ToBytes(message));
            _workSocket.MessageArrived += _workSocket_MessageArrived;
            UserNameLableContent = _workSocket.UserName;
        }

        IMClientSocket _workSocket;

        public ObservableCollection<string> FriendList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<MessageView> HistoryList { get; set; } = new ObservableCollection<MessageView>();

        private ICommand _friendChanged;
        public ICommand FriendChanged => _friendChanged;

        private ICommand _messageSendButton_Click;
        public ICommand MessageSendButton_Click => _messageSendButton_Click;

        public string UserNameLableContent { get; }

        private string _selectedFriendUserName = null;
        public string SelectedFriendUserName
        {
            get { return _selectedFriendUserName; }
            set { Set(ref _selectedFriendUserName, value); }
        }

        private string _messageSendTextBoxText;
        public string MessageSendTextBoxText
        {
            get { return _messageSendTextBoxText; }
            set { Set(ref _messageSendTextBoxText, value); }
        }

        private void _workSocket_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            switch (e.Message.Information.Header)
            {
                case MessageHeader.FriendListAck:
                    GetFriendList(e.Message);
                    break;
                case MessageHeader.MessageForwardAck:
                    switch (e.Message.Information.Kind)
                    {
                        case MessageKind.HistoryForward:
                        case MessageKind.TextForward:
                            GetMessage(e.Message);
                            break;
                    }
                    break;
            }
        }

        private void GetFriendList(Message message)
        {
            Application.Current.Dispatcher.Invoke(() => FriendList.Clear());
            string content = Encoding.GetEncoding("gbk").GetString(message.Content);
            string[] friends = content.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in friends)
            {
                Application.Current.Dispatcher.Invoke(new Action<string>(FriendList.Add), item);
                //FriendList.Add(item);
            }
        }

        private void OnFriendChanged(string userName)
        {
            if (userName == null) { return; }
            SelectedFriendUserName = userName;
            HistoryList.Clear();
            Message message;
            message.Content = Encoding.GetEncoding("gbk").GetBytes('@' + userName);
            message.Information.Header = MessageHeader.HistoryReq;
            message.Information.Kind = MessageKind.NonKind;
            message.Information.Length = (short)(message.Content.Length + Message.MessageInformationLength);
            _workSocket.Send(Message.ToBytes(message));
        }

        private void SendMessage(string messageContent)
        {
            Message message;
            message.Content = Encoding.GetEncoding("gbk").GetBytes($"@{SelectedFriendUserName}:{messageContent}");
            message.Information.Header = MessageHeader.MessageSendReq;
            message.Information.Kind = MessageKind.TextSend;
            message.Information.Length = (short)(message.Content.Length + Message.MessageInformationLength);
            _workSocket.Send(Message.ToBytes(message));
            MessageView messageView = new MessageView
            {
                UserName = _workSocket.UserName,
                Time = DateTime.Now.ToString(),
                Content = messageContent,
                MessageInformationForeground = Brushes.Blue
            };
            HistoryList.Add(messageView);
            MessageSendTextBoxText = null;
        }

        private void GetMessage(Message message)
        {
            string content = Encoding.GetEncoding("gbk").GetString(message.Content, sizeof(long), message.Content.Length - sizeof(long));
            string[] contents = content.Split(new char[] { '@', ':' });
            if (contents[0] != SelectedFriendUserName && contents[1] != SelectedFriendUserName)
            {
                return;
            }
            long time = BitConverter.ToInt64(message.Content, 0);
            MessageView messageView = new MessageView
            {
                UserName = contents[0],
                Time = MainModel.UnixTimeToDateTime(time).ToString(),
                Content = contents[2],
                MessageInformationForeground = contents[0] == _workSocket.UserName ? Brushes.Blue : Brushes.Green
            };
            App.Current.Dispatcher.Invoke(new Action<MessageView>(HistoryList.Add), messageView);
        }

    }
}