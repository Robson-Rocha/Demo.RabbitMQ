using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace data
{
    public static class Config
    {
        private static readonly IConfiguration _configuration;

        static Config()
        {
            string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"..\..\..\.."));
            _configuration = new ConfigurationBuilder()
                                            .SetBasePath(path)
                                            .AddJsonFile(@"appSettings.json")
                                            .Build();            
        }
        
        public static IConfiguration AppSettings => _configuration;

        public static string RabbitMQ_ConnectionString => _configuration.GetConnectionString("RabbitMQ");

        public static string MSSQL_ConnectionString => _configuration.GetConnectionString("MSSQL");

        public static int ProducerCheckInterval => int.Parse(_configuration["producer_checkInterval"]);

        public static int ConsumerBufferInterval => int.Parse(_configuration["consumer_bufferInterval"]);

        public static int ConsumerLojaId => int.Parse(_configuration["consumer_LojaId"]);
        
    }
}