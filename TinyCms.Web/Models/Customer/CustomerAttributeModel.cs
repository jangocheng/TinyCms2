﻿using System.Collections.Generic;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Customer
{
    public class CustomerAttributeModel : BaseNopEntityModel
    {
        public CustomerAttributeModel()
        {
            Values = new List<CustomerAttributeValueModel>();
        }

        public string Name { get; set; }
        public bool IsRequired { get; set; }

        /// <summary>
        ///     Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }
        public IList<CustomerAttributeValueModel> Values { get; set; }
    }

    public class CustomerAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public bool IsPreSelected { get; set; }
    }
}