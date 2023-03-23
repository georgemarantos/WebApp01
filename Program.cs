using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CleanDDTest.Data;
using CleanDDTest.Services;

var builder = WebApplication.CreateBuilder(args);

// --- added for sessions    
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);

});
// -- end of sessions

// Add services to the container.
builder.Services.AddControllersWithViews();

//// ===============   Uncomment THIS when working ================================
TokenCredential credential = new DefaultAzureCredential();
var keyVaultEndpoint = new Uri(builder.Configuration.GetValue<string>("KeyVault:VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
////.----------------   end of uncomment this when working

//Register blobserviceclient and implement azurestorageservice for check image upload
builder.Services.AddScoped<IDataService, DataService>();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

//// =====================   UNCOMMENT THIS WHEN WORKINGG  
//register Azure client for managed identity using DefaultAzureCredential and SqlColumnEncryptionAzureKeyVaultProvider Classes wg
SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credential);
Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
            {
                { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider }
            };
SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
//// ---------------------------   END OF UNCOMMENT THIS

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}else if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// --- added for sessions
app.UseSession();
// end of sessions

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
