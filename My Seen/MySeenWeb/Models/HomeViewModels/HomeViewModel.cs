﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySeenWeb.Models.Meta;
using MySeenWeb.Models.TablesLogic;
using static MySeenLib.Defaults;

namespace MySeenWeb.Models
{
    public class HomeViewModel
    {
        public bool Markers { get; set; }
        public MetaBase Meta { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
        public bool HaveLanguage { get; set; }
        public Style.Style Style { get; set; }

        public HomeViewModel(string userId, int markers, int theme, bool animationEnabled, HttpRequestBase request)
        {
            Markers = markers == (int) EnabledDisabledBase.Indexes.Enabled;
            Meta = MetaBase.Create(request);

            Style.Theme = theme;
            Style.AnimationEnabled = animationEnabled;

            var logic = new UserRolesLogic();
            UserRoles = logic.GetRoles(userId);

            if (request.UserLanguages != null && request.UserLanguages.Any())
                HaveLanguage =
                    request.UserLanguages.Any(lang => lang.ToLower().Contains("ru") || lang.ToLower().Contains("en"));
            else HaveLanguage = false;
        }
    }
}