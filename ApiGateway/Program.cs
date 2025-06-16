using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    var corsSettings = builder.Configuration.GetSection("Cors");
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins(corsSettings["Origins"]?.Split(',') ?? Array.Empty<string>())
               .WithMethods(corsSettings["Methods"]?.Split(',') ?? Array.Empty<string>())
               .WithHeaders(corsSettings["Headers"]?.Split(',') ?? Array.Empty<string>());
    });
});

// Add Ocelot
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration).AddPolly();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Add error handling
app.UseExceptionHandler("/error");

app.UseRouting();

// Use Ocelot middleware
await app.UseOcelot();

app.Run();


