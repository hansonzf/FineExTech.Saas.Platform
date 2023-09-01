using MediatR;
using Microsoft.EntityFrameworkCore;
using Orderpool.Api.Application.CollaborateServices.OrderCenter;
using Orderpool.Api.Infrastructure;
using Orderpool.Api.Models;
using Orderpool.Api.Services;
using Quartz;
using System.Net.NetworkInformation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderpoolDbContext>(options => {
    options.UseSqlServer(connectionString);
});

builder.Services.AddQuartz(q => {
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
});
builder.Services.AddQuartzServer(options => {
    options.WaitForJobsToComplete = true;
});

Assembly[] assemblies = new Assembly[1]
{
    Assembly.GetExecutingAssembly()
};
builder.Services.AddMediatR(assemblies);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
