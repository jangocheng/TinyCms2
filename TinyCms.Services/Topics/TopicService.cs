using System;
using System.Collections.Generic;
using System.Linq;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Security;
using TinyCms.Core.Domain.Topics;
using TinyCms.Services.Customers;
using TinyCms.Services.Events;

namespace TinyCms.Services.Topics
{
    /// <summary>
    ///     Topic service
    /// </summary>
    public class TopicService : ITopicService
    {
        #region Ctor

        public TopicService(IRepository<Topic> topicRepository,
            IWorkContext workContext,
            IRepository<AclRecord> aclRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            _topicRepository = topicRepository;
            _workContext = workContext;
            _aclRepository = aclRepository;
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : store ID
        ///     {1} : ignore ACL?
        /// </remarks>
        private const string TOPICS_ALL_KEY = "Nop.topics.all-{0}";

        /// <summary>
        ///     Key for caching
        /// </summary>
        /// <remarks>
        ///     {0} : topic ID
        /// </remarks>
        private const string TOPICS_BY_ID_KEY = "Nop.topics.id-{0}";

        /// <summary>
        ///     Key pattern to clear cache
        /// </summary>
        private const string TOPICS_PATTERN_KEY = "Nop.topics.";

        #endregion

        #region Fields

        private readonly IRepository<Topic> _topicRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Methods

        /// <summary>
        ///     Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Delete(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(topic);
        }

        /// <summary>
        ///     Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            var key = string.Format(TOPICS_BY_ID_KEY, topicId);
            return _cacheManager.Get(key, () => _topicRepository.GetById(topicId));
        }

        /// <summary>
        ///     Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicBySystemName(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                return null;

            var query = _topicRepository.Table;
            query = query.Where(t => t.SystemName == systemName);
            query = query.OrderBy(t => t.Id);
            var topics = query.ToList();
            return topics.FirstOrDefault();
        }

        /// <summary>
        ///     Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(bool ignorAcl = false)
        {
            var key = string.Format(TOPICS_ALL_KEY, ignorAcl);
            return _cacheManager.Get(key, () =>
            {
                var query = _topicRepository.Table;
                query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);

                if ((!_catalogSettings.IgnoreStoreLimitations) ||
                    (!ignorAcl && !_catalogSettings.IgnoreAcl))
                {
                    if (!ignorAcl && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                            join acl in _aclRepository.Table
                                on new {c1 = c.Id, c2 = "Topic"} equals new {c1 = acl.EntityId, c2 = acl.EntityName}
                                into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                    }


                    //only distinct topics (group by ID)
                    query = from t in query
                        group t by t.Id
                        into tGroup
                        orderby tGroup.Key
                        select tGroup.FirstOrDefault();
                    query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
                }

                return query.ToList();
            });
        }

        /// <summary>
        ///     Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void InsertTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Insert(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(topic);
        }

        /// <summary>
        ///     Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Update(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}