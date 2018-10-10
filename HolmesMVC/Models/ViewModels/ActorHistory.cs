﻿namespace HolmesMVC.Models.ViewModels
{
    using System;

    using HolmesMVC.Enums;

    public class ActorHistory
    {
        public ActorHistory(Appearance ap)
        {
            Airdate = ap.Episode.Airdate;
            AirdatePrecision = (DatePrecision)ap.Episode.AirdatePrecision;
            var rename = Shared.GetRename(ap);
            CharacterName = null == rename ? Shared.LongName(ap.Character) : Shared.LongName(rename);
            EpId = ap.EpisodeID;
            AdaptUrlName = ap.Episode.Season.Adaptation.UrlName;
            AdaptMediumUrlName = ap.Episode.Season.Adaptation.MediumUrlName;
            AirOrder = ap.Episode.AirOrder;
            SeasonAirOrder = ap.Episode.Season.AirOrder;
            AdaptName = ap.Episode.Season.Adaptation.Name
                        ?? Shared.DisplayName(ap.Episode.Season.Adaptation);
            EpName = Shared.DisplayName(ap.Episode);
            EpTranslation = ap.Episode.Translation;
            SeasonCode = ap.Episode.SeasonCode;
            Medium = ap.Episode.Season.Adaptation.Medium;
        }

        public DateTime Airdate { get; set; }

        public DatePrecision AirdatePrecision { get; set; }

        public string AdaptUrlName { get; set; }

        public string AdaptMediumUrlName { get; set; }

        public int AirOrder { get; set; }

        public int SeasonAirOrder { get; set; }

        public string AdaptName { get; set; }

        public string EpName { get; set; }

        public string CharacterName { get; set; }

        public string SeasonCode { get; set; }

        public int EpId { get; set; }

        public int Medium { get; set; }

        public string EpTranslation { get; set; }
    }
}
