
# How to use?

In order to use the package you need to do few simple things:

1. Add to your app config configuration for AWS
```
"AWS": {
    "Region": "eu-west-1",
    "Profile": "profile"
  }
```

2. Add configurations for the AWS AppConfig

```
"AwsAppConfigs": {
    "ApplicationName": "ExampleApplication", 
    "EnvironmentName": "Production", 
    "ConfigurationName": "ExampleConfig", 
    "Region": "eu-west-1",
    "ReloadPeriodInSeconds": 900
  },
```

3. In your project, on your initial setup invoke the following at your ApplicationBuilder

```
builder.Configuration.AddAwsAppConfig()
```

Voila! You are ready to go! Now you can use `IConfiguration` and get the configuration value as any other configuration wherever you need it.


# AwsAppConfig Configurations

| Configuration Name   | Required?   | Description
| -------------------- | ----------- |-------------------------------------------------------|
| ApplicationName      | T           | This is the name of your application in AWS AppConfig |
| EnvironmentName      | T           | This is the name of the environment from where the configurations are going to be loaded|
| ConfigurationName    | T           | The specific configuration which is going to be loaded|
| Region               | T           | The region where the AppConfig instance we want to load is instantiated|
| ReloadPeriodInSeconds| F (min 15)  | The time after which, the configurations are going to be reloaded from AppConfig. If no value or 0 is provided, this would mean that the configurations are going to be fetched only once.|


# Loading Multiple Configurations

Instead of using single AppConfig instance, you can use as many as you want. In order to do that you just need to define them in section `AwsAppConfigs`

Example:
```
"AwsAppConfigs": [{
    "ApplicationName": "ExampleApplication", 
    "EnvironmentName": "Production", 
    "ConfigurationName": "ExampleConfig", 
    "Region": "eu-west-1",
    "ReloadPeriodInSeconds": 500
  },{
    "ApplicationName": "ExampleApplication", 
    "EnvironmentName": "Production", 
    "ConfigurationName": "ExampleConfig2", 
    "Region": "eu-west-1",
    "ReloadPeriodInSeconds": 15
  },
  ...
  ],
```

If you want to pass settings through Environment Variables you can use the following example:
```
AwsAppConfigs__0__ApplicationName="ExampleApplication"
AwsAppConfigs__0__EnvironmentName="Production"
...
AwsAppConfigs__1__ApplicationName="ExampleApplication"
AwsAppConfigs__1__EnvironmentName="Production"
...
```


