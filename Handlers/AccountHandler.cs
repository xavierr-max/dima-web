using System.Net.Http.Json;
using System.Text;
using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

//responsável por todas as interações de conta/autenticação com a API do backend.
public class AccountHandler(IHttpClientFactory clientFactory) : 
    IAccountHandler
{
    //Cria e armazena uma instância configurada de HttpClient para interagir com o backend
    private readonly HttpClient _client = clientFactory.CreateClient(Configuration.HttpClientName);
    
    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        //o primeiro parametro indica o endpoint que será feito o POST, e o segundo é o objeto que vai ser serializado e passado no corpo da requisicao
        var result = await _client.PostAsJsonAsync("v1/identity/login?useCookies=true", request);
        return result.IsSuccessStatusCode
            ? new Response<string>("Login realizado com sucesso!", 200, "Login realizado com sucesso!")
            : new Response<string>(null, 500, "Não foi possível realizar o login.");
    }

    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/identity/register?useCookies=true", request);
        return result.IsSuccessStatusCode
            ? new Response<string>("Cadastro realizado com sucesso!", 200, "Cadastro realizado com sucesso!")
            : new Response<string>(null, 500, "Não foi possível realizar o cadastro.");
    }

    public async Task LogoutAsync()
    {
        //criar e armazena uma requisicao com JSON vazio
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _client.PostAsJsonAsync("v1/identity/logout", emptyContent);
    }
}