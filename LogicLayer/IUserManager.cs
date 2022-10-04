using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IUserManager
    {
        User AuthenticateUser(string email, string password);
        bool ResetPasword(int userID, string oldPassword, string newPassword);
        List<User> GetUserListByActive(bool active = true);
        bool FindUser(string email);

        bool EditUser(User oldUser, User newUser);

        bool AddUser(User user);

        bool SetUserActiveState(bool active, int userID);
        List<string> RetreiveUserRoles();
        List<string> RetreiveUserRoles(int userID);

        List<User> RetreiveUserByRole(string roleID);

        bool deleteUserRole(int userID, string roleID);
        bool addUserRole(int userID, string roleID);
        int RetrieveUserIDFromEmail(string email);


    }
}
