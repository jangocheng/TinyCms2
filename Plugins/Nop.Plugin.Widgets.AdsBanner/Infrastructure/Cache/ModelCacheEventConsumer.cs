﻿using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Configuration;
using TinyCms.Core.Events;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Events;

namespace Nop.Plugin.Widgets.AdsBanner.Infrastructure.Cache
{
    /// <summary>
    ///     Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public class ModelCacheEventConsumer :
        IConsumer<EntityInserted<Setting>>,
        IConsumer<EntityUpdated<Setting>>,
        IConsumer<EntityDeleted<Setting>>
    {
        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : picture id
        /// </remarks>
        public const string PICTURE_URL_MODEL_KEY = "Nop.plugins.widgets.nivosrlider.pictureurl-{0}";

        public const string PICTURE_URL_PATTERN_KEY = "Nop.plugins.widgets.nivosrlider";
        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            _cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        public void HandleEvent(EntityDeleted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }

        public void HandleEvent(EntityInserted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }
    }
}