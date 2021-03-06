﻿using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Customers
{
    public class CustomerReportsModel : BaseNopModel
    {
        public BestCustomersReportModel BestCustomersByOrderTotal { get; set; }
        public BestCustomersReportModel BestCustomersByNumberOfOrders { get; set; }
    }
}