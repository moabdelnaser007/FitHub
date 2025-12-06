using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace FitHubBackendAPI.Attributes
{
    public class BenchmarkAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var process = Process.GetCurrentProcess();

            TimeSpan cpuBefore = process.TotalProcessorTime;
            var memBefore = GC.GetTotalMemory(true);
            var sw = Stopwatch.StartNew();

            var exEndpoint = await next();

            sw.Stop();
            long memAfter = GC.GetTotalMemory(true);
            TimeSpan cpuAfter = process.TotalProcessorTime;

            Console.WriteLine($"\n=== BENCHMARK for {context.ActionDescriptor.DisplayName} ===");
            Console.WriteLine($"Time:   {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"CPU:    {(cpuAfter - cpuBefore).TotalMilliseconds} ms");
            Console.WriteLine($"Memory: {memAfter - memBefore} bytes");
            Console.WriteLine("=============================================\n");

        }
    }
}
