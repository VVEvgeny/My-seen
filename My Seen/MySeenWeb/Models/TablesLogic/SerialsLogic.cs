﻿using System;
using System.Linq;
using MySeenLib;
using MySeenWeb.Add_Code;
using MySeenWeb.Models.OtherViewModels;
using MySeenWeb.Models.Tables;

namespace MySeenWeb.Models.TablesLogic
{
    public class SerialsLogic : Serials
    {
        private readonly ApplicationDbContext _ac;
        public string ErrorMessage;
        private readonly ICacheService _cache;
        public SerialsLogic()
        {
            ErrorMessage = string.Empty;
            _ac = new ApplicationDbContext();
        }
        public SerialsLogic(ICacheService cache) : this()
        {
            _cache = cache;
        }
        private bool Fill(string name, string year, string season, string series, string datetime, string genre, string rating, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(datetime)) throw new Exception("Нужна дата");
                Name = name;
                Year = string.IsNullOrEmpty(year) ? 0 : Convert.ToInt32(year);
                LastSeason = string.IsNullOrEmpty(season) ? 1 : Convert.ToInt32(season);
                LastSeries = string.IsNullOrEmpty(series) ? 1 : Convert.ToInt32(series);
                DateBegin = UmtTime.To(Convert.ToDateTime(datetime));
                Genre = Convert.ToInt32(genre);                
                Rating = Convert.ToInt32(rating);
                DateChange = UmtTime.To(DateTime.Now);
                DateLast = UmtTime.To(DateTime.Now);
                UserId = userId;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
            return true;
        }
        private bool Fill(string id, string name, string year, string season, string series, string datetime, string genre, string rating, string userId)
        {
            try
            {
                Id = Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
            return Fill(name, year, season, series, datetime, genre, rating, userId);
        }
        private bool Contains()
        {
            return _ac.Serials.Any(f => f.Name == Name && f.UserId == UserId && f.Id != Id && f.Year == Year);
        }
        private bool Verify()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorMessage = Resource.EnterSerialName;
            }
            else if (Contains())
            {
                ErrorMessage = Resource.SerialNameAlreadyExists;
            }
            else return true;

            return false;
        }
        private bool Add()
        {
            try
            {
                _ac.Serials.Add(this);
                _ac.SaveChanges();
                _cache.Remove(CacheNames.UserSerials.ToString(), UserId);
            }
            catch (Exception e)
            {
                ErrorMessage = Resource.ErrorWorkWithDB + "=" + e.Message;
                return false;
            }
            return true;
        }
        private bool Update()
        {
            try
            {
                var film = _ac.Serials.First(f => f.UserId == UserId && f.Id == Id);
                film.Name = Name;
                film.Year = Year;
                if (film.LastSeason != LastSeason || film.LastSeries != LastSeries)
                {
                    film.DateLast = DateLast;
                }
                film.LastSeason = LastSeason;
                film.LastSeries = LastSeries;
                film.Genre = Genre;
                film.Rating = Rating;
                film.DateChange = DateChange;
                film.DateBegin = DateBegin;
                _cache.Remove(CacheNames.UserSerials.ToString(), UserId);
                _ac.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorMessage = Resource.ErrorWorkWithDB + "=" + e.Message;
                return false;
            }
            return true;
        }
        public bool Add(string name, string year, string season, string series, string datetime, string genre, string rating,string userId)
        {
            return Fill(name, year,season, series, datetime, genre, rating, userId) && Verify() && Add();
        }
        public bool Update(string id, string name, string year, string season, string series, string datetime, string genre, string rating, string userId)
        {
            return Fill(id, name, year, season, series, datetime, genre, rating, userId) && Verify() && Update();
        }
        public bool AddSeries(string id, string userId)
        {
            try
            {
                Id = Convert.ToInt32(id);
                if (_ac.Serials.Any(f => f.UserId == userId && f.Id == Id))
                {
                    _ac.Serials.First(f => f.UserId == userId && f.Id == Id).LastSeries++;
                    _ac.Serials.First(f => f.UserId == userId && f.Id == Id).DateLast = UmtTime.From(DateTime.Now);
                    _cache.Remove(CacheNames.UserSerials.ToString(), userId);
                    _ac.SaveChanges();
                }
                else
                {
                    ErrorMessage = Resource.NoData;
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = Resource.ErrorWorkWithDB + "=" + e.Message;
                return false;
            }
            return true;
        }
        public bool Delete(string id, string userId)
        {
            try
            {
                Id = Convert.ToInt32(id);
                if (_ac.Serials.Any(f => f.UserId == userId && f.Id == Id))
                {
                    _ac.Serials.RemoveRange(_ac.Serials.Where(f => f.UserId == userId && f.Id == Id));
                    _cache.Remove(CacheNames.UserSerials.ToString(), userId);
                    _ac.SaveChanges();
                }
                else
                {
                    ErrorMessage = Resource.NoData;
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = Resource.ErrorWorkWithDB + "=" + e.Message;
                return false;
            }
            return true;
        }
        public string GetShare(string id, string userId)
        {
            try
            {
                Id = Convert.ToInt32(id);
                if (_ac.Serials.First(f => f.UserId == userId && f.Id == Id).Shared)
                {
                    var key = _ac.Users.First(t => t.Id == userId).ShareSerialsKey;
                    return MySeenWebApi.ApiHost + MySeenWebApi.ShareSerials + key;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = Resource.ErrorWorkWithDB + "=" + e.Message;
            }
            return "-";
        }
        public string GenerateShare(string id, string userId)
        {
            var iid = Convert.ToInt32(id);
            var key = _ac.Users.First(t => t.Id == userId).ShareSerialsKey;
            _ac.Serials.First(e => e.Id == iid).Shared = true;
            _cache.Remove(CacheNames.UserSerials.ToString(), userId);
            _ac.SaveChanges();
            return MySeenWebApi.ApiHost + MySeenWebApi.ShareSerials + key;
        }
        public string DeleteShare(string id, string userId)
        {
            var iid = Convert.ToInt32(id);
            _ac.Serials.First(e => e.Id == iid && e.UserId == userId).Shared = false;
            _cache.Remove(CacheNames.UserSerials.ToString(), userId);
            _ac.SaveChanges();
            return "-";
        }
        public int GetCountShared(string key)
        {
            return _ac.Serials.Count(f => f.Shared && f.User.ShareSerialsKey == key);
        }
    }
}
