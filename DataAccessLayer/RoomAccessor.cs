using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using System.Reflection.PortableExecutable;

namespace DataAccessLayer
{
    public class RoomAccessor : IRoomAccessor
    {
        public int DeleteRoom(string roomID)
        {
            throw new NotImplementedException();
        }

        public Room GetRoomByID(string roomID)
        {
            var room = new Room();

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_select_room_by_id");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add ("@RoomID", SqlDbType.NVarChar);
            cmd.Parameters["@RoomID"].Value = roomID;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        room.RoomID = reader.GetString(0);
                        room.RoomNotes = reader.GetString(1);


                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return room;

        }

        public int InsertRoom(Room room)
        {
            int result = 0;

            var conn = DBConn.GetConnection();
            var cmd = new SqlCommand("sp_insert_room", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
            cmd.Parameters.AddWithValue("@RoomNotes", room.RoomNotes);




            try
            {
                conn.Open();
                result = Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }


            return result;
        }

        public List<Room> SelectAllRooms()
        {
            List<Room> rooms = new List<Room>();

            var conn = DBConn.GetConnection();

            var cmd = new SqlCommand("sp_select_all_rooms", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var Reader = cmd.ExecuteReader();
                if (Reader.HasRows) {
                    while (Reader.Read())
                    {
                        var room = new Room();
                        room.RoomID = Reader.GetString(0);
                        room.RoomNotes = Reader.GetString(1);

                        rooms.Add(room);
                    }
                }

                Reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return rooms;
        }
    }
}
