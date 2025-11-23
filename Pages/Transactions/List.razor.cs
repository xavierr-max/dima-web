using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public partial class ListTransactionsPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; } = false;
    
    //serve para a listagem das transacoes no front
    public List<Transaction> Transactions { get; set; } = [];
    
    //serve para receber os dados de filtro do front
    public string SearchTerm { get; set; } = string.Empty;
    
    //propriedade que guarda o valor apenas do ano atual por padrao, mas pode ser alterada no front
    public int CurrentYear { get; set; } = DateTime.Now.Year;
    
    //propriedade que guarda o valor apenas do ano atual por padrao, mas pode ser alterada no front
    public int CurrentMonth { get; set; } = DateTime.Now.Month;

    //serve de selecao dos 3 ultimos anos para o front
    public int[] Years { get; set; } =
    {
        DateTime.Now.Year,
        DateTime.Now.AddYears(-1).Year,
        DateTime.Now.AddYears(-2).Year,
        DateTime.Now.AddYears(-3).Year
    };

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    //serve para caixa de textos
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ITransactionHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    //executa o método assim que a página for carregada
    protected override async Task OnInitializedAsync()
        => await GetTransactionsAsync();

    #endregion

    #region Public Methods

    //quando submetido, realiza o método de busca das transacoes e atualiza a tela
    public async Task OnSearchAsync()
    {
        await GetTransactionsAsync();
        StateHasChanged();
    }

    public async void OnDeleteButtonClickedAsync(long id, string title)
    {
        var result = await DialogService.ShowMessageBox(
            "ATENÇÃO",
            $"Ao prosseguir o lançamento {title} será excluído. Esta ação é irreversível! Deseja continuar?",
            yesText: "EXCLUIR",
            cancelText: "Cancelar");

        if (result is true)
            await OnDeleteAsync(id, title);

        StateHasChanged();
    }

    //utiliza o objeto Transaction e retorna um booleano
    public Func<Transaction, bool> Filter => transaction =>
    {
        //caso o modelo de filtro(SearchTerm) esteja vazio, todas as transacoes passam pelo filtro (ou seja, nao foi definido um filtro)
        if (string.IsNullOrEmpty(SearchTerm))
            return true;

        //se o SearchTerm tiver valor, ele filtrará pelo Id ou Title
        return transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
               || transaction.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
    };

    #endregion
    
    #region Private Methods 

    //busca as transacoes por periodo e pagina, entao retornado ao front
    private async Task GetTransactionsAsync()
    {
        IsBusy = true;

        try
        {
            //cria e recebe um objeto do tipo request de Get por Período com definicoes
            var request = new GetTransactionsByPeriodRequest
            {
                //recebe o primeiro dia do mes atual
                StartDate = DateTime.Now.GetFirstDay(CurrentYear, CurrentMonth),
                //recebe o ultimo dia do mes atual
                EndDate = DateTime.Now.GetLastDay(CurrentYear, CurrentMonth),
                PageNumber = 1,
                PageSize = 1000
            };
            //recebe o valor obtido pelo método e, para poder receber precisa ser um PagedResponse
            var result = await Handler.GetByPeriodAsync(request);
            //caso tenha sucesso, o Transaction(modelo de dados para o front) recebe o conteúdo(Data) ou retorna nulo
            if (result.IsSuccess)
                Transactions = result.Data ?? [];
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

    //quando submetido, deleta a transacao com base no id e tile informados no front
    private async Task OnDeleteAsync(long id, string title)
    {
        IsBusy = true;

        try
        {
            var result = await Handler.DeleteAsync(new DeleteTransactionRequest { Id = id });
            if (result.IsSuccess)
            {
                Snackbar.Add($"Lançamento {title} removido!", Severity.Success);
                Transactions.RemoveAll(x => x.Id == id);
            }
            else
            {
                Snackbar.Add(result.Message!, Severity.Error);
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