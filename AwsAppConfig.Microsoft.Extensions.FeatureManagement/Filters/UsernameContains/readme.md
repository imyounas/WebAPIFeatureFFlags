# How to use `Usernames` feature flag filter with AWS AppConfig 

## How to add the flag

### In code

1. You go to your static class which holds all key strings for feature flags
2. Add a new one, e.g. 'feature1'
3. In the place where you want to check the state of the flag, use

```test
await _featureManager.IsEnabledAsync(FeatureFlags.BETA,new UsernameFilterContext { Username = "{username_value_runtime}" })
```

### In AWS AppConfig

1. Go to AWS AppConfig and select the Config you are using in your project for feature flags
2. Click the button `+ Add new flag`
3. For flag key enter the same key you have added in your project - `feature1`
4. Add description and mark if it is a Short-term flag
5. Click on `Add new attribute`
6. The key of the attribute should be `type`, the type should be `String`, and in the value, you should add `filter`
7. Click on `Add new attribute`
8. The key of the attribute should be `enable_for`, the type should be `String`, and in the value should be the following

```text
{"Name": "Usernames", "Parameters": { "AllowedUsernames" : "{{allowed_username_1}},{{allowed_username_2}},..." }}
```

9. Click `Create flag`
10. Click `Save new version`
11. Deploy to your desired environment

## How to add new user

1. Go to the flag
2. Edit and add in the `AllowedUsernames` new username which to be available to use the feature
3. Save and deploy