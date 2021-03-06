﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Common;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Seo;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Common;
using TinyCms.Services.Configuration;
using TinyCms.Services.Customers;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Security;

namespace TinyCms.Admin.Controllers
{
    public class CommonController : BaseAdminController
    {
        #region Constructors

        public CommonController(
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ISearchTermService searchTermService,
            ISettingService settingService,
            HttpContextBase httpContext, CatalogSettings catalogSettings)
        {
            _customerService = customerService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _searchTermService = searchTermService;
            _settingService = settingService;
            _httpContext = httpContext;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISearchTermService _searchTermService;
        private readonly ISettingService _settingService;
        private readonly CatalogSettings _catalogSettings;
        private readonly HttpContextBase _httpContext;

        #endregion

        #region Methods

        public ActionResult SystemInfo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new SystemInfoModel();
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception)
            {
            }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception)
            {
            }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception)
            {
            }
            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.HttpHost = _webHelper.ServerVariables("HTTP_HOST");
            foreach (var key in _httpContext.Request.ServerVariables.AllKeys)
            {
                model.ServerVariables.Add(new SystemInfoModel.ServerVariableModel
                {
                    Name = key,
                    Value = _httpContext.Request.ServerVariables[key]
                });
            }
            //Environment.GetEnvironmentVariable("USERNAME");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                model.LoadedAssemblies.Add(new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName
                    //we cannot use Location property in medium trust
                    //Location = assembly.Location
                });
            }
            return View(model);
        }


        public ActionResult Warnings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new List<SystemWarningModel>();

            //store URL
            var currentStoreUrl = _settingService.GetSettingByKey<string>("StoreInformationSettings.Url");
            if (!String.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase)
                 ||
                 currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)
                    ))
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.URL.Match")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text =
                        string.Format(_localizationService.GetResource("Admin.System.Warnings.URL.NoMatch"),
                            currentStoreUrl, _webHelper.GetStoreLocation(false))
                });


            //validate write permissions (the same procedure like during installation)
            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite(_webHelper);
            foreach (var dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text =
                            string.Format(
                                _localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.Wrong"),
                                WindowsIdentity.GetCurrent().Name, dir)
                    });
                    dirPermissionsOk = false;
                }
            if (dirPermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.OK")
                });

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite(_webHelper);
            foreach (var file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text =
                            string.Format(
                                _localizationService.GetResource("Admin.System.Warnings.FilePermission.Wrong"),
                                WindowsIdentity.GetCurrent().Name, file)
                    });
                    filePermissionsOk = false;
                }
            if (filePermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.FilePermission.OK")
                });

            //machine key
            try
            {
                var machineKeySection = ConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
                var machineKeySpecified = machineKeySection != null &&
                                          !String.IsNullOrEmpty(machineKeySection.DecryptionKey) &&
                                          !machineKeySection.DecryptionKey.StartsWith("AutoGenerate",
                                              StringComparison.InvariantCultureIgnoreCase);

                if (!machineKeySpecified)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = _localizationService.GetResource("Admin.System.Warnings.MachineKey.NotSpecified")
                    });
                }
                else
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Pass,
                        Text = _localizationService.GetResource("Admin.System.Warnings.MachineKey.Specified")
                    });
                }
            }
            catch (Exception exc)
            {
                LogException(exc);
            }

            return View(model);
        }


        public ActionResult Maintenance()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new MaintenanceModel();
            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public ActionResult MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = (model.DeleteGuests.StartDate == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.DeleteGuests.EndDate == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, _dateTimeHelper.CurrentTimeZone)
                        .AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = _customerService.DeleteGuestCustomers(startDateValue,
                endDateValue);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public ActionResult MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = (model.DeleteExportedFiles.StartDate == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value,
                        _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.DeleteExportedFiles.EndDate == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value,
                        _dateTimeHelper.CurrentTimeZone).AddDays(1);


            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;
            var path = Path.Combine(Request.PhysicalApplicationPath, "content\\files\\exportimport");
            foreach (var fullPath in Directory.GetFiles(path))
            {
                try
                {
                    var fileName = Path.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = new FileInfo(fullPath);
                    if ((!startDateValue.HasValue || startDateValue.Value < info.CreationTimeUtc) &&
                        (!endDateValue.HasValue || info.CreationTimeUtc < endDateValue.Value))
                    {
                        System.IO.File.Delete(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            return View(model);
        }


        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService
                .GetAllLanguages()
                .Select(x => x.ToModel())
                .ToList();
            return PartialView(model);
        }

        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new {area = "Admin"});
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            return Redirect(returnUrl);
        }


        public ActionResult ClearCache(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            cacheManager.Clear();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            return Redirect(returnUrl);
        }


        public ActionResult RestartApplication(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            return Redirect(returnUrl);
        }


        public ActionResult SeNames()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new UrlRecordListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SeNames(DataSourceRequest command, UrlRecordListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var urlRecords = _urlRecordService.GetAllUrlRecords(model.SeName, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = urlRecords.Select(x =>
                {
                    //language
                    string languageName;
                    if (x.LanguageId == 0)
                    {
                        languageName = _localizationService.GetResource("Admin.System.SeNames.Language.Standard");
                    }
                    else
                    {
                        var language = _languageService.GetLanguageById(x.LanguageId);
                        languageName = language != null ? language.Name : "Unknown";
                    }

                    //details URL
                    var detailsUrl = "";
                    var entityName = x.EntityName != null ? x.EntityName.ToLowerInvariant() : "";
                    switch (entityName)
                    {
                        case "category":
                            detailsUrl = Url.Action("Edit", "Category", new {id = x.EntityId});
                            break;
                        case "post":
                            detailsUrl = Url.Action("Edit", "Post", new {id = x.EntityId});
                            break;
                        case "topic":
                            detailsUrl = Url.Action("Edit", "Topic", new {id = x.EntityId});
                            break;
                        default:
                            break;
                    }

                    return new UrlRecordModel
                    {
                        Id = x.Id,
                        Name = x.Slug,
                        EntityId = x.EntityId,
                        EntityName = x.EntityName,
                        IsActive = x.IsActive,
                        Language = languageName,
                        DetailsUrl = detailsUrl
                    };
                }),
                Total = urlRecords.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var urlRecords = new List<UrlRecord>();
                foreach (var id in selectedIds)
                {
                    var urlRecord = _urlRecordService.GetUrlRecordById(id);
                    if (urlRecord != null)
                        urlRecords.Add(urlRecord);
                }
                foreach (var urlRecord in urlRecords)
                    _urlRecordService.DeleteUrlRecord(urlRecord);
            }

            return Json(new {Result = true});
        }


        [ChildActionOnly]
        public ActionResult PopularSearchTermsReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return Content("");

            return PartialView();
        }

        [HttpPost]
        public ActionResult PopularSearchTermsReport(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var searchTermRecordLines = _searchTermService.GetStats(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = searchTermRecordLines.Select(x => new SearchTermReportLineModel
                {
                    Keyword = x.Keyword,
                    Count = x.Count
                }),
                Total = searchTermRecordLines.TotalCount
            };
            return Json(gridModel);
        }

        //action displaying notification (warning) to a store owner that "ACL rules" feature is ignored
        [ChildActionOnly]
        public ActionResult AclDisabledWarning()
        {
            //default setting
            var enabled = _catalogSettings.IgnoreAcl;

            //This setting is disabled. No warnings.
            if (!enabled)
                return Content("");

            return PartialView();
        }

        #endregion
    }
}