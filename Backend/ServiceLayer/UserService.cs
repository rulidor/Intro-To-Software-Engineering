using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.User_Package;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserService
    {
        private UserController users;
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Simple public constructor.
        /// </summary>
        public UserService()
        {
            users = new UserController();
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        public void loadData()
        {
            try
            {
                users.loadData();
                logger.Info("load users data from dataBase successfully");
                users.saveUsers();

            }
            catch (Exception e)
            {
                logger.Error("error while loading data");
                throw e;
            }
        }
        
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string email, string password, string nickname)
        {
            try
            {
                users.register(email, nickname, password);
                users.saveUsers();
                logger.Info(email+" registered to the system");
            }
            catch (Exception e)
            {
                logger.Error("REGISTER- "+e.Message);
                return new Response<Exception>(e, e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            User newUser;
            try
            {
                BusinessLayer.User_Package.User user = users.login(email, password);
                newUser = new User(email, user.getNickName());
                logger.Info(email+" has logged into the system");
                //users.saveUsers();
            }
            catch (Exception e)
            {
                logger.Error("LOGIN- "+e.Message);
                return new Response<User>(e.Message);
            }
            return new Response<User>(newUser, null);
        }

        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            try
            {
                
                users.logout(email);
                logger.Info(email +" has logged out from the system");
                //users.saveUsers();
            }
            catch (Exception e)
            {
                logger.Warn("user is already logged out");
                return new Response( e.Message);
            }
            return new Response();
        }

        /// <summary>        
        /// getter for the kanban users
        /// </summary>
        /// <returns>A Dictionary object. The Dictionary should contain a map of <email, User> of all the users details
        public Dictionary<string, BusinessLayer.User_Package.User> getUsers()
        {
            return users.getUsers();
        }

        /// <summary>        
        /// checks the login status of a user. true if the user is logged in and false otherwise
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>bool. The bool should be true for a looged in user and false otherwise
        public bool isLoggedIn(string email)
        {
            if (!users.getUsers().ContainsKey(email))
            {
                throw new Exception("user is not in the system");
            }
            return users.getUsers()[email].isLoggedIn();
        }

        ///<summary>Remove all the User persistent data.</summary>
        public void deleteData()
        {
            try
            {
                users.deleteData();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}