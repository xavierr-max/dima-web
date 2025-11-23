using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public class CreateTransactionPage : ComponentBase
{
    #region Properties

    //serve apenas para administrar eventos que a operacao iniciou e finalizou
    public bool IsBusy { get; set; } = false;
    
    //representa o request 
    public CreateTransactionRequest InputModel { get; set; } = new();
    
    //propriedade para receber as categorias e servir para selecao no front (1:N)
    public List<Category> Categories { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    //para usar o método Create do Transaction 
    public ITransactionHandler TransactionHandler { get; set; } = null!;

    [Inject]
    //para usar o método GetAll trazendo todas as categorias 
    public ICategoryHandler CategoryHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Overrides

    //assim que a página carregada
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        try
        {
            //cria e recebe um modelo vazio do request de GetAll de categorias para servir de modelo
            var request = new GetAllCategoriesRequest();
            //preenche e recebe o modelo com as informacoes do usário logado, retornando as suas categorias 
            var result = await CategoryHandler.GetAllAsync(request);
            //caso tenha sucesso, a propriedade Categories recebe as categorias do usuário logado e será utilizada para o front
            if (result.IsSuccess)
            {
                Categories = result.Data ?? [];
                //valor definido antes do cliente selecionar um
                InputModel.CategoryId = Categories.FirstOrDefault()?.Id ?? 0;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Methods

    //quando formulário for submetido
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            //cria e recebe a transacao com os valores recebidos pelo InputModel
            var result = await TransactionHandler.CreateAsync(InputModel);
            if (result.IsSuccess)
            {
                //retorna a tela do cliente uma resposta definida no Handler de sucesso
                Snackbar.Add(result.Message!, Severity.Success);
                NavigationManager.NavigateTo("/lancamentos/historico");
            }
            else
                Snackbar.Add(result.Message!, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}