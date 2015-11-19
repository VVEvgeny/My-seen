﻿using System;
using System.Linq;
using MySeenWeb.Models.Database;
using MySeenWeb.Models.Database.Tables;

namespace MySeenWeb.Models.Tools
{
    public static class LogSave
    {
        public static void Save(string userId, string ipAdress, string userAgent, string pageName)
        {
            Save(userId, ipAdress, userAgent, pageName, string.Empty);
        }

        public static void Save(string userId, string ipAdress, string userAgent, string pageName, string addData)
        {
            var ac = new ApplicationDbContext();
            var date = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

            if (
                !ac.Logs.Any(
                    l =>
                        l.IPAdress == ipAdress && l.UserAgent == userAgent && l.UserId == userId && l.OnlyDate == date &&
                        l.PageName == pageName))
            {
                ac.Logs.Add(new Logs
                {
                    IPAdress = ipAdress,
                    UserId = userId,
                    UserAgent = userAgent,
                    OnlyDate = date,
                    DateFirst = DateTime.Now,
                    DateLast = DateTime.Now,
                    PageName = pageName,
                    AddData = addData,
                    Count = 1
                });
            }
            else
            {
                var log =
                    ac.Logs.First(
                        l =>
                            l.IPAdress == ipAdress && l.UserAgent == userAgent && l.UserId == userId &&
                            l.OnlyDate == date && l.PageName == pageName);
                log.DateLast = DateTime.Now;
                log.Count++;
                if (!string.IsNullOrEmpty(addData)) log.AddData += "!%!" + addData;
            }
            ac.SaveChanges();
        }
    }
}