using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketplaceQuoteModule.Core;

public static class ModuleConstants
{
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
}
