using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.BusinessLayer.User_Package
{
    public class UserController
    {
        readonly int minPassword = 5;
        readonly int maxPassword = 25;
        private DalController dalController;
        
        private Dictionary<string, User> users { get; set; }

        /// <summary>Constructor of a UserController
        public UserController()
        {
            dalController=new DalController();
            users=new Dictionary<string, User>();
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        public void loadData()
        {
            users=dalController.readUsers();
            foreach (var user in users.Values)
            {
                user.logout();
            }
            
        }
        
        /// <summary>        
        /// getter for the kanban users
        /// </summary>
        /// <returns>A Dictionary object. The Dictionary should contain a map of <email, User> of all the users details
        public Dictionary<string, User> getUsers()
        {
            return users;
        }
        
        /// <summary>        
        /// login to the kanban system. Intended be invoked only when the user login successfully
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A User object. The User should contain the user details
        public User login(string email, string password)
        {
            if (!this.users.ContainsKey(email)) 
            {
                throw new Exception("User was not found");
            }
            if(users[email].isLoggedIn())
                throw new Exception("user is already logged in");
            if (!this.users[email].getPassword().Equals(password)) 
            {
                throw new Exception("Wrong password");
            }
            
            this.users[email].login();
            users[email].toDalObject().save();

            return users[email];
        }

        /// <summary>        
        /// logout from the kanban system. Intended be invoked only when the user logout successfully
        /// </summary>
        /// <param name="email">The email address of the user to logout</param>
        public void logout(string email)
        {
            if (!this.users.ContainsKey(email)) 
            {
                throw new Exception("User was not found");
            }
            if(!users[email].isLoggedIn())
                throw new Exception("user is already logged out ");
            users[email].logout();
            users[email].toDalObject().save();
        }

        /// <summary>
        /// checks for valid email of a user, returns true for legal email and false otherwise
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
   
        /// <summary>
        /// Registers a new user and adds it to the users controller
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        public void register(string email, string nickname, string password)
        {
            if(!IsValidEmail(email))
                throw new Exception("email is illegal");
            foreach (var uEmail in users.Keys)
            {
                if(uEmail.ToLower()==email.ToLower())
                    throw new Exception("the user already exists");
            }
            if (string.IsNullOrWhiteSpace(password)  || !(password.Length >= minPassword && password.Length <= maxPassword) || !(password.Any(char.IsUpper)) || !(password.Any(char.IsLower)) || !(password.Any(char.IsDigit)))
            {
                throw new Exception("A user password must be in length of 4 to 20 characters and must include at"+'\n' +
                                    "least one uppercase letter, one small character and a number. ");
            }
            if (string.IsNullOrWhiteSpace(nickname) || nickname.Length<1)
            {
                throw new Exception("cannot register with empty nickname");
            }
            
            User u = new User(email, nickname, password);
            users.Add(email, u);
        }

        ///<summary>
        /// create a new User and add it to the controller of the users.</summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        public void createUser(string email, string password, string nickname)
        {
            users.Add(email, new User(email, nickname, password));
        }

        ///<summary>Remove all the User persistent data.</summary>
        public void deleteData()
        {
            users=new Dictionary<string, User>();
        }
        
        ///<summary>save all the Users data in the DataBase.</summary>
        public void saveUsers()
        {
            foreach (var user in users.Values)
            {
                user.toDalObject().save();
            }
        }
    }
}