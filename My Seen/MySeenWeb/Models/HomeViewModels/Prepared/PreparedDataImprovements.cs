﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MySeenLib;
using static MySeenLib.Defaults;

namespace MySeenWeb.Models.Prepared
{
    public class PreparedDataImprovements
    {
        public IEnumerable<SelectListItem> SelectedList { get; set; }
        public IEnumerable<SelectListItem> SelectedListForAdd { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }

        public PreparedDataImprovements()
        {
            SelectedList =
                Complexes.GetAll()
                    .Select(
                        sel =>
                            new SelectListItem
                            {
                                Text = sel,
                                Value = Complexes.GetId(sel).ToString(),
                                Selected = Complexes.GetId(sel) == (int) ComplexBase.Indexes.All
                            })
                    .ToList();

            SelectedListForAdd = SelectedList.Where(l => l.Value != "0");

            TypeList = new List<SelectListItem>
            {
                new SelectListItem {Text = Resource.All, Value = "0", Selected = true},
                new SelectListItem {Text = Resource.Active, Value = "1", Selected = false},
                new SelectListItem {Text = Resource.Ended, Value = "2", Selected = false}
            };
        }
    }
}