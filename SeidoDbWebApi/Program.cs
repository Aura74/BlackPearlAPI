using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DbAppWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppLog.Instance.LogInformation("Main started");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>() );
    }
}
