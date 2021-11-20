using AirCo.AzureAD.Licensing;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.SCIM.Core.Tests
{
    public class RoleCollectionTests
    {
        [Fact]
        public void DeserializeJsonArray_Returns2Objects()
        {
            // Arrange
            const string jsonString = "[{\"value\":\"{\\\"display\\\":\\\"Primary Role\\\",\\\"value\\\":\\\"first_role\\\",\\\"primary\\\":true}\"},{\"value\":\"{\\\"display\\\":\\\"Secondary Role\\\",\\\"value\\\":\\\"second_role\\\",\\\"primary\\\":false}\"}]";

            // Act
            var roles = JsonConvert.DeserializeObject<RoleCollection>(jsonString);

            // Assert
            Assert.NotNull(roles);
            Assert.Collection(roles,
                el0 => Assert.Equal("first_role", el0.Value),
                el1 => Assert.Equal("second_role", el1.Value));
        }
    }
}
