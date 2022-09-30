using Amazon;
using Microsoft.Extensions.Configuration;
using System;

namespace AwsAppConfig.Microsoft.Extensions.Configuration
{
    public class AwsAppConfigFeatureManagementConfigurationSource : IConfigurationSource
    {
        public AwsAppConfigFeatureManagementConfigurationSource(string applicationName, string configurationName, string environmentName, RegionEndpoint regionEndpoint) :
            this(applicationName, configurationName, environmentName, regionEndpoint, default, null)
        {
        }
        public AwsAppConfigFeatureManagementConfigurationSource(string applicationName, string configurationName, string environmentName, RegionEndpoint regionEndpoint, TimeSpan reloadPeriod) :
            this(applicationName, configurationName, environmentName, regionEndpoint, reloadPeriod, null)
        {
        }

        public AwsAppConfigFeatureManagementConfigurationSource(string applicationName, string configurationName, string environmentName, RegionEndpoint regionEndpoint, TimeSpan reloadPeriod, TimeSpan? loadTimeout)
        {
            ApplicationName = applicationName;
            ConfigurationName = configurationName;
            EnvironmentName = environmentName;
            RegionEndpoint = regionEndpoint;
            LoadTimeout = loadTimeout ?? TimeSpan.FromSeconds(900);
            ReloadPeriod = reloadPeriod;
        }

        public string ApplicationName { get; private set; }
        public string ConfigurationName { get; private set; }
        public string EnvironmentName { get; private set; }
        public TimeSpan LoadTimeout { get; private set; }
        public TimeSpan ReloadPeriod { get; private set; }
        public RegionEndpoint RegionEndpoint { get; private set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsAppConfigFeatureManagementConfigurationProvider(this);
        }
    }
}

