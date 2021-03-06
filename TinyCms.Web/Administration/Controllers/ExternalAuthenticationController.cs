﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.ExternalAuthentication;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Plugins;
using TinyCms.Services.Authentication.External;
using TinyCms.Services.Configuration;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Controllers
{
    public class ExternalAuthenticationController : BaseAdminController
    {
        #region Constructors

        public ExternalAuthenticationController(IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ISettingService settingService, IPermissionService permissionService, IPluginFinder pluginFinder)
        {
            _openAuthenticationService = openAuthenticationService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _settingService = settingService;
            _permissionService = permissionService;
            _pluginFinder = pluginFinder;
        }

        #endregion

        #region Fields

        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginFinder _pluginFinder;

        #endregion

        #region Methods

        public ActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult Methods(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var methodsModel = new List<AuthenticationMethodModel>();
            var methods = _openAuthenticationService.LoadAllExternalAuthenticationMethods();
            foreach (var method in methods)
            {
                var tmp1 = method.ToModel();
                tmp1.IsActive = method.IsMethodActive(_externalAuthenticationSettings);
                methodsModel.Add(tmp1);
            }
            methodsModel = methodsModel.ToList();
            var gridModel = new DataSourceResult
            {
                Data = methodsModel,
                Total = methodsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult MethodUpdate([Bind(Exclude = "ConfigurationRouteValues")] AuthenticationMethodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var eam = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(model.SystemName);
            if (eam.IsMethodActive(_externalAuthenticationSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(
                        eam.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_externalAuthenticationSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(
                        eam.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_externalAuthenticationSettings);
                }
            }
            var pluginDescriptor = eam.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;
            PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }

        public ActionResult ConfigureMethod(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var eam = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(systemName);
            if (eam == null)
                //No authentication method found with the specified id
                return RedirectToAction("Methods");

            var model = eam.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            eam.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
    }
}