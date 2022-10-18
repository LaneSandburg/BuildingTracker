using DataObjects;
using LogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingTracker.Controllers
{
    public class MapController : Controller
    {
        private IRoomManager _roomManager = null;
        public MapController()
        {
            _roomManager = new RoomManager();
        }
        // GET: MapController
        public ActionResult Index()
        {
            ViewBag.title = "List of Rooms";
            var rooms = _roomManager.SelectAllRoooms();
            return View(rooms);
        }
        public ActionResult Map()
        {
            ViewBag.title = "Map of Rooms";
            var rooms = _roomManager.SelectAllRoooms();
            return View(rooms);
        }

        public ActionResult RoomDetails(string ID)
        {
            var room = _roomManager.GetRoom(ID);
            ViewBag.Title = room.RoomID + "Details";
            return View(room);
        }


        // GET: MapController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MapController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MapController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MapController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MapController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MapController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MapController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
