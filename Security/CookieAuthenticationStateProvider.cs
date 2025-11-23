using System.Net.Http.Json;
using System.Security.Claims;
using Dima.Core.Models.Account;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dima.Web.Security;

//busca as informações do usuário no endpoint de identidade da API (v1/identity/manage/info) e notifica o Blazor sobre o estado atual do usuário.
public class CookieAuthenticationStateProvider
    //cria uma nova instancia de HttpClient
    (IHttpClientFactory clientFactory) :
    //reponsável por informar qual o usuário atual e gerencia-lo
    AuthenticationStateProvider,
    ICookieAuthenticationStateProvider
{
    private bool _isAuthenticated = false;
    private readonly HttpClient _client = clientFactory.CreateClient(Configuration.HttpClientName);

    //verifica se o usuário está autenticado
    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _isAuthenticated;
    }

    //notifica a página sobre o estado do usuário para ser atualizada
    public void NotifyAuthenticationStateChanged()
    {
        base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    //obtem o estado(descobrir e retornar as informações) do usuário
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _isAuthenticated = false;
        //cria um objeto ClaimsPrincipal que encapsula uma nova ClaimsIdentity vazia.
        //ClaimsIdentity: é um usuário inicialmente anonimo que pode ser preenchido  
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var userInfo = await GetUser();
        if (userInfo is null)
            return new AuthenticationState(user);
        
        var claims = await GetClaims(userInfo);
        
        //recebe e cria uma identidade de usuário totalmente autenticada e preenchida
        var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
        user = new ClaimsPrincipal(id);
        
        _isAuthenticated = true;
        return new AuthenticationState(user);
    }

    //obtem as inforcoes básicas do usuário pelo endpoint do Identity 
    private async Task<User?> GetUser()
    {
        try
        {
            //GetFromJsonAsync: faz uma requisição GET e deserializa automaticamente a resposta JSON em um objeto .NET
            return await _client.GetFromJsonAsync<User?>("v1/identity/manage/info");
        }
        catch
        {
            return null;
        }
    }

    //construir e retornar uma lista completa de permissões (Claim) que definem a identidade e as autorizações do usuário na aplicação.
    private async Task<List<Claim>> GetClaims(User user)
    {
        //recebe Claims do usuario no parametro
        var claims = new List<Claim>()
        {
            //passa o tipo da claim e o valor a ser armazenado
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        //1°filtro -> adiciona na lista principal de claims, as claims do usuário, com excecao do Name e Email
        //x.Key: armazena o tipo da claim
        claims.AddRange(
            user.Claims.Where(x =>
                    x.Key != ClaimTypes.Name &&
                    x.Key != ClaimTypes.Email)
                .Select(x =>
                    new Claim(x.Key, x.Value))
        );
        
        //2°filtro -> array de RoleClaim
        RoleClaim[]? roles; 
        try
        {
            roles = await _client.GetFromJsonAsync<RoleClaim[]>("v1/identity/roles");
        }
        catch
        {
            return claims;
        }
        
        //converter uma coleção de modelos RoleClaim em objetos System.Security.Claims.Claim
        claims.AddRange(
            from role in roles ?? []
            where
                !string.IsNullOrEmpty(role.Type) &&
                !string.IsNullOrEmpty(role.Value)
            select new
                Claim(role.Type!, role.Value!, role.ValueType, role.Issuer, role.OriginalIssuer));

        return claims;
    }
}