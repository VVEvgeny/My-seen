﻿using System.Globalization;
using MySeenLib;
using MySeenWeb.Models.Tables;

namespace MySeenWeb.Models.TablesViews
{
    public class FilmsView : Films
    {
        public static FilmsView Map(Films model)
        {
            if (model == null) return new FilmsView();

            return new FilmsView
            {
                Id = model.Id,
                Name = model.Name,
                Year = model.Year,
                UserId = model.UserId,
                DateChange = UmtTime.From(model.DateChange),
                DateSee = UmtTime.From(model.DateSee),
                Genre = model.Genre,
                Rating = model.Rating,
                Shared = model.Shared
            };
        }
        public string GenreText
        {
            get { return Defaults.Genres.GetById(Genre); }
        }
        public string RatingText
        {
            get { return Defaults.Ratings.GetById(Rating); }
        }
        public string GenreVal
        {
            get { return Genre.ToString(); }
        }
        public string RatingVal
        {
            get { return Rating.ToString(); }
        }
        public string YearText
        {
            get { return Year == 0 ? "" : Year.ToString(); }
        }

        public string DateSeeText
        {
            get { return DateSee.ToString(CultureInfo.CurrentCulture); }
        }
    }
}
