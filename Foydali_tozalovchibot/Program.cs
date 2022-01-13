using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Foydali_tozalovchibot
{
    public static class Program
    {
        public static List<DB> dBs = JsonConvert.DeserializeObject<List<DB>>(File.ReadAllText(@"db.json"));
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            try
            {
               //dBs = JsonConvert.DeserializeObject<List<DB>>(File.ReadAllText(@"C:\Users\USER\source\repos\foydali_tozalovchibot\Foydali_tozalovchibot\db.json"));
            }
            catch (Exception ex) { Console.WriteLine("error:\n" + ex.Message); }
            //Console.WriteLine(JsonConvert.SerializeObject(dBs));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
