using System.Reflection;
using ASPtestShop.Auth;
using ASPtestShop.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace ASPtestShop.Tests
{
    public class RefundAuthSchemeTests
    {
        [Fact]
        public void RefundApiController_ShouldIncludeAdminCookieAuthScheme()
        {
            var authAttribute = typeof(RefundApiController).GetCustomAttribute<AuthorizeAttribute>();

            Assert.NotNull(authAttribute);
            Assert.NotNull(authAttribute.AuthenticationSchemes);
            Assert.Contains(AdminCookieAuth.Scheme, authAttribute.AuthenticationSchemes);
        }
    }
}
