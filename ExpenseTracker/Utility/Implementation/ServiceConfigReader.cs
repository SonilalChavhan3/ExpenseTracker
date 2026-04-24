// Copyright (c) 2026 Sonilal Chavhan. All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying, use, or distribution is prohibited.
//
// File: ServiceConfigReader.cs
// Author: Sonilal Chavhan
// Branch: <branch-name>
//
// History:
// - Created on: {date}
// - Description: Initial creation
//

using ExpenseTracker.Utility.Interface;

namespace ExpenseTracker.Utility.Implementation
{
    public class ServiceConfigReader : IServiceConfigReader
    {
        private readonly IConfiguration _configuration;
        public ServiceConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public T GetConfigeValue<T>(string sectionName, string parameterName)
        {
            try
            {
                return (T)Convert.ChangeType(_configuration.GetSection(sectionName + ":" + parameterName).Value, typeof(T));
            }
            catch (Exception)
            {

                return default(T);
            }
            
        }

        public T GetConfigSection<T>(string sectionName)
        {
            return _configuration.GetSection(sectionName).Get<T>();
        }

        public bool IsFeatureEnabled(string featureName)
        {
            return GetConfigeValue<bool>("FeatureSwitches", featureName);
        }
    }
}
