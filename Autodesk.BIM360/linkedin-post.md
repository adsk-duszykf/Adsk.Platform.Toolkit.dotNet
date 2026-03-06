# LinkedIn Post - BIM 360 SDK Announcement

🚀 **The BIM 360 SDK is HERE - 277 Endpoints, Complete Coverage!**

Following the ACC SDK I announced recently, I'm excited to share its companion: a comprehensive .NET SDK for **Autodesk BIM 360** APIs. Same approach, same quality - built with Microsoft Kiota for a fully-typed, fluent API experience.

📊 **277 endpoints** - covering ALL public BIM 360 APIs in a single, unified package. No gaps. No missing features. Complete coverage.

✨ **What's included:**
- 🔧 Issues Management
- 📊 Cost Management
- 📋 RFIs & Checklists
- 🔍 Clash Detection
- 📁 Docs & Documents
- 🏗️ ModelSet Management
- 🔗 Data Connector
- 📦 Assets Management
- 🏢 Account & Admin APIs
- 🌍 EU Region Support
- 🔎 Index/Search APIs
...and much more!

💻 **Quick example:**
```csharp
var bim360 = new BIM360client(getAccessToken);

// Get quality issues
var issues = await bim360.Issues.Containers[containerId].QualityIssues.GetAsync();
```

💡 **Why this SDK?**
- **100% complete** - All 277 public BIM 360 endpoints, no exceptions
- **Fully typed** - Complete IntelliSense support for requests, responses, headers, and query parameters
- **Fluent API** - Code structure mirrors the REST API endpoints for easy navigation
- **Modern .NET** - Built for .NET 8.0+
- **Built-in resilience** - Automatic retry handling for rate limits

📦 **Get started today:**
```
dotnet add package Adsk.Platform.BIM360
```

🔗 Check it out on NuGet and let me know what you think!

#Autodesk #AEC #Construction #DotNet #SDK #OpenSource #BIM #BIM360 #AutodeskBIM360 #SoftwareDevelopment #API
