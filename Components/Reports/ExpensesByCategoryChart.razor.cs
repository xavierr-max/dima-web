using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports;

public partial class ExpensesByCategoryChartComponent : ComponentBase
{
    //propriedades que o MudBlazor exige para fazer parte da estrutura do gráfico
    #region Properties

    public List<double> Data { get; set; } = [];
    public List<string> Labels { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public IReportHandler Handler { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        await GetExpensesByCategoryAsync();
    }

    private async Task GetExpensesByCategoryAsync()
    {
        //recebe um request para poder acionar o método, nao possui um corpo por ser um método GET 
        var request = new GetExpensesByCategoryRequest();
        //recebe o conteúdo do método vindo da API 
        var result = await Handler.GetExpensesByCategoryReportAsync(request);
        if (!result.IsSuccess || result.Data is null)
        {
            Snackbar.Add("Falha ao obter dados do relatório", Severity.Error);
            return;
        }

        //exibe cada categoria e seu respectivo valor 
        //lógica para o grafico funcionar 
        foreach (var item in result.Data)
        {
            Labels.Add($"{item.Category} ({item.Expenses:C})");
            Data.Add(-(double)item.Expenses);
        }
    }
    
    #endregion
}