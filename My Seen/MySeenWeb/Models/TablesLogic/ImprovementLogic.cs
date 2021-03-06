﻿using System;
using System.Linq;
using MySeenResources;
using MySeenWeb.Models.OtherViewModels;
using MySeenWeb.Models.Tables;
using MySeenWeb.Models.TablesLogic.Base;
using static System.Convert;
using static MySeenLib.UmtTime;

namespace MySeenWeb.Models.TablesLogic
{
    public class ImprovementLogic : Bugs, IBaseLogic
    {
        private readonly ApplicationDbContext _ac;
        public string ErrorMessage;

        public ImprovementLogic()
        {
            ErrorMessage = string.Empty;
            _ac = new ApplicationDbContext();
        }

        public string GetError()
        {
            return ErrorMessage;
        }

        public bool Delete(string id, string userId)
        {
            try
            {
                Id = ToInt32(id);
                _ac.Bugs.RemoveRange(_ac.Bugs.Where(b => b.Id == Id));
                _ac.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorMessage = $"{Resource.ErrorWorkWithDB} = {e.Message}";
                return false;
            }
            return true;
        }

        private bool Fill(string text, string complex, string userId)
        {
            try
            {
                Text = text;
                DateFound = To(DateTime.Now);
                Complex = ToInt32(complex);
                UserId = userId;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
            return true;
        }

        private bool Fill(string id, string text, string complex, string userId)
        {
            try
            {
                Id = ToInt32(id);
                Text = text;
                Complex = ToInt32(complex);
                UserId = userId;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
            return true;
        }

        private bool Contains()
        {
            return _ac.Bugs.Any(f => f.Text == Text && f.Id != Id);
        }

        private bool Verify()
        {
            if (Id == 0 && string.IsNullOrEmpty(Text)) ErrorMessage = Resource.DescToShort;
            else if (Contains()) ErrorMessage = Resource.BugAlreadyExists;
            else return true;

            return false;
        }

        private bool Add()
        {
            try
            {
                _ac.Bugs.Add(this);
                _ac.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorMessage = $"{Resource.ErrorWorkWithDB} = {e.Message}";
                return false;
            }
            return true;
        }

        private bool Update()
        {
            try
            {
                var elem = _ac.Bugs.First(f => f.Id == Id);
                elem.Text = Text;
                elem.Complex = Complex;
                _ac.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorMessage = $"{Resource.ErrorWorkWithDB} = {e.Message}";
                return false;
            }
            return true;
        }

        public bool Add(string text, string complex, string userId)
        {
            return Fill(text, complex, userId) && Verify() && Add();
        }

        public bool Update(string id, string text, string complex, string userId)
        {
            return Fill(id, text, complex, userId) && Verify() && Update();
        }

        public bool End(string id, string textEnd, string version)
        {
            try
            {
                Id = ToInt32(id);

                var elem = _ac.Bugs.First(f => f.Id == Id);
                elem.TextEnd = textEnd;
                elem.DateEnd = DateTime.Now;
                elem.Version = ToInt32(version);

                _ac.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorMessage = $"{Resource.ErrorWorkWithDB} = {e.Message}";
                return false;
            }
            return true;
        }
    }
}