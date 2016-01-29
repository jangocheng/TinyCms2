//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using TinyCms.Core.Domain.Customers;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     Open authentication service
    /// </summary>
    public interface IOpenAuthenticationService
    {
        /// <summary>
        ///     Load active external authentication methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Payment methods</returns>
        IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods();

        /// <summary>
        ///     Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName);

        /// <summary>
        ///     Load all external authentication methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>External authentication methods</returns>
        IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods();

        bool AccountExists(OpenAuthenticationParameters parameters);
        void AssociateExternalAccountWithUser(Customer customer, OpenAuthenticationParameters parameters);
        Customer GetUser(OpenAuthenticationParameters parameters);
        IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(Customer customer);
        void DeletExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord);
        void RemoveAssociation(OpenAuthenticationParameters parameters);
    }
}