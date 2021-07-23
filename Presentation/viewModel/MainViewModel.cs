using System;
using Presentation.model;

namespace Presentation.viewModel
{
    public class MainViewModel : NotifiableObject
    {
        public BackendController Controller { get; private set; }
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                this._email = value;
                RaisePropertyChanged("Email");
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                this._password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set
            {
                this._nickName = value;
                RaisePropertyChanged("NickName");
            }
        }

        private string _hostEmail;
        public string HostEmail
        {
            get => _hostEmail;
            set
            {
                this._hostEmail = value;
                RaisePropertyChanged("HostEmail");
            }
        }
        
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                this._message = value;
                RaisePropertyChanged("Message");
            }
        }

        public MainViewModel()
        {
            Controller = new BackendController();
            this.Email = "";
            this.Password = "";
            this.NickName = "";
            this.HostEmail = "";
            this.Message = "";
        }

        public MainViewModel(BackendController cont)
        {
            this.Email = "";
            this.Password = "";
            this.NickName = "";
            this.HostEmail = "";
            this.Message = "";
            Controller = cont;
        }
        // <summary>
        /// Registers a new user and joins the user to an existing board.
        /// </summary>
        public void Register()
        {
            Message = "";
            try
            {
                Controller.register(Email, Password,NickName,HostEmail);
                Message = "Registered successfully";
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        public UserModel Login()
        {
            Message = "";
            try
            {
                return Controller.login(Email, Password);
                
            }
            catch (Exception e)
            {
                Message = e.Message;
                return null;
            }
        }
        
        ///<summary>Remove all persistent data.</summary>
        public void deleteData()
        {
            Message = "";
            try
            {
                Controller.deleteData();
                Message = "delete was successful";

            }
            catch (Exception e)
            {
                Message="Delete data: " + e.Message;
            }
        }
    }
}