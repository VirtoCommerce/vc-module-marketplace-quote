using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Security;
using VcmpQuoteModule = VirtoCommerce.MarketplaceQuoteModule.Core;
using VendorModule = VirtoCommerce.MarketplaceVendorModule.Core;

namespace VirtoCommerce.MarketplaceQuoteModule.Web.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseModuleAuthorization(this IApplicationBuilder appBuilder)
    {
        using var serviceScope = appBuilder.ApplicationServices.CreateScope();

        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        SavePredefinedRolesAsync(roleManager).GetAwaiter().GetResult();

        return appBuilder;
    }

    private static async Task SavePredefinedRolesAsync(RoleManager<Role> roleManager)
    {
        foreach (var vendorModuleRole in VendorModule.ModuleConstants.Security.Roles.AllRoles)
        {
            var existingVendorModuleRole = await roleManager.FindByIdAsync(vendorModuleRole.Id);
            var quoteModuleRole = VcmpQuoteModule.ModuleConstants.Security.Roles.AllRoles.Where(x => x.Id == vendorModuleRole.Id).FirstOrDefault();

            if (existingVendorModuleRole != null)
            {
                vendorModuleRole.Permissions = existingVendorModuleRole.Permissions.Concat(vendorModuleRole.Permissions).Distinct().ToList();
                if (quoteModuleRole != null)
                {
                    vendorModuleRole.Permissions = vendorModuleRole.Permissions.Concat(quoteModuleRole.Permissions).Distinct().ToList();
                }
                await roleManager.UpdateAsync(vendorModuleRole);
            }
            else
            {
                if (quoteModuleRole != null)
                {
                    vendorModuleRole.Permissions = vendorModuleRole.Permissions.Concat(quoteModuleRole.Permissions).Distinct().ToList();
                }
                await roleManager.CreateAsync(vendorModuleRole);
            }
        }
    }
}
