using Microsoft.AspNetCore.Components.Authorization;

namespace Dima.Web.Security;

//É uma interface que estabelece o contrato para qualquer classe que gerencie o estado de autenticação
public interface ICookieAuthenticationStateProvider
{
    Task<bool> CheckAuthenticatedAsync();
    Task<AuthenticationState>  GetAuthenticationStateAsync();
    void NotifyAuthenticationStateChanged();
}

