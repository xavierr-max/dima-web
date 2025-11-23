using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages;

//contém a lógica do Resumo Financeiro
public class HomePage : ComponentBase
{
    #region Properties

    //propriedade para definir o modo oculto, para esconder valores
    public bool ShowValues { get; set; } = true;
    
    //propriedade para exibir os valores recebidos da API
    public FinancialSummary? Summary { get; set; }

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IReportHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        var request = new GetFinancialSummaryRequest();
        var result = await Handler.GetFinancialSummaryReportAsync(request);
        //caso tenha sucesso, Summary recebe os valores de entrada, saida e sua diferenca conforme o método
        if (result.IsSuccess)
            Summary = result.Data;
    }

    #endregion

    #region Methods

    //define se o valor pode ser exibido ou nao
    public void ToggleShowValues() 
        => ShowValues = !ShowValues;

    #endregion
}