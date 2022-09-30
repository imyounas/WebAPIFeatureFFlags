using Amazon;
using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AwsAppConfig.Microsoft.Extensions.Configuration
{
    public class AwsAppConfigFeatureManagementConfigurationProvider : ConfigurationProvider
    {
        private readonly IAmazonAppConfigData _appConfigDataClient;
        private readonly string _environmentName;
        private readonly string _applicationName;
        private readonly string _configurationName;
        private readonly TimeSpan _loadTimeout;

        private TimeSpan _reloadPeriod;
        private string _nextPollConfigurationToken;


        public AwsAppConfigFeatureManagementConfigurationProvider(AwsAppConfigFeatureManagementConfigurationSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _appConfigDataClient = new AmazonAppConfigDataClient(new AmazonAppConfigDataConfig
            {
                RegionEndpoint = source.RegionEndpoint ?? RegionEndpoint.EUWest1
            });

            _environmentName = source.EnvironmentName;
            _applicationName = source.ApplicationName;
            _configurationName = source.ConfigurationName;
            _reloadPeriod = source.ReloadPeriod;
            _loadTimeout = source.LoadTimeout;

            ChangeToken.OnChange(() =>
            {
                if (_reloadPeriod == default(TimeSpan))
                    return null;

                CancellationTokenSource cts = new CancellationTokenSource(_reloadPeriod);
                CancellationChangeToken cancellationChangeToken = new CancellationChangeToken(cts.Token);
                return cancellationChangeToken;
            }, async () => { await LoadAsync(); });
        }

        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(_loadTimeout))
            {
                Dictionary<string, string> kvPairs = await GetConfigurationsAsync(cts.Token);

                if (kvPairs != null && kvPairs.Count > 0)
                {
                    Data = kvPairs;
                    OnReload();
                };
            }
        }


        private async Task<Dictionary<string, string>> GetConfigurationsAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_nextPollConfigurationToken))
                    await StartConfigurationSessionRequest(cancellationToken);


                var request = new GetLatestConfigurationRequest
                {
                    ConfigurationToken = _nextPollConfigurationToken,
                };

                var response = await _appConfigDataClient.GetLatestConfigurationAsync(request, cancellationToken);

                _nextPollConfigurationToken = response.NextPollConfigurationToken;
                _reloadPeriod = TimeSpan.FromSeconds(response.NextPollIntervalInSeconds);

                if (response.Configuration is null)
                    return null;

                Dictionary<string, string> result = await DeserializeDataAsync(response.Configuration);

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task StartConfigurationSessionRequest(CancellationToken cancellationToken)
        {
            
            
            var request = new StartConfigurationSessionRequest
            {
                ApplicationIdentifier = _applicationName,
                EnvironmentIdentifier = _environmentName,
                ConfigurationProfileIdentifier = _configurationName,
            };

            if (_reloadPeriod != default(TimeSpan))
                request.RequiredMinimumPollIntervalInSeconds = (int)_reloadPeriod.TotalSeconds;

            var response = await _appConfigDataClient.StartConfigurationSessionAsync(request, cancellationToken);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.Created)
                _nextPollConfigurationToken = response.InitialConfigurationToken;
        }

        private async Task<Dictionary<string, string>> DeserializeDataAsync(MemoryStream stream)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string json = null;

            using (StreamReader reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            if (!string.IsNullOrEmpty(json))
            {
                result = GetJsonAsConfiguration(json); 
            }

            result = FormatConfigsToWorkWithFeaturesManager(result);

            return result;
        }

        private Dictionary<string, string> GetJsonAsConfiguration(string json)
        {
            IEnumerable<(string Path, string P)> GetLeaves(string path, JsonProperty p)
            {
                return p.Value.ValueKind != JsonValueKind.Object
                                    ? new[] { (Path: path == null ? p.Name : path + ":" + p.Name, p.Value.ToString()) }
                                    : p.Value.EnumerateObject().SelectMany(child => GetLeaves(path == null ? p.Name : path + ":" + p.Name, child));
            }

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.EnumerateObject()
                    .SelectMany(p => GetLeaves(null, p))
                    .ToDictionary(k => k.Path, v => v.P);
            }
        }

        private Dictionary<string, string> FormatConfigsToWorkWithFeaturesManager(Dictionary<string, string> json)
        {
            var result = new Dictionary<string, string>();
            var groupedAttributesByFeaturesMainFlag = GetGroupedAttributesByFeaturesMainFlagFromJson(json);

            foreach (var featureFlagAttributes in groupedAttributesByFeaturesMainFlag)
            {
                var type = GetFeatureFlagType(featureFlagAttributes);

                var configsFromFeatureFlag = GenerateFormatedConfigsFromFeatureFlagBasedOnType(type, featureFlagAttributes);

                configsFromFeatureFlag.ToList().ForEach(x => result.Add(x.Key, x.Value));
            }

            return result;
        }

        private Dictionary<string, string> GenerateFormatedConfigsFromFeatureFlagBasedOnType(string type, IGrouping<string, KeyValuePair<string, string>> featureFlagAttributes)
        {
            var result = new Dictionary<string, string>();

            if (type == FlagTypes.Filter)
            {
                var enableForValueFromFilterAsString = featureFlagAttributes.FirstOrDefault(x => x.Key.Contains(":enable_for")).Value;

                var filterDefinition = JsonConvert.DeserializeObject<FeatureFilterDefinition>(enableForValueFromFilterAsString);

                var nameCpmfog = $"FeatureManagement:{featureFlagAttributes.Key}:EnabledFor:0:Name";
                result.Add(nameCpmfog, filterDefinition.Name);

                foreach (var x in filterDefinition.Parameters)
                {
                    var parametersForConfig = $"FeatureManagement:{featureFlagAttributes.Key}:EnabledFor:0:Parameters:{x.Key}";
                    result.Add(parametersForConfig, x.Value.ToString());
                }
            }
            else if (type == FlagTypes.Nested) 
            {
                foreach (var item in featureFlagAttributes)
                {
                    var formatedKey = "FeatureManagement:" + item.Key.Replace(":enabled", "").Replace(":", "__");
                    result.Add(formatedKey, item.Value);
                }
            }
            else
            {
                foreach (var item in featureFlagAttributes)
                {
                    var formatedKey = "FeatureManagement:" + item.Key.Replace(":enabled", "");
                    result.Add(formatedKey, item.Value);
                }
            }

            return result;
        }

        private static IEnumerable<IGrouping<string, KeyValuePair<string, string>>> GetGroupedAttributesByFeaturesMainFlagFromJson(Dictionary<string, string> json)
        {
            return json.GroupBy(x => x.Key.Split(':')[0]);
        }

        private static string GetFeatureFlagType(IGrouping<string, KeyValuePair<string, string>> featureAttributes)
        {
            return featureAttributes.FirstOrDefault(x => x.Key.Split(':')[1] == "type").Value;
        }
    }
}

