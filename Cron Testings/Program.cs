using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Dashboard;
using Cron_Testings.Jobs;
using System.Text;
using Hangfire.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
    config.UseConsole();
});

builder.Services.AddHangfireServer(options => options.WorkerCount = 1);

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

app.UseHangfireServer();
app.UseHangfireDashboard();

//RecurringJob.AddOrUpdate<TestingJobs>("Testing JOB",
//    x => x.RunAsync(null), Cron.Minutely, null, "testing");

BackgroundJob.Enqueue<TestingJobs>(x => x.RunAsync(null));

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

