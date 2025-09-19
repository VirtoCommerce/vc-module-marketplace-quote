using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketplaceQuoteModule.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Roles
        {
            public static readonly Role Operator = new()
            {
                Id = "vcmp-operator-role",
                Permissions = new[]
                {
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Access,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Read,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Quotes", Name = x })
                .ToList()
            };

            public static readonly Role VendorOwner = new()
            {
                Id = "vcmp-owner-role",
                Permissions = new[]
                {
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Access,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Read,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Quotes", Name = x })
                .ToList()
            };

            public static readonly Role VendorAdmin = new()
            {
                Id = "vcmp-admin-role",
                Permissions = new[]
                {
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Access,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Read,
                    QuoteModule.Core.ModuleConstants.Security.Permissions.Update,
                }
                .Select(x => new Permission { GroupName = "Quotes", Name = x })
                .ToList()
            };

            public static Role[] AllRoles = { Operator, VendorOwner, VendorAdmin };

        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor QuoteSplitEnabled { get; } = new()
            {
                Name = "Quotes.SplitEnabled",
                GroupName = "Quotes|General",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return QuoteSplitEnabled;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }

    public static class StateMachineObjectType
    {
        public const string QuoteRequest = "VirtoCommerce.QuoteModule.Core.Models.QuoteRequest";
    }
}
