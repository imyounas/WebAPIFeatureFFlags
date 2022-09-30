using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAppConfig.Microsoft.Extensions.FeatureManagement.Filters.Usernames
{

    [FilterAlias("UsernameContains")]
    public class UsernameContainsFilter : IContextualFeatureFilter<IUsernameContainsFilterContext>
    {
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureEvaluationContext, IUsernameContainsFilterContext accountId)
        {
            var settings = featureEvaluationContext.Parameters.Get<UsernameContainsFilterSettings>();

            if (settings is null || settings.TextsAllowedInUsername is null)
            {
                return Task.FromResult(false);
            }
            else if(settings.TextsAllowedInUsername.Trim() == "*")
            {   
                return Task.FromResult(true);
            }
            else
            {
                var result = settings.TextsAllowedInUsername.Split(',').Any(x => accountId.Username.Contains(x));

                return Task.FromResult(result);
            }
        }
    }
}
