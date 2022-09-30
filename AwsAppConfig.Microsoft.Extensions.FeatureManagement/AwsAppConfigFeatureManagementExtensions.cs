using Amazon;
using AwsAppConfig.Microsoft.Extensions.FeatureManagement.Filters.Usernames;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwsAppConfig.Microsoft.Extensions.Configuration
{
    public static class AwsAppConfigFeatureManagementExtensions
    {
        public static IConfigurationBuilder AddAwsAppConfigFeatureFlags(this IConfigurationBuilder builder)
        {
            var tempConfig = builder.Build();

            var sourceFromSingleConfig = GetSourceFromSingleConfigurations(tempConfig);

            if (sourceFromSingleConfig is null == false)
                builder.Add(sourceFromSingleConfig);

            var sourceFromMultipleConfigs = GetSourceFromMultipleConfigurations(tempConfig);

            if (sourceFromMultipleConfigs.Count() == 0)
                return builder;

            foreach (AwsAppConfigFeatureManagementConfigurationSource source in sourceFromMultipleConfigs)
            {
                builder.Add(source);
            }

            return builder;
        }

        public static IFeatureManagementBuilder AddAwsAppConfigFeatureFlagsCustomFilters(this IFeatureManagementBuilder builder)
        {
            return builder
                .AddFeatureFilter<UsernameFilter>()
                .AddFeatureFilter<UsernameContainsFilter>();
        }

        private static AwsAppConfigFeatureManagementConfigurationSource GetSourceFromSingleConfigurations(IConfigurationRoot root)
        {
            var configSection = root.GetSection("AwsAppConfigFeatureFlag");

            if (configSection.GetChildren().Count() == 0)
                return null;

            return CreateSourceFromConfigSection(configSection);
        }

        private static IEnumerable<AwsAppConfigFeatureManagementConfigurationSource> GetSourceFromMultipleConfigurations(IConfigurationRoot root)
        {
            var configSection = root.GetSection("AwsAppConfigFeatureFlags");
            var result = new List<AwsAppConfigFeatureManagementConfigurationSource>();

            if (configSection.GetChildren().Count() == 0)
                return result;

            foreach (var section in configSection.GetChildren())
            {
                result.Add(CreateSourceFromConfigSection(section));
            }

            return result;
        }

        private static AwsAppConfigFeatureManagementConfigurationSource CreateSourceFromConfigSection(IConfigurationSection section)
        {
            string applicationName = section.GetSection("ApplicationName").Value;
            string environmentName = section.GetSection("EnvironmentName").Value;
            string configurationName = section.GetSection("ConfigurationName").Value;
            string reloadPeriodAsString = section.GetSection("ReloadPeriodInSeconds").Value;
            TimeSpan reloadPeriod = string.IsNullOrWhiteSpace(reloadPeriodAsString) ? default : TimeSpan.FromSeconds(int.Parse(reloadPeriodAsString));
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(section.GetSection("Region").Value);

            return new AwsAppConfigFeatureManagementConfigurationSource(applicationName, configurationName, environmentName, regionEndpoint, reloadPeriod);
        }
    }
}