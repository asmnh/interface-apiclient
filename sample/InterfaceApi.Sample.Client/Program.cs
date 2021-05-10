using InterfaceApi.Sample.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using InterfaceApiClient;
using System.Threading.Tasks;
using System.Threading;

namespace InterfaceApi.Sample.Client
{
    class Program
    {
        static Task Main(string[] args) =>
            CreateHostBuilder(args).Build().RunAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (_, services) =>
                    services.AddHostedService<Worker>()
                        .AddHttpClient()
                        .UseInterfaceApiClient()
                            .WithTransientClient<IHelloWorld>(config => { config.UseEndpoint<IHelloWorld>("https://localhost:44356/"); })
                            .Apply()
                );
    }

    public class Worker : BackgroundService
    {
        private readonly IHelloWorld _helloClient;

        public Worker(IHelloWorld helloClient)
        {
            _helloClient = helloClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string world = await _helloClient.SayHello("World");
                    Console.WriteLine(world);
                    string france = await _helloClient.SayHello("France", language: "fr");
                    Console.WriteLine(france);
                    string rude = await _helloClient.SayHello(new HelloRequest("Waldo", "en", CustomSuffix: "Rude"));
                    Console.WriteLine(rude);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
