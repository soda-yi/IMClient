using IMClient.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace IMClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public IMClientSocket ClientSocket { get; } = new IMClientSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public SignInWindow SignInWindow { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string host = "149.129.68.184";
            int port = 22888;
            IPAddress ip = IPAddress.Parse(host);
            EndPoint endPoint = new IPEndPoint(ip, port);
            try
            {
                ClientSocket.Connect(endPoint);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connect Failed");
                Shutdown();
            }

            if (ClientSocket.Connected)
            {
                ClientSocket.ReceiveMessage();
                SignInWindow = new SignInWindow();
                SignInWindow.Show();
                //MainWindow = new MainWindow();
                //MainWindow.Show();
            }
        }
    }
}
