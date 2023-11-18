using aspnetapp.Code;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CounterContext>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

//允许读取缓冲区的BODY流
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});
//允许跨域            
app.UseCorsMiddleware();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
