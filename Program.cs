using System.Globalization;
using Dima.Core.Handlers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Dima.Web;
using Dima.Web.Handlers;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;
Configuration.StripePublicKey = builder.Configuration.GetValue<string>("StripePublicKey") ?? string.Empty;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
//força sempre utiliza nosso método de autenticacao com os cookies modificados
builder.Services.AddScoped(x=>(ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
//serviço de MudBlazor
builder.Services.AddMudServices();

builder.Services.AddHttpClient(Configuration.HttpClientName, opt =>
{
    //define uma base de endereço para se comunicar com o backend
    opt.BaseAddress = new Uri(Configuration.BackendUrl);
}).AddHttpMessageHandler<CookieHandler>();

//"se precisou de um IAccountHandler, ele devolve um AccountHandler"
builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
builder.Services.AddTransient<IVoucherHandler, VoucherHandler>();
builder.Services.AddTransient<IProductHandler, ProductHandler>();
builder.Services.AddTransient<IOrderHandler, OrderHandler>();
builder.Services.AddTransient<IStripeHandler, StripeHandler>();
builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<IReportHandler, ReportHandler>();

//habilita recursos para outros idiomas 
builder.Services.AddLocalization();
//força o código c# possuir threads em portugues
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
//força a interface ser em portugues 
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

await builder.Build().RunAsync();
