using Newtonsoft.Json.Linq;

namespace AwsAppConfig.Microsoft.Extensions.Configuration
{
    public class FeatureFilterDefinition
    {
        public string Name { get; set; }

        public JObject Parameters { get; set; }
    }
}

