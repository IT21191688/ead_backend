// File: Program
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ead_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

