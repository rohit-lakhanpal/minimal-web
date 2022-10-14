# minimal-web
This is a minimal .NET 6.0 web application that:
- Exposes health checks via the /healthz (via Microsoft.Extensions.Diagnostics.HealthChecks)
- All other requests echo the http connection, headers and query properties. Like so ![Catch-all Response Image](https://raw.githubusercontent.com/rohit-lakhanpal/minimal-web/main/preview.png)
- All headers that start with X-AZURE or X-FORWARDED are logged to App Insights as shown ![Catch-all Response Image](https://raw.githubusercontent.com/rohit-lakhanpal/minimal-web/main/headers.png)
