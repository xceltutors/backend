﻿using Xcel.Config;
using Xcel.Config.Options;

namespace Infra.Options;

public class DevPowersOptions : IOptionsValidator
{
    public DatabaseDevPower Recreate { get; set; } = DatabaseDevPower.None;
    public DatabaseDevPower Migrate { get; set; } = DatabaseDevPower.None;

    public void Validate(EnvironmentOptions environmentOptions)
    {
        if (environmentOptions.IsProduction())
        {
            if (Recreate != DatabaseDevPower.None || Migrate != DatabaseDevPower.None)
            {
                throw new ArgumentException("[DevPowersOptions] The database migrate options are not supported in production.");
            }
        }
    }
}