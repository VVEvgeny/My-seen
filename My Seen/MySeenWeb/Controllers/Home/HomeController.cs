﻿using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MySeenLib;
using MySeenWeb.ActionFilters;
using MySeenWeb.Add_Code.Services.Logging.NLog;
using MySeenWeb.Controllers._Base;
using MySeenWeb.Models;
using MySeenWeb.Models.TablesLogic;
using MySeenWeb.Models.Tools;

namespace MySeenWeb.Controllers.Home
{
    //[RequireHttps]
    public class HomeController : BaseController
    {
        [BrowserActionFilter]
        public ActionResult Index()
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult Index()";
            try
            {
                return View(new HomeViewModel(
                    ReadUserSideStorage(UserSideStorageKeys.MarkersOnRoads, 0),
                    ReadUserSideStorage(UserSideStorageKeys.HomeCategory, (int)Defaults.CategoryBase.Indexes.Films)
                        .ToString(),
                    User.Identity.IsAuthenticated ? User.Identity.GetUserId() : string.Empty,
                    ReadUserSideStorage(UserSideStorageKeys.ImprovementsCategory, (int)Defaults.ComplexBase.Indexes.All),
                    ReadUserSideStorage(UserSideStorageKeys.EndedEvents, 0) == 1
                    ));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        public JsonResult EditFilm(string id, string name, string year, string datetime, string genre, string rating)
        {
            var logger = new NLogLogger();
            var methodName = "public JsonResult EditFilm(string id, string name, string year, string datetime, string genre, string rating)";
            try
            {
                var logic = new FilmsLogic();
                return !logic.Update(id, name, year, datetime, genre, rating, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        
        [Authorize]
        [HttpPost]
        public JsonResult EditSerial(string id, string name, string year, string season, string series, string datetime, string genre, string rating)
        {
            var logger = new NLogLogger();
            var methodName = "public JsonResult EditSerial(string id, string name, string year, string season, string series, string datetime, string genre, string rating)";
            try
            {
                var logic = new SerialsLogic();
                return !logic.Update(id, name, year, season, series, datetime, genre, rating, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditEvent(string id, string name, string datetime, string type)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult EditEvent(string id, string name, string datetime, string type)";
            try
            {
                var logic = new EventsLogic();
                return !logic.Update(id, name, datetime, type, User.Identity.GetUserId())
                    ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult DeleteEvent(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult DeleteEvent(string id)";
            try
            {
                var logic = new EventsLogic();
                return !logic.Delete(id, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditBook(string id, string name, string year, string authors, string datetime, string genre, string rating)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult EditBook(string id, string name, string year, string authors, string datetime, string genre, string rating)";
            try
            {
                var logic = new BooksLogic();
                return !logic.Update(id, name, year, authors, datetime, genre, rating, User.Identity.GetUserId())
                    ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult DeleteFilm(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult DeleteFilm(string id)";
            try
            {
                var logic = new FilmsLogic();
                return !logic.Delete(id, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult DeleteSerial(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult DeleteSerial(string id)";
            try
            {
                var logic = new SerialsLogic();
                return !logic.Delete(id, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult DeleteBook(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult DeleteBook(string id)";
            try
            {
                var logic = new BooksLogic();
                return !logic.Delete(id, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult EndImprovement(string id, string desc, string version)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult EndImprovement(string id, string desc, string version)";
            try
            {
                if (Admin.IsAdmin(User.Identity.Name))
                {
                    var logic = new ImprovementLogic();
                    return !logic.Update(id, desc, version, User.Identity.GetUserId())
                        ? new JsonResult {Data = new {success = false, error = logic.ErrorMessage}}
                        : Json(new {success = true});
                }
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public JsonResult DeleteImprovement(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult DeleteImprovement(string id)";
            try
            {
                var logic = new ImprovementLogic();
                return !logic.Delete(id, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        
        [Authorize]
        [HttpPost]
        public JsonResult EditTrack(string id, string name, string datetime, string type, string coordinates, string distance)
        {
            var logger = new NLogLogger();
            const string methodName = "public JsonResult EditTrack(string id, string name, string datetime, string type, string coordinates, string distance)";
            try
            {
                var logic = new TracksLogic();
                return !logic.Update(id, name, datetime, type, coordinates, distance, User.Identity.GetUserId()) ? new JsonResult { Data = new { success = false, error = logic.ErrorMessage } } : Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTrack(int id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrack(int id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrack(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        //[Authorize]
        [HttpPost]
        public ActionResult GetTrackByKey(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrackByKey(string id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrackByKey(id, ReadUserSideStorage(UserSideStorageKeys.RoadsYear, 0)));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTrackByType(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrackByType(string id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrackByType(id, ReadUserSideStorage(UserSideStorageKeys.RoadsYear, 0)));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [HttpPost]
        public ActionResult GetMarkers()
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetMarkers()";
            try
            {
                return Json(ReadUserSideStorage(UserSideStorageKeys.MarkersOnRoads, 0) == (int)Defaults.EnabledDisabledBase.Indexes.Enabled);
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTrackNameById(int id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrackNameById(int id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrackNameById(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTrackDateById(int id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrackDateById(int id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrackDateById(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteNLog(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult DeleteNLog(string id)";
            try
            {
                if (Admin.IsAdmin(User.Identity.Name))
                {
                    var logic = new NLogErrorsLogic();
                    return !logic.Delete(id, User.Identity.GetUserId())
                        ? new JsonResult {Data = new {success = false, error = logic.ErrorMessage}}
                        : Json(new {success = true});
                }
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetEventShare(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetEventShare(string id)";
            try
            {
                return Json(HomeViewModelEvents.GetShare(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GenerateEventShare(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GenerateEventShare(string id)";
            try
            {
                return Json(HomeViewModelEvents.GenerateShare(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteEventShare(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult DeleteEventShare(string id)";
            try
            {
                return Json(HomeViewModelEvents.DeleteShare(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        public ActionResult DeleteTrack(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult DeleteTrack(string id)";
            try
            {
                HomeViewModelRoads.DeleteTrack(id, User.Identity.GetUserId());
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetTrackCoordinatesById(int id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult GetTrackCoordinatesById(int id)";
            try
            {
                return Json(HomeViewModelRoads.GetTrackCoordinatesById(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
        
        [BrowserActionFilter]
        [Authorize]
        public ActionResult TrackEditor(string id)
        {
            var logger = new NLogLogger();
            const string methodName = "public ActionResult TrackEditor(string id)";
            try
            {
                return View(new HomeViewModelTrackEditor(id, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                logger.Error(methodName, ex);
            }
            return new JsonResult { Data = new { success = false, error = methodName } };
        }
    }
}