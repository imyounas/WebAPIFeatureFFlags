namespace Onboarding.MerchantProvisioning.FeatureFlags
{
    public static class Features
    {
        public const string PHASE_2 = "phase_two_features";
        public const string DATES_RESPONSE_FORMAT_INCLUDING_TIME = "dates_response_format_to_include_time";
        public const string BANK_SWIFT_CODE_VALIDATION = "bank_swift_code_validation";
        public const string BANK_IBAN_CODE_VALIDATION = "bank_iban_code_validation";
        public const string OWNERSHIP_PERCENTAGE_VALIDATION = "ownership_percentage_validation";
        public const string BIRTH_DATE_NOT_IN_FUTURE = "birth_date_not_in_future";
        public const string DATE_OF_INCORPORATION_NOT_IN_FUTURE = "date_of_incorporation_not_in_future";
        public const string IS_IDENTITY_DOC_EXPIRED = "is_identity_document_expired";
    }
}