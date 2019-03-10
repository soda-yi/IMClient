using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using IMClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IMClient.ViewModel
{
    public class SignInViewModel : ViewModelBase
    {
        private readonly IMClientSocket _workSocket;

        public SignInViewModel()
        {
            // 需要使用GalaSoft.MvvmLight.CommandWpf而非GalaSoft.MvvmLight.Command, 与ICommand接口中事件的实现有关
            _mouseCommand = new RelayCommand(NextButton_Click,
                        () => UserNameTextBoxText != null && UserNameTextBoxText != string.Empty);
            _workSocket = (Application.Current as App).ClientSocket;
        }

        private string _inputTextBlockText = "User Name";
        public string InputTextBlockText
        {
            get { return _inputTextBlockText; }
            set { Set(ref _inputTextBlockText, value); }
        }

        private string _userNameTextBoxText;
        public string UserNameTextBoxText
        {
            get { return _userNameTextBoxText; }
            set { Set(ref _userNameTextBoxText, value); }
        }

        private string _nextButtonContent = "_Next";
        public string NextButtonContent
        {
            get { return _nextButtonContent; }
            set { Set(ref _nextButtonContent, value); }
        }

        private ICommand _mouseCommand;
        public ICommand MouseCommand => _mouseCommand;

        private bool _verifyUserNameComplete = false;

        private void NextButton_Click()
        {
            if (!_verifyUserNameComplete)
            {
                _workSocket.MessageArrived += VerifyUserNameProcess;
                SignInModel.PackUserName(UserNameTextBoxText, out Message message);
                _workSocket.Send(Message.ToBytes(message));
                _workSocket.UserName = UserNameTextBoxText;
            }
            else
            {
                _workSocket.MessageArrived += VerifyPasswordProcess;
                SignInModel.PackPassword(UserNameTextBoxText, out Message message);
                _workSocket.Send(Message.ToBytes(message));
            }
        }

        private void VerifyUserNameProcess(object o, MessageArrivedEventArgs messageArrivedEventArgs)
        {
            var result = messageArrivedEventArgs.Message.VerifyUserName();
            if (result == VerifyResult.Success)
            {
                InputTextBlockText = "Password";
                UserNameTextBoxText = string.Empty;
                NextButtonContent = "sign in";
                _verifyUserNameComplete = true;
            }
            else if (result == VerifyResult.SetPassword)
            {
                InputTextBlockText = "Set Your Password";
                UserNameTextBoxText = string.Empty;
                NextButtonContent = "sign up";
                _verifyUserNameComplete = true;
            }
            else
            {
                MessageBox.Show("User Name Verify Failed");
                UserNameTextBoxText = string.Empty;
                _workSocket.UserName = null;
            }
            _workSocket.MessageArrived -= VerifyUserNameProcess;
        }

        private void VerifyPasswordProcess(object o, MessageArrivedEventArgs messageArrivedEventArgs)
        {
            var result = messageArrivedEventArgs.Message.VerifyPassword();
            if (result == VerifyResult.Success)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    (Application.Current as App).SignInWindow.Close();
                }));
            }
            else
            {
                MessageBox.Show("Password Verify Failed");
                UserNameTextBoxText = string.Empty;
            }
            _workSocket.MessageArrived -= VerifyPasswordProcess;
        }
    }
}
