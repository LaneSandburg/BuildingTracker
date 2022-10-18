using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using DataAccessLayer;

namespace LogicLayer
{
    public class RoomManager : IRoomManager
    {
        private IRoomAccessor _roomAccessor;

        public RoomManager() 
        {
            _roomAccessor = new RoomAccessor();   
        }
        public RoomManager(IRoomAccessor roomAccessor)
        {
            _roomAccessor = roomAccessor;
        }
        public int DeleteRoom(string roomId)
        {
            throw new NotImplementedException();
        }

        public Room GetRoom(string roomId)
        {
            try
            {
                return _roomAccessor.GetRoomByID(roomId);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Room not available", ex);
            }
        }

        public int InsertRoom(Room room)
        {
            throw new NotImplementedException();
        }

        public List<Room> SelectAllRoooms()
        {
            try
            {
                return _roomAccessor.SelectAllRooms();
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Rooms not available", ex); ;
            }
        }
    }
}
