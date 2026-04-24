// Copyright (c) 2026 Sonilal Chavhan. All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying, use, or distribution is prohibited.
//
// File: IServiceConfigReader.cs
// Author: Sonilal Chavhan
// Branch: <branch-name>
//
// History:
// - Created on: {date}
// - Description: Initial creation
//

namespace ExpenseTracker.Utility.Interface
{
    
    public interface IServiceConfigReader
    {
        T GetConfigSection<T>(string sectionName);
        T GetConfigeValue<T>(string sectionName,string parameterName);
        bool IsFeatureEnabled(string featureName);
    }
}
