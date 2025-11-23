using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Identity;

//contém a lógica c# da página Register
public partial class RegisterPage : ComponentBase
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

    #region Properties
    
    //define se a página está ocupada
    public bool IsBusy { get; set; } = false;
    
    //recebe os dados vindos da página por meio de alguma referencia html/razor
    public RegisterRequest InputModel { get; set; } = new();
    
    #endregion

    #region Overrides 
    
    //verifica se o usuário está autenticado, e envia para página inicial caso esteja
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        //"se existe o usuário e ele é autenticado, então... "
        if(user.Identity is {IsAuthenticated : true})
            NavigationManager.NavigateTo($"/");
    }
    
    #endregion
    
    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var result = await Handler.RegisterAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message!, Severity.Success);
                NavigationManager.NavigateTo($"/login");
            }
            else 
                Snackbar.Add(result.Message!, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    #endregion
}