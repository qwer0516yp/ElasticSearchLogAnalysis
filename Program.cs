using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => 
                            configuration.Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .WriteTo.Console()
                            .WriteTo.File($"logs/testlog_{DateTime.UtcNow:yyyy-MM}.log", rollingInterval: RollingInterval.Day)
                            .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"])) 
                            { 
                                IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                                AutoRegisterTemplate= true,
                                NumberOfShards = 2,
                                NumberOfReplicas = 1
                            })
                            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                            .ReadFrom.Configuration(context.Configuration)
                      );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
