﻿namespace TinyCms.Core.Domain.Catalog
{
    /// <summary>
    ///     Represents the product sorting
    /// </summary>
    public enum PostSortingEnum
    {
        /// <summary>
        ///     Position (display order)
        /// </summary>
        Position = 0,

        /// <summary>
        ///     Name: A to Z
        /// </summary>
        NameAsc = 5,

        /// <summary>
        ///     Name: Z to A
        /// </summary>
        NameDesc = 6,

        /// <summary>
        ///     Product creation date
        /// </summary>
        CreatedOn = 15,

        /// <summary>
        ///     ViewCount
        /// </summary>
        ViewCount = 20
    }
}