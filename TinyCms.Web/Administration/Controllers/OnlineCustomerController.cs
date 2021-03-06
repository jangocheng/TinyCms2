﻿using System;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Models.Customers;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Directory;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Kendoui;

namespace TinyCms.Admin.Controllers
{
    public class OnlineCustomerController : BaseAdminController
    {
        #region Constructors

        public OnlineCustomerController(ICustomerService customerService,
            IGeoLookupService geoLookupService, IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            _customerService = customerService;
            _geoLookupService = geoLookupService;
            _dateTimeHelper = dateTimeHelper;
            _customerSettings = customerSettings;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        #endregion

        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Methods

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers =
                _customerService.GetOnlineCustomers(
                    DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                    null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new OnlineCustomerModel
                {
                    Id = x.Id,
                    CustomerInfo =
                        x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                    LastIpAddress = x.LastIpAddress,
                    Location = _geoLookupService.LookupCountryName(x.LastIpAddress),
                    LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                    LastVisitedPage = _customerSettings.StoreLastVisitedPage
                        ? x.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage)
                        : _localizationService.GetResource(
                            "Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled")
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion
    }
}