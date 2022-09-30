using Amazon;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwsAppConfig.Microsoft.Extensions.Configuration
{
    public static class AwsAppConfigConfiguratioBuilderExtensions
    {
        public static IConfigurationBuilder AddAwsAppConfig(this IConfigurationBuilder builder)
        {
            var tempConfig = builder.Build();

            var sourceFromSingleConfig = GetSourceFromSingleConfigurations(tempConfig);

            if (sourceFromSingleConfig is null == false)
            {
                builder.Add(sourceFromSingleConfig);
                return builder;
            }

            var sourceFromMultipleConfigs = GetSourceFromMultipleConfigurations(tempConfig);

            if (sourceFromMultipleConfigs.Count() == 0)
                return builder;

            foreach (AwsAppConfigConfigurationSource source in sourceFromMultipleConfigs)
            {
                builder.Add(source);
            }

            return builder;
        }

        private static AwsAppConfigConfigurationSource GetSourceFromSingleConfigurations(IConfigurationRoot root)
        {   
            var configSection = root.GetSection("AwsAppConfigs");

            if (configSection.GetChildren().Count() == 0)
                return null;

            return CreateSourceFromConfigSection(configSection);
        }

        private static IEnumerable<AwsAppConfigConfigurationSource> GetSourceFromMultipleConfigurations(IConfigurationRoot root)
        {
            var configSection = root.GetSection("AwsAppConfigs");
            var result = new List<AwsAppConfigConfigurationSource>();

            if (configSection.GetChildren().Count() == 0)
                return result;
            
            foreach (var section in configSection.GetChildren())
            {
                result.Add(CreateSourceFromConfigSection(section));
            }

            return result;
        }

        private static AwsAppConfigConfigurationSource CreateSourceFromConfigSection(IConfigurationSection section)
        {
            string applicationName = section.GetSection("ApplicationName").Value;
            string environmentName = section.GetSection("EnvironmentName").Value;
            string configurationName = section.GetSection("ConfigurationName").Value;
            string reloadPeriodAsString = section.GetSection("ReloadPeriodInSeconds").Value;
            TimeSpan reloadPeriod = string.IsNullOrWhiteSpace(reloadPeriodAsString) ? default : TimeSpan.FromSeconds(int.Parse(reloadPeriodAsString));
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(section.GetSection("Region").Value);

            return new AwsAppConfigConfigurationSource(applicationName, configurationName, environmentName, regionEndpoint, reloadPeriod);
        }
    }
}