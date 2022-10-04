using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class UserManager : IUserManager
    {
        private IUserAccessor _userAccessor;

        public UserManager()
        {
            _userAccessor = new UserAccessor();
        }

        public UserManager(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public bool AddUser(User user)
        {
            bool result = false;
            try
            {
                result = (_userAccessor.InsertUser(user) > 0);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("User Add Failed", ex);
            }
            return result;
        }

        public bool addUserRole(int userID, string roleID)
        {
            bool result = false;

            try
            {
                result = (1 == _userAccessor.InsertOrDeleteUserRole(userID, roleID));
            }
            catch (Exception ex)
            {

                throw new ApplicationException("update failed. ", ex);
            }
            return result;
        }

        public bool deleteUserRole(int userID, string roleID)
        {
            bool result = false;

            try
            {
                result = (1 == _userAccessor.InsertOrDeleteUserRole(userID, roleID, delete: true));
            }
            catch (Exception ex)
            {

                throw new ApplicationException("update failed. ", ex);
            }
            return result;
        }

        public User AuthenticateUser(string email, string password)
        {
            User user = null;
            try
            {
                var passwordHash = hashPassword(password);
                password = null;

                user = _userAccessor.AuthenticateUser(email, passwordHash);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("login failed,", ex);
            }
            return user;
        }



        public bool EditUser(User oldUser, User newUser)
        {
            bool result = false;
            try
            {
                result = (1 == _userAccessor.UpdateUser(oldUser, newUser));
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Update failed", ex);
            }
            return result;
        }

        public List<User> GetUserListByActive(bool active = true)
        {
            try
            {

                return _userAccessor.SelectUserByActive(active);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("List Not Available", ex);
            }
        }

        public bool ResetPasword(int userID, string oldPassword, string newPassword)
        {
            bool result = false;

            try
            {
                string oldHash = hashPassword(oldPassword);
                string newHash = hashPassword(newPassword);
                result = _userAccessor.UpdatePasswordHash(userID, oldHash, newHash);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed. ", ex);
            }
            return result;
        }

        public List<string> RetreiveUserRoles()
        {
            List<string> roles = null;

            try
            {
                roles = _userAccessor.SelectAllRoles();
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Roles not found", ex);
            }

            return roles;
        }

        public List<string> RetreiveUserRoles(int userID)
        {
            List<string> roles = null;

            try
            {
                roles = _userAccessor.SelectRolesByUserID(userID);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Roles not found", ex);
            }

            return roles;
        }


        public bool SetUserActiveState(bool active, int userID)
        {
            bool result = false;
            try
            {
                if (active)
                {
                    result = (1 == _userAccessor.ActivateUser(userID));
                }
                else
                {
                    result = (1 == _userAccessor.DeactivateUser(userID));
                }

            }
            catch (Exception ex)
            {

                throw new ApplicationException("Update failed.", ex);
            }


            return result;
        }

        private string hashPassword(string Password)
        {
            string result = " ";


            byte[] data;

            using (SHA256 sha256hash = SHA256.Create())
            {

                data = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(Password));
                var Builder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {

                    Builder.Append(data[i].ToString("x2"));
                }
                result = Builder.ToString().ToUpper();
            }

            return result;
        }

        public List<User> RetreiveUserByRole(string roleID)
        {

            try
            {

                return _userAccessor.SelectUserByRole(roleID);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("List Not Available", ex);
            }
        }

        public bool FindUser(string email)
        {
            try
            {
                return _userAccessor.SelectUserByEmail(email) != null;
            }
            catch (ApplicationException ax)
            {
                if (ax.Message == "User Not Found")
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Database Error", ex);
            }
        }

        public int RetrieveUserIDFromEmail(string email)
        {
            try
            {
                return _userAccessor.SelectUserByEmail(email).UserID;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Database Error", ex);
            }
        }
    }
}
