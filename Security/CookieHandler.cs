using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Dima.Web.Security;

//inclui credenciais (como cookies de autenticação) automaticamente nas requisições HTTP feitas pela aplicação Blazor WebAssembly
//DelegatingHandler: atua como um componente de middleware (intermediário) para requisições HTTP de saída.
public class CookieHandler : DelegatingHandler
{
    //intercepta e modifica as requisições HTTP antes que sejam enviadas.
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        //inclui automaticamente quaisquer credenciais que o navegador tenha para o domínio de destino, sendo a mais comum o cookie de autenticação/sessão.
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        //usado como uma medida de segurança ou para diferenciar tipos de requisição
        request.Headers.Add("X-Request-With", ["XMLHttpRequest"]);
        
        //passa a requisição modificada
        return base.SendAsync(request, cancellationToken);
    }
}