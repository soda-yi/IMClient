using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IMClient
{
    /// <summary>
    /// SignIn.xaml 的交互逻辑
    /// </summary>
    public partial class SignInWindow : Window
    {
        private bool _isUserNameSuccess = false;

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendPackage;
            if (!_isUserNameSuccess)
            {
                PackUserName(UserNameTextBox.Text, out sendPackage);
            }
            else
            {
                PackPassword(PasswordPasswordBox.Password, out sendPackage);
            }
            var socket = (Application.Current as App).ClientSocket;
            socket.Send(sendPackage);
            byte[] recvPackage = new byte[4];
            socket.Receive(recvPackage);
            if (!_isUserNameSuccess)
            {
                if (recvPackage[1] == 0x00)
                {
                    MessageBox.Show("User Name Verify Success");
                    UserNameTextBox.Visibility = Visibility.Hidden;
                    PasswordPasswordBox.Visibility = Visibility.Visible;
                    InputTextBlock.Text = "Password";
                    NextButton.Content = "sign in";
                    _isUserNameSuccess = true;
                }
                else
                {
                    MessageBox.Show("User Name Verify Failed");
                    UserNameTextBox.Clear();
                    return;
                }
            }
            else
            {
                if (recvPackage[1] == 0x02)
                {
                    MessageBox.Show("Password Verify Success");
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Password Verify Failed");
                    PasswordPasswordBox.Clear();
                    return;
                }
            }
        }

        private void PackUserName(string userName, out byte[] package)
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

        private void PackPassword(string password, out byte[] package)
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
    }
}
