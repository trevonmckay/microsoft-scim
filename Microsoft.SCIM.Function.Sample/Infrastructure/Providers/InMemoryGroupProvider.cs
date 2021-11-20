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

    public class InMemoryGroupProvider : ProviderBase
    {
        private readonly InMemoryStorage storage;

        public InMemoryGroupProvider()
        {
            this.storage = InMemoryStorage.Instance;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2Group> exisitingGroups = this.storage.Groups.Values;
            if
            (
                exisitingGroups.Any(
                    (Core2Group exisitingGroup) =>
                        string.Equals(exisitingGroup.DisplayName, group.DisplayName, StringComparison.Ordinal))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            string resourceIdentifier = Guid.NewGuid().ToString();
            resource.Identifier = resourceIdentifier;
            this.storage.Groups.Add(resourceIdentifier, group);

            return Task.FromResult(resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string identifier = resourceIdentifier.Identifier;

            if (this.storage.Groups.ContainsKey(identifier))
            {
                this.storage.Groups.Remove(identifier);
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
            IEnumerable<Core2Group> buffer = Enumerable.Empty<Core2Group>();
            if (queryFilter == null)
            {
                buffer = this.storage.Groups.Values;
            }
            else
            {
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
                    throw new NotSupportedException(DGSDomainIdentityManagementServiceResources.ExceptionInvalidContext);
                }

                if (queryFilter.AttributePath.Equals(AttributeNames.DisplayName))
                {
                    buffer =
                        this.storage.Groups.Values
                        .Where(
                            (Core2Group item) =>
                               string.Equals(
                                   item.DisplayName,
                                   parameters.AlternateFilters.Single().ComparisonValue,
                                   StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    throw new NotSupportedException(DGSDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate);
                }
            }

            results =
                buffer
                .Select((Core2Group item) =>
                 {
                     Core2Group bufferItem =
                     new Core2Group
                     {
                         DisplayName = item.DisplayName,
                         ExternalIdentifier = item.ExternalIdentifier,
                         Identifier = item.Identifier,
                         Members = item.Members,
                         Metadata = item.Metadata
                     };

                     if (parameters?.ExcludedAttributePaths?.Any(
                             (string excludedAttributes) =>
                                 excludedAttributes.Equals(AttributeNames.Members, StringComparison.OrdinalIgnoreCase))
                         == true)
                     {
                         bufferItem.Members = null;
                     }

                     return bufferItem;
                 })
                .Select((Core2Group item) => item as Resource)
                .ToArray();

            return Task.FromResult(results);
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2Group> exisitingGroups = this.storage.Groups.Values;
            if
            (
                exisitingGroups.Any(
                    (Core2Group exisitingUser) =>
                        string.Equals(exisitingUser.DisplayName, group.DisplayName, StringComparison.Ordinal) &&
                        !string.Equals(exisitingUser.Identifier, group.Identifier, StringComparison.OrdinalIgnoreCase))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (!this.storage.Groups.TryGetValue(group.Identifier, out Core2Group _))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.storage.Groups[group.Identifier] = group;
            Resource result = group as Resource;
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

            string identifier = parameters.ResourceIdentifier.Identifier;

            if (this.storage.Groups.ContainsKey(identifier))
            {
                if (this.storage.Groups.TryGetValue(identifier, out Core2Group group))
                {
                    Resource result = group as Resource;
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
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(DGSDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate);
            }

            PatchRequest2 patchRequest =
                patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            if (this.storage.Groups.TryGetValue(patch.ResourceIdentifier.Identifier, out Core2Group group))
            {
                group.Apply(patchRequest);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Task.CompletedTask;
        }
    }
}
