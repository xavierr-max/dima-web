using System.Globalization;
using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports;

public partial class IncomesAndExpensesChartComponent : ComponentBase
{
    //propriedades para serem componentes do gráfico
    #region Properties

    public ChartOptions Options { get; set; } = new();
    public List<ChartSeries>? Series { get; set; }
    public List<string> Labels { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public IReportHandler Handler { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        var request = new GetIncomesAndExpensesRequest();
        var result = await Handler.GetIncomesAndExpensesReportAsync(request);
        if (!result.IsSuccess || result.Data is null)
        {
            Snackbar.Add("Não foi possível obter os dados do relatório", Severity.Error);
            return;
        }

        //guarda os valores de entrada 
        var incomes = new List<double>();
        //guarda os valores de saida
        var expenses = new List<double>();

        //lógica do gráfico
        foreach (var item in result.Data)
        {
            //inclui os valores para o primeiro nome/indice
            incomes.Add((double)item.Incomes);
            //inclui os valores para o segundo nome/indice
            expenses.Add(-(double)item.Expenses);
            //inclui os valores da linha de baixo do grafico (meses)
            Labels.Add(GetMonthName(item.Month));
        }

        //valor máximo da coluna lateral do grafico
        Options.YAxisTicks = 1000;
        Options.LineStrokeWidth = 5;
        Options.ChartPalette = ["#00b34c", Colors.Red.Default];
        Series =
        [
            //primeiro nome/indice do gráfico
            new ChartSeries { Name = "Receitas", Data = incomes.ToArray() },
            //segundo nome/indice do gráfico
            new ChartSeries { Name = "Saídas", Data = expenses.ToArray() }
        ];

        StateHasChanged();
    }

    #endregion

    //converte o tipo int (representando o numero do mes) para string (para obter o nome do mes)
    private static string GetMonthName(int month)
        => new DateTime(DateTime.Now.Year, month, 1)
            .ToString("MMMM", CultureInfo.CurrentCulture);
}