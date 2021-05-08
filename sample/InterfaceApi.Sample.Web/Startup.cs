using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InterfaceApi.Sample.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace InterfaceApi.Sample.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public record Hello2Body(string? Suffix);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("UP!");
                });
                endpoints.MapPost("/hello2/{name:alpha}", async context =>
                {
                    string name = context.Request.RouteValues["name"]!.ToString()!;
                    var lang = context.Request.Query["lang"].ToString();
                    var body = await JsonSerializer.DeserializeAsync<Hello2Body>(context.Request.Body);
                    await context.Response.WriteAsync(JsonSerializer.Serialize($"Hello {name} from {lang} - you are {body?.Suffix}"));
                });
                endpoints.MapPost("/hello/{toWhom:alpha}", async context =>
                {
                    var toWhom = context.Request.RouteValues["toWhom"];
                    context.Request.Query.TryGetValue("lang", out StringValues lang);
                    await context.Response.WriteAsync(JsonSerializer.Serialize($"Hello, {toWhom} from {lang}"));
                });
            });
        }
    }
}
