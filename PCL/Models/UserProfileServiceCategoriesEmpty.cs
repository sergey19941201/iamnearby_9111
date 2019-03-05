﻿using System.Collections.Generic;

namespace PCL.Models
{
    public class UserProfileServiceCategoriesEmpty
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string avatarUrl { get; set; }
        public bool online { get; set; }
        public string myReviews { get; set; }
        public bool notifications { get; set; }
        public City city { get; set; }
        public string address { get; set; }
        public string distance { get; set; }
        public Coordinates coordinates { get; set; }
        public List<List<object>> serviceCategories { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}