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

var app = builder.Build();
app.MapHub<HubSignalR>("/hubs/test");

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
