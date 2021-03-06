using System.Web.Routing;
using TinyCms.Core.Plugins;

namespace TinyCms.Services.Common
{
    /// <summary>
    ///     Misc plugin interface.
    ///     It's used by the plugins that have a configuration page but don't fit any other category (such as payment or tax
    ///     plugins)
    /// </summary>
    public interface IMiscPlugin : IPlugin
    {
        /// <summary>
        ///     Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues);
    }
}