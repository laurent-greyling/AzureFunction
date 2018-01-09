using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;

namespace AzureFunction.Helpers
{
    public class AppInsightClient
    {
        private static readonly string Key = TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("APPINSIGHTS_INSTRUMENTATIONKEY");
        public static TelemetryClient Telemetry = new TelemetryClient()
        {
            InstrumentationKey = Key
        };
    }
}
