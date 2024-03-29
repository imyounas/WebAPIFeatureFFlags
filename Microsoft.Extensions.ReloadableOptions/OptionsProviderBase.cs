﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.ReloadableOptions
{
    public abstract class OptionsProviderBase<TOptions> :
        IConfigureOptions<TOptions>,
        IOptionsChangeTokenSource<TOptions>,
        IOptionsFactory<TOptions>
        where TOptions : class, new()
    {
        protected IConfiguration configuration;

        public OptionsProviderBase(IConfiguration configuration, string name)
        {
            this.configuration = configuration;
            this.Name = name;
        }

        public OptionsProviderBase(IConfiguration configuration)
            : this(configuration, Options.Options.DefaultName)
        { }

        public string Name { get; }

        public abstract void Configure(TOptions options);

        public TOptions Create(string name)
        {
            var newOptions = new TOptions();
            Configure(newOptions);
            return newOptions;
        }

        public IChangeToken GetChangeToken() => configuration.GetReloadToken();
    }
}