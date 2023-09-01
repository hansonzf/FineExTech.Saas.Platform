using LocationApi.Domain.AggregateModels.LocationAggregate;
using LocationApi.Infrastructure;
using LocationApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LocationDbContext>(options => {
    options.UseSqlServer(defaultConnection);
});

builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "vue-client",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:8080", "http://10.32.16.183:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("vue-client");
app.UseAuthorization();

app.MapControllers();

app.Run();
