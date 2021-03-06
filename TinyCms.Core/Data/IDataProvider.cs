﻿using System.Data.Common;

namespace TinyCms.Core.Data
{
    /// <summary>
    ///     Data provider interface
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        ///     A value indicating whether this data provider supports stored procedures
        /// </summary>
        bool StoredProceduredSupported { get; }

        /// <summary>
        ///     Initialize connection factory
        /// </summary>
        void InitConnectionFactory();

        /// <summary>
        ///     Set database initializer
        /// </summary>
        void SetDatabaseInitializer();

        /// <summary>
        ///     Initialize database
        /// </summary>
        void InitDatabase();

        /// <summary>
        ///     Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();
    }
}