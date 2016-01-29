﻿using System.Collections.Generic;

namespace TinyCms.Services.Customers
{
    /// <summary>
    ///     Change password result
    /// </summary>
    public class ChangePasswordResult
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        public ChangePasswordResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        ///     Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (Errors.Count == 0); }
        }

        /// <summary>
        ///     Errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        ///     Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}