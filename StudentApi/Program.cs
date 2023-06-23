using GTUParser.Remote;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<GTUDbContext>();
var app = builder.Build();

app.MapControllerRoute(
    "default",
    "/{controller=Home}/{action=Index}/{id?}"
);

app.Run();