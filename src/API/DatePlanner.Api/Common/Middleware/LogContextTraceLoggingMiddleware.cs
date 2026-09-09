using System.Diagnostics;
using Serilog.Context;

namespace DatePlanner.Api.Common.Middleware;

internal sealed class LogContextTraceLoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        string traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next.Invoke(context);
        }
    }
}
