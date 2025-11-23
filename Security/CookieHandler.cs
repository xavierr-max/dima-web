using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Dima.Web.Security;

public class CookieHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Correto: Garante o envio do Cookie Cross-Site
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        
        // CORRIGIDO: "Requested" no lugar de "Request"
        request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);
        
        return base.SendAsync(request, cancellationToken);
    }
}