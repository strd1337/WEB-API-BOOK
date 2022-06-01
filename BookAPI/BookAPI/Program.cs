using BookAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from the config file
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Add the ApplicationContext as a service to the application
builder.Services.AddDbContext<BookDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddRazorPages();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();


// Add services to the container; handling loops
builder.Services.AddControllersWithViews().AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = 
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
