using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IUserAccessor
    {
        User AuthenticateUser(string username, string passwordHash);
        bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash);
        List<User> SelectUserByActive(bool active = true);
        User SelectUserByEmail(string email);

        List<User> SelectUserByRole(string roleID);

        int UpdateUser(User oldUser, User newUser);
        int InsertUser(User user);
        int DeactivateUser(int userID);
        int ActivateUser(int userID);
        List<string> SelectAllRoles();
        List<string> SelectRolesByUserID(int userID);

        int InsertOrDeleteUserRole(int userid, string roleID, bool delete = false);
    }
}
