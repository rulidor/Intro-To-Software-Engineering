namespace IntroSE.Kanban.Backend.BusinessLayer.User_Package
{
    public class User : IPersistedObject<DataAccessLayer.User>
    {
        private string Email { get; set; }
        private string Nickname { get; set; }
        private string password { get; set; }
        private bool isLoggedin { get; set; }

        /// <summary>        
        /// Constructor of a User by email, nickname and password
        /// </summary>
        /// /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        public User(string email, string nickname, string password)
        {
            this.Email = email;
            this.Nickname = nickname;
            this.password = password;
            isLoggedin = false;
        }
        /// <summary>        
        /// Constructor of a User by email, nickname, password and loggedIn status
        /// </summary>
        /// /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <param name="isLogged">The login status of the user to login</param>
        public User(string email, string nickname, string password,bool isLogged)
        {
            this.Email = email;
            this.Nickname = nickname;
            this.password = password;
            isLoggedin = isLogged;
        }
        
        /// <summary>        
        /// login to the kanban system. Intended be invoked only when the user login successfully
        /// </summary>
        public void login()
        {
            isLoggedin = true;
        }
        
        /// <summary>        
        /// logout from the kanban system. Intended be invoked only when the user logout successfully
        /// </summary>
        public void logout()
        {
            isLoggedin = false;
        }

        /// <summary>        
        /// update the user password. Intended be invoked only when the old password equals to the current password
        /// and the new password is different 
        /// </summary>
        /// <param name="password">The password of the user to change, the new password</param>
        public void changePassword(string newPassword)
        {
            password = newPassword;
        }

        /// <summary>        
        /// setter for the user nickname.
        /// </summary>
        /// <param name="nickname">The nickname of the user to update</param>
        public void setNickName(string nickname)
        {
            this.Nickname = nickname;
        }

        /// <summary>        
        /// setter for the user email.
        /// </summary>
        /// <param name="email">The email address of the user to update</param>
        public void setEmail(string email)
        {
            this.Email = email;
        }

        /// <summary>        
        /// getter for the user password
        /// </summary>
        /// <returns>A string. The string should contain the current user password in the system
        public string getPassword()
        {
            return password;
        }

        /// <summary>        
        /// getter for the user nickname
        /// </summary>
        /// <returns>A string. The string should contain the current user nickname in the system
        public string getNickName()
        {
            return Nickname;
        }

        /// <summary>        
        /// getter for the user email
        /// </summary>
        /// <returns>A string. The string should contain the current user email in the system
        public string getEmail()
        {
            return Email;
        }

        /// <summary>        
        /// getter for the user loggedIn status
        /// </summary>
        /// <returns>A bool. The bool should contain true if the user currently logged in to the system
        /// and false otherwise
        public bool isLoggedIn()
        {
            return isLoggedin;
        }
        
        /// <summary>        
        /// transfers the object between layers to deal with DB and logic, convert from Bussiness Layer Object
        /// to Data Access Layer Object
        /// </summary>
        /// <returns>A DataAccessLayer.User object. The DataAccessLayer.User should contain a User that can deal with the DB
        public DataAccessLayer.User toDalObject()
        {
            return new DataAccessLayer.User(this.Email,this.Nickname,this.password,this.isLoggedin);
        }
    }
}