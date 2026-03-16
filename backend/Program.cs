using BOF_app.Services;

var myAllowSpecificOrigins = "_myAllowSpecificOriginPolicy";

var builder = WebApplication.CreateBuilder(args);
var isRunningInContainer = builder.Configuration.GetValue("DOTNET_RUNNING_IN_CONTAINER", false);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IBanknotesService, BanknotesService>();
builder.Services.AddScoped<IExchangeRatesService, ExchangeRatesService>();
builder.Services.AddHttpClient<IExchangeRatesService, ExchangeRatesService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173", "https://red-tree-089f24210.6.azurestaticapps.net/")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();
Console.WriteLine(DateTime.Now.ToString());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}

app.UseCors(myAllowSpecificOrigins);

app.MapControllers();

app.Run();
