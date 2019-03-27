using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEEIPro.Models
{
    public class ExpertInfo
    {

        public int sId { get; set; }
        public string eId { get; set; }
        public string eName { get; set; }
        public string gender { get; set; }
        public Nullable<System.DateTime> birthDay { get; set; }
        public string identityNumber { get; set; }
        public string unitporetyString { get; set; }
        public string UnitDetailsOne { get; set; }
        public string UnitDetailsTwo { get; set; }
        public string UnitDetailsThree { get; set; }
        public string academicTitles { get; set; }
        public string field { get; set; }
        public string email { get; set; }
        public string officePhone { get; set; }
        public string cellPhone { get; set; }
        public string postalAddress { get; set; }
        public string expertSources { get; set; }
        public string bestatusString { get; set; }
        public string img { get; set; }
        public string personalUrl { get; set; }
        public string Categories { get; set; }
        public string registrationForm { get; set; }
        public string appointmentBook { get; set; }
        public string SerialNum { get; set; }
        public string remark { get; set; }
        public string expertIntroduction { get; set; }
        public string expertworkingExperience { get; set; }
        public string expertAchievement { get; set; }
        public Nullable<System.DateTime> addTime { get; set; }
    }
}