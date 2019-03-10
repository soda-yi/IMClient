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
            MainModel.PackageFriendList(out Message message);
            _friendChanged = new RelayCommand<string>(OnFriendChanged);
            _workSocket = (Application.Current as App).ClientSocket;
            _workSocket.Send(Message.ToBytes(message));
            _workSocket.MessageArrived += _workSocket_MessageArrived;
        }

        IMClientSocket _workSocket;

        public ObservableCollection<string> FriendList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<MessageView> HistoryList { get; set; } = new ObservableCollection<MessageView>();

        private ICommand _friendChanged;
        public ICommand FriendChanged => _friendChanged;

        private string _selectedFriendUserName;
        public string SelectedFriendUserName
        {
            get { return _selectedFriendUserName; }
            set { Set(ref _selectedFriendUserName, value); }
        }

        private void _workSocket_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            switch (e.Message.Information.Header)
            {
                case MessageHeader.FriendListAck:
                    GetFriendList(e.Message);
                    break;
                case MessageHeader.MessageForwardAck:
                    GetHistory(e.Message);
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

        private void GetHistory(Message message)
        {
            if (message.Information.Length <= Message.MessageInformationLength)
            {
                return;
            }
            string content = Encoding.GetEncoding("gbk").GetString(message.Content, 19, message.Content.Length - 19);
            string[] contents = content.Split(new char[] { '@', ':' });
            MessageView messageView = new MessageView
            {
                UserName = contents[0],
                Time = Encoding.GetEncoding("gbk").GetString(message.Content, 0, 19),
                Content = contents[2],
                MessageInformationForeground = contents[0] == _workSocket.UserName ? Brushes.Blue : Brushes.Green
            };
            App.Current.Dispatcher.Invoke(new Action<MessageView>(HistoryList.Add), messageView);
        }

        private void OnFriendChanged(string userName)
        {
            SelectedFriendUserName = userName;
            HistoryList.Clear();
            Message message;
            message.Content = Encoding.GetEncoding("gbk").GetBytes('@' + userName);
            message.Information.Header = MessageHeader.HistoryReq;
            message.Information.Kind = MessageKind.NonKind;
            message.Information.Length = (short)(message.Content.Length + Message.MessageInformationLength);
            _workSocket.Send(Message.ToBytes(message));
        }

    }
}