using GalaSoft.MvvmLight;
using IMClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows;
using System;
using System.Text;

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
            MainModel.PackageFriendList(out byte[] friendListPackage);
            _workSocket = (Application.Current as App).ClientSocket;
            _workSocket.Send(friendListPackage);
            _workSocket.MessageArrived += GetFriendList;
        }

        IMClientSocket _workSocket;

        public ObservableCollection<string> FriendList { get; set; } = new ObservableCollection<string>();
        /*private List<string> _friendList = new List<string>();
        public List<string> FriendList
        {
            get { return _friendList; }
            set { Set(ref _friendList, value); }
        }*/

        private void GetFriendList(object o, MessageArrivedEventArgs messageArrivedEventArgs)
        {
            string content = Encoding.Default.GetString(messageArrivedEventArgs.Message.Content);
            string[] friends = content.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in friends)
            {
                App.Current.Dispatcher.Invoke(new Action<string>(FriendList.Add), item);
                //FriendList.Add(item);
            }
            _workSocket.MessageArrived -= GetFriendList;
        }

    }
}