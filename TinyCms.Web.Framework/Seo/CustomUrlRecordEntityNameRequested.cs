﻿using System.Web.Routing;
using TinyCms.Services.Seo;

namespace TinyCms.Web.Framework.Seo
{
    /// <summary>
    ///     Event to handle unknow URL record entity names
    /// </summary>
    public class CustomUrlRecordEntityNameRequested
    {
        public CustomUrlRecordEntityNameRequested(RouteData routeData, UrlRecordService.UrlRecordForCaching urlRecord)
        {
            RouteData = routeData;
            UrlRecord = urlRecord;
        }

        public RouteData RouteData { get; private set; }
        public UrlRecordService.UrlRecordForCaching UrlRecord { get; private set; }
    }
}