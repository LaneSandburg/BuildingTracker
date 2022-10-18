using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IRoomManager
    {
        int InsertRoom(Room room);
        int DeleteRoom(string roomId);
        List<Room> SelectAllRoooms();
        Room GetRoom(string roomId);   

    }
}
