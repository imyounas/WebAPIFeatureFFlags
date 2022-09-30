using Amazon.Runtime.Internal.Transform;
using AwsAppConfig.Microsoft.Extensions.FeatureManagement.Filters.Usernames;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Onboarding.MerchantProvisioning.FeatureFlags;

namespace WebAPIFeatureFFlags.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        [HttpGet(Name = "GetFeatureFlags")]
        public async Task<Dictionary<string, bool>> GetFeatureFlags()
        {
            Dictionary<string, bool> fflagValues = new Dictionary<string, bool>();
            string userName = "imran@truevo.com";

            var isBirthDateInFutureValidationEnabled = await _featureManager.IsEnabledAsync(Features.BIRTH_DATE_NOT_IN_FUTURE,
               new UsernameContainsFilterContext { Username = userName });

            var bankSwiftCodeValidationFeatureFlag = await _featureManager.IsEnabledAsync(Features.BANK_SWIFT_CODE_VALIDATION,
                   new UsernameContainsFilterContext { Username = userName });

            var isIdentityDocExpired = await _featureManager.IsEnabledAsync(Features.IS_IDENTITY_DOC_EXPIRED,
                new UsernameContainsFilterContext { Username = userName });

            var ownershipPercentageValidation = await _featureManager.IsEnabledAsync(Features.OWNERSHIP_PERCENTAGE_VALIDATION,
              new UsernameContainsFilterContext { Username = userName });

            var isDateOfIncorporationNotInInFuture = await _featureManager.IsEnabledAsync(Features.DATE_OF_INCORPORATION_NOT_IN_FUTURE,
              new UsernameContainsFilterContext { Username = userName });

            

            var bankIBANCodeValidationFeatureFlag = await _featureManager.IsEnabledAsync(Features.BANK_IBAN_CODE_VALIDATION,
                new UsernameContainsFilterContext { Username = userName });

            fflagValues.Add("ownershipPercentageValidation", ownershipPercentageValidation);
            fflagValues.Add("bankSwiftCodeValidationFeatureFlag", bankSwiftCodeValidationFeatureFlag);
            fflagValues.Add("bankIBANCodeValidationFeatureFlag", bankIBANCodeValidationFeatureFlag);
            fflagValues.Add("isBirthDateInFutureValidationEnabled", isBirthDateInFutureValidationEnabled);
            fflagValues.Add("isIdentityDocExpired", isIdentityDocExpired);
            fflagValues.Add("isDateOfIncorporationNotInInFuture", isDateOfIncorporationNotInInFuture);
            

            return fflagValues;
        }
    }
}