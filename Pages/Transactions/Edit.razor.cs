using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public partial class EditTransactionPage : ComponentBase
{
    #region Properties

    [Parameter]
    
    //serve para receber o Id que vem da url
    public string Id { get; set; } = string.Empty;
    public bool IsBusy { get; set; } = false;
    public UpdateTransactionRequest InputModel { get; set; } = new();
    
    //serve selecionar qual o tipo de categoria tera a transacao
    public List<Category> Categories { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ITransactionHandler TransactionHandler { get; set; } = null!;

    [Inject]
    public ICategoryHandler CategoryHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        await GetTransactionByIdAsync();
        await GetCategoriesAsync();

        IsBusy = false;
    }

    #endregion

    //método da API relacionado a página
    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var result = await TransactionHandler.UpdateAsync(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add("Lançamento atualizado", Severity.Success);
                NavigationManager.NavigateTo("/lancamentos/historico");
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
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

    //métodos da API
    #region Private Methods

    //busca as informacoes de uma transacao pelo o Id na url e, retorna para o front editar 
    private async Task GetTransactionByIdAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetTransactionByIdRequest() { Id = long.Parse(Id) };
            var result = await TransactionHandler.GetByIdAsync(request);
            if (result is { IsSuccess: true, Data: not null })
            {
                InputModel = new UpdateTransactionRequest
                {
                    CategoryId = result.Data.CategoryId,
                    PaidOrReceivedAt = result.Data.PaidOrReceivedAt,
                    Title = result.Data.Title,
                    Type = result.Data.Type,
                    Amount = result.Data.Amount,
                    Id = result.Data.Id,
                };
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

    //busca todas as categorias e guarda na lista para selecao(Categories)
    private async Task GetCategoriesAsync()
    {
        IsBusy = true;
        try
        {
            //cria e recebe um request de GetAll de categorias
            var request = new GetAllCategoriesRequest();
            //recebe por um Response o resultado do método 
            var result = await CategoryHandler.GetAllAsync(request);
            //caso tenha sucesso, a propriedade Categories recebe todas as categorias 
            if (result.IsSuccess)
            {
                Categories = result.Data ?? [];
                //busca a primeira categoria pelo Id e faz uma prédefinicao
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
}