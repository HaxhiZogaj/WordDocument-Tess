using Syncfusion.Licensing;
using Microsoft.EntityFrameworkCore;
using WordDocumentEditor.Models;
using Microsoft.Extensions.Configuration;
using WordDocumentEditor.DBEModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5cdHRSRGlfV0R/X0o=");


// Register db context for Identity
builder.Services.AddDbContext<DocTemplateDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DocTemplateDBContext"));//DbContextIdentity
});

//builder.Services.AddScoped<IDocTemplateRepository, DocTemplateRepository>();
//builder.Services.AddScoped<IDocTemplateService, DocTemplateService>();


var connectionString = builder.Configuration.GetConnectionString("DocContext");
builder.Services.AddDbContext<DocContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
