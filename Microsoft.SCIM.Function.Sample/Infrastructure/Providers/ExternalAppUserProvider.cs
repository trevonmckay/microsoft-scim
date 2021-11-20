// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM.Sample.Infrastructure.Providers
{
    using Microsoft.SCIM;
    using Microsoft.SCIM.Function.Sample.Resources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ExternalAppUserProvider : ProviderBase
    {
        private readonly InMemoryStorage _providerStorage;

        public ExternalAppUserProvider()
        {
            this._providerStorage = InMemoryStorage.Instance;
            CreateFakeUsers();
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2EnterpriseUser> exisitingUsers = this._providerStorage.Users.Values;
            if
            (
                exisitingUsers.Any(
                    (Core2EnterpriseUser exisitingUser) =>
                        string.Equals(exisitingUser.UserName, user.UserName, StringComparison.Ordinal))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            string resourceIdentifier = Guid.NewGuid().ToString();
            resource.Identifier = resourceIdentifier;
            this._providerStorage.Users.Add(resourceIdentifier, user);

            return Task.FromResult(resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string identifier = resourceIdentifier.Identifier;

            if (this._providerStorage.Users.ContainsKey(identifier))
            {
                this._providerStorage.Users.Remove(identifier);
            }

            return Task.CompletedTask;
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            Resource[] results;
            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();
            if (queryFilter == null)
            {
                IEnumerable<Core2EnterpriseUser> allUsers = this._providerStorage.Users.Values;
                results =
                    allUsers.Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (queryFilter.FilterOperator != ComparisonOperator.Equals)
            {
                throw new NotSupportedException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.UserName, StringComparison.OrdinalIgnoreCase))
            {
                IEnumerable<Core2EnterpriseUser> allUsers = this._providerStorage.Users.Values;
                results =
                    allUsers.Where(
                        (Core2EnterpriseUser item) =>
                           string.Equals(
                                item.UserName,
                               parameters.AlternateFilters.Single().ComparisonValue,
                               StringComparison.OrdinalIgnoreCase))
                               .Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                IEnumerable<Core2EnterpriseUser> allUsers = this._providerStorage.Users.Values;
                results =
                    allUsers.Where(
                        (Core2EnterpriseUser item) =>
                           string.Equals(
                                item.ExternalIdentifier,
                               parameters.AlternateFilters.Single().ComparisonValue,
                               StringComparison.OrdinalIgnoreCase))
                               .Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.DisplayName, StringComparison.OrdinalIgnoreCase))
            {
                IEnumerable<Core2EnterpriseUser> allUsers = this._providerStorage.Users.Values;
                results =
                    allUsers.Where(
                        (Core2EnterpriseUser item) =>
                           string.Equals(
                               item.DisplayName,
                               parameters.AlternateFilters.Single().ComparisonValue,
                               StringComparison.OrdinalIgnoreCase))
                               .Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }


            throw new NotSupportedException(DGSDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate);
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2EnterpriseUser> exisitingUsers = this._providerStorage.Users.Values;
            if
            (
                exisitingUsers.Any(
                    (Core2EnterpriseUser exisitingUser) =>
                        string.Equals(exisitingUser.UserName, user.UserName, StringComparison.Ordinal) &&
                        !string.Equals(exisitingUser.Identifier, user.Identifier, StringComparison.OrdinalIgnoreCase))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (!this._providerStorage.Users.TryGetValue(user.Identifier, out Core2EnterpriseUser _))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this._providerStorage.Users[user.Identifier] = user;
            Resource result = user as Resource;
            return Task.FromResult(result);
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Resource result = null;
            string identifier = parameters.ResourceIdentifier.Identifier;

            if (this._providerStorage.Users.ContainsKey(identifier))
            {
                if (this._providerStorage.Users.TryGetValue(identifier, out Core2EnterpriseUser user))
                {
                    result = user as Resource;
                    return Task.FromResult(result);
                }
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidIdentifier);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidIdentifier);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidIdentifier);
            }

            PatchRequest2 patchRequest =
                patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            if (this._providerStorage.Users.TryGetValue(patch.ResourceIdentifier.Identifier, out Core2EnterpriseUser user))
            {
                user.Apply(patchRequest);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Task.CompletedTask;
        }

        private void CreateFakeUsers()
        {
            Core2EnterpriseUser firstUser = new Core2EnterpriseUser
            {
                Active = true,
                Name = new Name
                {
                    GivenName = "First",
                    FamilyName = "User"
                },
                DisplayName = "First User",
                UserName = "firstuser@xyz.com",
                Identifier = "a8d9bd4e-d608-4595-87d1-9f69b236bdc5",
                ElectronicMailAddresses = new List<ElectronicMailAddress>()
                {
                    new ElectronicMailAddress
                    {
                        Primary = true,
                        Value = "firstuser@xyz.com",
                        ItemType = "work"
                    }
                },
                ExternalIdentifier = "0001"
            };

            Core2EnterpriseUser secondUser = new Core2EnterpriseUser
            {
                Active = true,
                Name = new Name()
                {
                    GivenName = "Second",
                    FamilyName = "User"
                },
                DisplayName = "Second User",
                UserName = "test.user@okta.local",
                Identifier = "c30c7c7c-08e4-4fa8-b531-feba885a9d90",
                ElectronicMailAddresses = new List<ElectronicMailAddress>()
                {
                    new ElectronicMailAddress
                    {
                        Primary = true,
                        Value = "seconduser@xyz.com",
                        ItemType = "work"
                    }
                },
                ExternalIdentifier = "0002"
            };

            this._providerStorage.Users.Add(firstUser.Identifier, firstUser);
            this._providerStorage.Users.Add(secondUser.Identifier, secondUser);
        }
    }
}
