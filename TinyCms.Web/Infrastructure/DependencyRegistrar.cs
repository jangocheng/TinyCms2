using Autofac;
using Autofac.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Configuration;
using TinyCms.Core.Infrastructure;
using TinyCms.Core.Infrastructure.DependencyManagement;
using TinyCms.Web.Controllers;

namespace TinyCms.Web.Infrastructure
{
    /// <summary>
    ///     Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        ///     Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //we cache presentation models between requests
            builder.RegisterType<CommonController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }

        /// <summary>
        ///     Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}