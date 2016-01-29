﻿using System.Collections.Generic;
using TinyCms.Admin.Models.Customers;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Security
{
    public class PermissionMappingModel : BaseNopModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableCustomerRoles = new List<CustomerRoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }

        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}