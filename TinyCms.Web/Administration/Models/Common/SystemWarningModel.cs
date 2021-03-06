﻿using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Common
{
    public class SystemWarningModel : BaseNopModel
    {
        public SystemWarningLevel Level { get; set; }
        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}