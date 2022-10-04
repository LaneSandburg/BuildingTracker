using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public class UserAccessor : IUserAccessor
    {
        public int ActivateUser(int userID)
        {
            int rows = 0;
            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_user", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return rows;
        }
        public int DeactivateUser(int userID)
        {
            int rows = 0;
            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_deactivate_user", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return rows;
        }

        public User AuthenticateUser(string username, string passwordHash)
        {
            User user = null;
            int result = 0;

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_authenticate_user");
            cmd.Connection = conn;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);

            cmd.Parameters["@Email"].Value = username;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;

            try
            {
                conn.Open();

                result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result == 1)
                {
                    user = SelectUserByEmail(username);
                }
                else
                {
                    throw new ApplicationException("Username of Password Are incorrect");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return user;
        }



        public int InsertOrDeleteUserRole(int userid, string roleID, bool delete = false)
        {
            int rows = 0;

            string cmdText = delete ? "sp_delete_user_role" : "sp_insert_user_role";

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userid);
            cmd.Parameters.AddWithValue("@RoleID", roleID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int InsertUser(User user)
        {
            int UserID = 0;

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_insert_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@Email", user.Email);



            try
            {
                conn.Open();
                UserID = Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }


            return UserID;
        }

        public List<string> SelectAllRoles()
        {
            List<string> roles = new List<string>();

            var conn = DBConn.GetConnection();

            var cmd = new SqlCommand("sp_select_all_roles", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var Reader = cmd.ExecuteReader();

                while (Reader.Read())
                {
                    roles.Add(Reader.GetString(0));
                }
                Reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return roles;
        }

        public List<string> SelectRolesByUserID(int userID)
        {
            List<string> roles = new List<string>();


            var conn = DBConn.GetConnection();

            var cmd = new SqlCommand("sp_select_roles_by_userID", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);

            cmd.Parameters["@UserID"].Value = userID;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return roles;
        }

        public List<User> SelectUserByActive(bool active = true)
        {
            List<User> users = new List<User>();

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_select_users_by_active");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Active", SqlDbType.Bit);
            cmd.Parameters["@Active"].Value = active;

            try
            {
                conn.Open();
                var Reader = cmd.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var user = new User();
                        user.UserID = Reader.GetInt32(0);
                        user.FirstName = Reader.GetString(1);
                        user.LastName = Reader.GetString(2);
                        user.Email = Reader.GetString(4);
                        user.Active = active;

                        users.Add(user);

                    }
                }
                Reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return users;
        }

        public bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash)
        {
            bool result = false;

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_update_password");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@OldPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 100);

            cmd.Parameters["@UserID"].Value = userID;
            cmd.Parameters["@OldPasswordHash"].Value = oldPassHash;
            cmd.Parameters["@NewPasswordHash"].Value = newPassHash;

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { conn.Close(); }


            return result;
        }

        public int UpdateUser(User oldUser, User newUser)
        {
            int rows = 0;

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_update_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", oldUser.UserID);

            cmd.Parameters.AddWithValue("@NewFirstName", newUser.FirstName);
            cmd.Parameters.AddWithValue("@NewLastName", newUser.LastName);
            cmd.Parameters.AddWithValue("@NewEmail", newUser.Email);

            cmd.Parameters.AddWithValue("@OldFirstName", oldUser.FirstName);
            cmd.Parameters.AddWithValue("@OldLastName", oldUser.LastName);
            cmd.Parameters.AddWithValue("@OldEmail", oldUser.Email);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    throw new ApplicationException("record not found.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }


            return rows;
        }

        public User SelectUserByEmail(string email)
        {
            User user = null;

            var conn = DBConn.GetConnection();

            var cmd1 = new SqlCommand("sp_select_user_by_email", conn);
            cmd1.CommandType = CommandType.StoredProcedure;
            var cmd2 = new SqlCommand("sp_select_roles_by_userID", conn);
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd1.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd2.Parameters.Add("@UserID", SqlDbType.Int);

            cmd1.Parameters["@Email"].Value = email;

            try
            {
                conn.Open();
                var reader1 = cmd1.ExecuteReader();
                user = new User();

                user.Email = email;
                if (reader1.Read())
                {
                    user.UserID = reader1.GetInt32(0);
                    user.FirstName = reader1.GetString(1);
                    user.LastName = reader1.GetString(2);
                }
                else
                {
                    throw new ApplicationException("User Not Found");
                }
                reader1.Close();

                cmd2.Parameters["@UserID"].Value = user.UserID;
                var reader2 = cmd2.ExecuteReader();
                List<string> roles = new List<string>();
                while (reader2.Read())
                {
                    roles.Add(reader2.GetString(0));
                }
                reader2.Close();

                user.Roles = roles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;
        }

        public List<User> SelectUserByRole(string roleID)
        {
            List<User> users = new List<User>();

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_select_users_by_role");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar);
            cmd.Parameters["@RoleID"].Value = roleID;

            try
            {
                conn.Open();
                var Reader = cmd.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var user = new User();
                        user.UserID = Reader.GetInt32(0);

                        users.Add(user);

                    }
                }
                Reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return users;
        }
    }
}
