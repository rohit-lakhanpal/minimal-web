/// This is a minimal .NET 6.0 web application configured for 3 things:
/// 1. Redirects to HTTPS
/// 2. Exposes health checks via the /healthz (via Microsoft.Extensions.Diagnostics.HealthChecks)
/// 3. All other requests echo the http connection, headers and query properties.

#region PrintHelpers
var print = (string key, object value)
    => $"{key?.ToUpper()}: {value.ToString()} {Environment.NewLine}";

var printHeaders = (string key, IHeaderDictionary headers)
    => string.Join(string.Empty, headers.SelectMany(h => print($"{key}[{h.Key}]", h.Value)));
#endregion PrintHelpers


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapHealthChecks("/healthz");


app.MapFallback((HttpContext c) =>
{   
    var output = print("host", c.Request.Host);
    output += print("path", c.Request.Path);
    output += printHeaders("request header", c.Request.Headers);
    output += print("query string", c.Request.QueryString);
    return output;
});


app.Run();
