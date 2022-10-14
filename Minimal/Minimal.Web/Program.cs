using Microsoft.ApplicationInsights.DataContracts;

#region PrintHelpers
/// This is a minimal .NET 6.0 web application configured for 3 things:
/// 1. Redirects to HTTPS
/// 2. Exposes health checks via the /healthz (via Microsoft.Extensions.Diagnostics.HealthChecks)
/// 3. All other requests echo the http connection, headers and query properties.
var print = (string key, object value)
    => $"{key?.ToUpper()}: {value.ToString()} {Environment.NewLine}";

var printHeaders = (string key, IHeaderDictionary headers)
    => string.Join(string.Empty, headers.OrderBy(h => h.Key).SelectMany(h => print($"{key}[{h.Key}]", h.Value)));
#endregion PrintHelpers

#region TelemetryHelper
// Log http request headers that start with X-AZURE or X-FORWARDED
var addHttpHeadersToAppInsights = 
    (RequestTelemetry telemetry, IHeaderDictionary headers) => headers
        .Where(h => h.Key.StartsWith("X-AZURE", StringComparison.InvariantCultureIgnoreCase) ||
                    h.Key.StartsWith("X-FORWARDED", StringComparison.InvariantCultureIgnoreCase))
        .ToList()
        .ForEach(h => telemetry.Properties.Add(h.Key.Replace("-", string.Empty), h.Value));
#endregion TelemetryHelper

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapHealthChecks("/healthz");

// map catch-all handler
app.MapFallback((HttpContext c) =>
{
    // capture headers for App Insights
    addHttpHeadersToAppInsights(c.Features.Get<RequestTelemetry>(), c.Request.Headers);

    // return catch-all result
    var output = print("host", c.Request.Host);
    output += print("path", c.Request.Path);
    output += printHeaders("request header", c.Request.Headers);
    output += print("query string", c.Request.QueryString);
    return output;
});


app.Run();
