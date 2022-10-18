using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IRoomAccessor
    {
        int InsertRoom(Room room);
        int DeleteRoom(string roomID);
        List<Room> SelectAllRooms();
        Room GetRoomByID(string roomID);
    }
}
