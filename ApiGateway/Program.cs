using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOcelot();
//builder.WebHost.ConfigureAppConfiguration((ctx, conf) => {
//    conf.AddOcelot(ctx.HostingEnvironment);
//});
var app = builder.Build();

app.UseOcelot();
app.MapGet("/", () => "Getway is running!");

app.Run();
