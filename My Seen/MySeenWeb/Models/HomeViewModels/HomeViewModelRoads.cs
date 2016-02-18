﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MySeenLib;
using System.Globalization;
using MySeenWeb.Add_Code;
using MySeenWeb.Models.OtherViewModels;
using MySeenWeb.Models.TablesViews;

namespace MySeenWeb.Models
{
    public class HomeViewModelRoads
    {
        public IEnumerable<RoadsView> DataFoot { get; set; }
        public IEnumerable<RoadsView> DataCar { get; set; }
        public IEnumerable<RoadsView> DataBike { get; set; }
        public IEnumerable<SelectListItem> YearsList { get; set; }

        public HomeViewModelRoads(string userId, int roadYear, string search, string shareKey, ICacheService cache)
        {
            DataFoot =
                cache.Get<IEnumerable<RoadsView>>(cache.GetFormatedName(CacheNames.UserRoadsFoot.ToString(), userId,
                    roadYear, search, shareKey));
            DataCar =
                cache.Get<IEnumerable<RoadsView>>(cache.GetFormatedName(CacheNames.UserRoadsCar.ToString(), userId,
                    roadYear, search, shareKey));
            DataBike =
                cache.Get<IEnumerable<RoadsView>>(cache.GetFormatedName(CacheNames.UserRoadsBike.ToString(), userId,
                    roadYear, search, shareKey));
            YearsList =
                cache.Get<IEnumerable<SelectListItem>>(cache.GetFormatedName(CacheNames.UserRoadsYearsList.ToString(),
                    userId, roadYear, search, shareKey));

            if (DataFoot == null || DataCar == null || DataBike == null || YearsList == null)
            {
                var ac = new ApplicationDbContext();

                if (DataFoot == null)
                {
                    DataFoot =
                        ac.Tracks.AsNoTracking()
                            .Where(
                                t =>
                                    t.Type == (int) RoadTypes.Foot
                                    && (roadYear == 0 || t.Date.Year == roadYear)
                                    && ((string.IsNullOrEmpty(shareKey) && t.UserId == userId)
                                        ||
                                        (!string.IsNullOrEmpty(shareKey) &&
                                         (t.User.ShareTracksFootKey == shareKey || t.ShareKey == shareKey)))
                                    && (string.IsNullOrEmpty(search) || t.Name.Contains(search))
                            )
                            .OrderByDescending(t => t.Date)
                            .Select(RoadsView.Map)
                            .ToList();
                    if (DataFoot.Any())
                    {
                        var dataFootAll = new List<RoadsView>
                        {
                            new RoadsView
                            {
                                Id = -(int) RoadTypes.Foot,
                                Name = Resource.All,
                                UserId = DataFoot.Count().ToString(),
                                Date = new DateTime(1980, 3, 3),
                                Distance = DataFoot.Sum(item => item.Distance),
                                ShareKey = "+"
                            }
                        };
                        DataFoot = dataFootAll.Concat(DataFoot);
                    }
                    cache.Set(cache.GetFormatedName(CacheNames.UserRoadsFoot.ToString(), userId, roadYear, search, shareKey), DataFoot, 15);
                }
                if (DataCar == null)
                {
                    DataCar =
                        ac.Tracks.AsNoTracking()
                            .Where(
                                t =>
                                    t.Type == (int) RoadTypes.Car
                                    && (roadYear == 0 || t.Date.Year == roadYear)
                                    && ((string.IsNullOrEmpty(shareKey) && t.UserId == userId)
                                        ||
                                        (!string.IsNullOrEmpty(shareKey) &&
                                         (t.User.ShareTracksCarKey == shareKey || t.ShareKey == shareKey)))
                                    && (string.IsNullOrEmpty(search) || t.Name.Contains(search))
                            )
                            .OrderByDescending(t => t.Date)
                            .Select(RoadsView.Map)
                            .ToList();
                    if (DataCar.Any())
                    {
                        var dataCarAll = new List<RoadsView>
                        {
                            new RoadsView
                            {
                                Id = -(int) RoadTypes.Car,
                                Name = Resource.All,
                                UserId = DataCar.Count().ToString(),
                                Date = new DateTime(1980, 3, 3),
                                Distance = DataCar.Sum(item => item.Distance),
                                ShareKey = "+"
                            }
                        };
                        DataCar = dataCarAll.Concat(DataCar);
                    }
                    cache.Set(cache.GetFormatedName(CacheNames.UserRoadsCar.ToString(), userId, roadYear, search, shareKey), DataCar, 15);
                }
                if (DataBike == null)
                {
                    DataBike =
                        ac.Tracks.AsNoTracking()
                            .Where(
                                t =>
                                    t.Type == (int) RoadTypes.Bike
                                    && (roadYear == 0 || t.Date.Year == roadYear)
                                    && ((string.IsNullOrEmpty(shareKey) && t.UserId == userId)
                                        ||
                                        (!string.IsNullOrEmpty(shareKey) &&
                                         (t.User.ShareTracksBikeKey == shareKey || t.ShareKey == shareKey)))
                                    && (string.IsNullOrEmpty(search) || t.Name.Contains(search))
                            )
                            .OrderByDescending(t => t.Date)
                            .Select(RoadsView.Map)
                            .ToList();
                    if (DataBike.Any())
                    {
                        var dataBikeAll = new List<RoadsView>
                        {
                            new RoadsView
                            {
                                Id = -(int) RoadTypes.Bike,
                                Name = Resource.All,
                                UserId = DataBike.Count().ToString(),
                                Date = new DateTime(1980, 3, 3),
                                Distance = DataBike.Sum(item => item.Distance),
                                ShareKey = "+"
                            }
                        };
                        DataBike = dataBikeAll.Concat(DataBike);
                    }
                    cache.Set(cache.GetFormatedName(CacheNames.UserRoadsBike.ToString(), userId, roadYear, search, shareKey), DataBike, 15);
                }
                if (YearsList == null)
                {
                    var years = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = Resource.All,
                            Value = "0",
                            Selected = roadYear == 0
                        }
                    };
                    foreach (var elem in ac.Tracks.AsNoTracking().Where(t =>
                        ((string.IsNullOrEmpty(shareKey) && t.UserId == userId)
                         ||
                         (!string.IsNullOrEmpty(shareKey) &&
                          (((t.User.ShareTracksBikeKey == shareKey && t.Type == (int) RoadTypes.Bike) ||
                            (t.User.ShareTracksCarKey == shareKey && t.Type == (int) RoadTypes.Car) ||
                            (t.User.ShareTracksFootKey == shareKey && t.Type == (int) RoadTypes.Foot))
                           || t.ShareKey == shareKey)))
                        && (string.IsNullOrEmpty(search) || t.Name.Contains(search))
                        ).Select(t => t.Date.Year).Distinct())
                    {
                        years.Add(new SelectListItem
                        {
                            Text = elem.ToString(),
                            Value = elem.ToString(),
                            Selected = roadYear == elem
                        });
                    }
                    YearsList = years.OrderByDescending(y => y.Text);
                    cache.Set(cache.GetFormatedName(CacheNames.UserRoadsYearsList.ToString(), userId, roadYear, search, shareKey), YearsList, 15);
                }
            }
        }
    }
}
