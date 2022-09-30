using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAppConfig.Microsoft.Extensions.FeatureManagement.Filters.Usernames
{

    [FilterAlias("Usernames")]
    public class UsernameFilter : IContextualFeatureFilter<IUsernameFilterContext>
    {
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureEvaluationContext, IUsernameFilterContext accountId)
        {
            var settings = featureEvaluationContext.Parameters.Get<UsernamesFilterSettings>();

            if (settings is null || settings.AllowedUsernames is null)
                return Task.FromResult(false);
            else
            {
                var result = settings.AllowedUsernames.Split(',').Any(x => x == accountId.Username);

                return Task.FromResult(result);
            }
        }
    }
}
