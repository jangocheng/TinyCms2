﻿using Autofac;
using TinyCms.Core.Configuration;

namespace TinyCms.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    ///     Dependency registrar interface
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        ///     Order of this dependency registrar implementation
        /// </summary>
        int Order { get; }

        /// <summary>
        ///     Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config);
    }
}