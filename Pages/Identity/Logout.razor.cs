using Dima.Core.Handlers;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Identity;

public partial class LogoutPage : ComponentBase
{
    #region Dependencies

    //injeção de dependencia 
    //inject: busca do Program algum serviço Transient, Scoped ou Singleton
    [Inject] 
    //ISnackbar: retorna o resultado para página, similar ao StatusCode no aspnet
    public ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    public IAccountHandler Handler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } =  null!;

    [Inject]
    public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    #endregion
    
    #region Overrides 
    
    //desconecta o autenticacao do usuario e atualiza a página
    protected override async Task OnInitializedAsync()
    {
        if (await AuthenticationStateProvider.CheckAuthenticatedAsync())
        {
            await Handler.LogoutAsync();
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            AuthenticationStateProvider.NotifyAuthenticationStateChanged();
        }
        
        //garantir que o seu componente funcione corretamente
        await base.OnInitializedAsync();
    }
    
    #endregion
}