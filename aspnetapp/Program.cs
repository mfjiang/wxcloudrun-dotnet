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

//�����ȡ��������BODY��
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});
//�������            
app.UseCorsMiddleware();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
