﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using System;
    using System.Collections.Generic;

    public partial class line
    {

        //public line()
        //{
        //    Type t = typeof(line);
        //    foreach (var property in t.GetProperties())
        //    {
        //        this.
        //    }

        //}


        public long idFeedLine { get; set; }

        public Nullable<int> rot { get; set; }

        public byte period_id { get; set; }
        public string period { get; set; }
        public Nullable<int> event_id { get; set; }
        public Nullable<short> sportsbook { get; set; }
        public string description { get; set; }
        public byte league_id { get; set; }
        public byte sport_id { get; set; }
        public Nullable<int> away_team_id { get; set; }
        public Nullable<int> home_team_id { get; set; }
        public Nullable<int> draw_price { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<int> home_rot { get; set; }
        public Nullable<float> total_total { get; set; }
        public Nullable<int> over_price { get; set; }
        public Nullable<int> under_price { get; set; }
        public Nullable<float> total { get; set; }
        public Nullable<System.DateTime> timeReceived { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        public Nullable<int> away_money { get; set; }
        public Nullable<int> ml_home_price { get; set; }
        public Nullable<int> ml_away_price { get; set; }
        public Nullable<int> ps_away_price { get; set; }
        public Nullable<double> ps_home_price { get; set; }
        public Nullable<float> ps_away_spread { get; set; }
        public Nullable<float> ps_home_spread { get; set; }


    }
}