using Amazon;
using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config AWS SQS client
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = builder.Configuration.GetSection("AWS");
    var region = config["Region"] ?? "us-east-1";  // Virginia EUA
    return new AmazonSQSClient(RegionEndpoint.GetBySystemName(region));
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("vue",
        p => p.WithOrigins("http://localhost:5173")   // porta padrão do Vite
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());                   // necessário p/ SignalR cross-origin
});


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

app.UseCors("vue");
app.MapHub<HubSignalR>("/hubs/test");
app.Run();
