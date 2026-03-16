using BOF_app.Services;

var MyAllowSpecificOrgins = "_myAllowSpecificOrginiPolicy";

var builder = WebApplication.CreateBuilder(args);

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
    options.AddPolicy(name: MyAllowSpecificOrgins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
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

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrgins);

app.MapControllers();

app.Run();
