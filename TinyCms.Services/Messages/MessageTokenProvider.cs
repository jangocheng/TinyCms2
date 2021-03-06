﻿using System;
using System.Collections.Generic;
using System.Web;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Messages;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Events;
using TinyCms.Services.Localization;

namespace TinyCms.Services.Messages
{
    public class MessageTokenProvider : IMessageTokenProvider
    {
        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            IEventPublisher eventPublisher)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _workContext = workContext;

            _templatesSettings = templatesSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        protected virtual string GetStoreUrl(int storeId = 0, bool useSsl = false)
        {
            var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();

            return useSsl ? storeSettings.SecureUrl : storeSettings.Url;
        }

        #endregion

        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        private readonly MessageTemplatesSettings _templatesSettings;

        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Methods

        public virtual void AddStoreTokens(IList<Token> tokens, StoreInformationSettings store,
            EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            tokens.Add(new Token("Store.Name", store.Name));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            //event notification
            //_eventPublisher.EntityTokensAdded(store, tokens);
        }

        public void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", customer.GetFullName()));
            tokens.Add(new Token("Customer.FirstName",
                customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)));
            tokens.Add(new Token("Customer.LastName",
                customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName)));
            tokens.Add(new Token("Customer.VatNumber",
                customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber)));


            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var passwordRecoveryUrl = string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", GetStoreUrl(),
                customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken),
                HttpUtility.UrlEncode(customer.Email));
            var accountActivationUrl = string.Format("{0}customer/activation?token={1}&email={2}", GetStoreUrl(),
                customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken),
                HttpUtility.UrlEncode(customer.Email));
            var wishlistUrl = string.Format("{0}wishlist/{1}", GetStoreUrl(), customer.CustomerGuid);
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfCampaignAllowedTokens()
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>
            {
                "%Store.Name%",
                "%Store.URL%",
                "%Store.Email%",
                "%Store.CompanyName%",
                "%Store.CompanyAddress%",
                "%Store.CompanyPhoneNumber%",
                "%Customer.Email%",
                "%Customer.Username%",
                "%Customer.FullName%",
                "%Customer.FirstName%",
                "%Customer.LastName%",
                "%Customer.PasswordRecoveryURL%",
                "%Customer.AccountActivationURL%",
                "%Post.ID%",
                "%Post.Name%",
                "%Post.ShortDescription%",
                "%Post.PostURLForCustomer%"
            };
            return allowedTokens.ToArray();
        }

        #endregion
    }
}