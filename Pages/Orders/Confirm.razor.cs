using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class ConfirmPage : ComponentBase
{
    #region Parameters

    [Parameter] public string Number { get; set; } = string.Empty;
    
    #endregion
    
    #region Properties

    public Order? Order { get; set; }
    
    #endregion
    
    #region Services

    [Inject]public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject]public ISnackbar Snackbar { get; set; } = null!;

    #endregion
    
    #region Overrides

    //método que consome a confirmacao de pagamento da API, e tras para a interface
    protected override async Task OnInitializedAsync()
    {
        var request = new PayOrderRequest
        {
            Number = Number,
        };
        var result = await OrderHandler.PaysAsync(request);
        if (result.IsSuccess == false)
        {
            Snackbar.Add(result.Message!, Severity.Error);
            return;
        }
        
        Order = result.Data;
        Snackbar.Add(result.Message!, Severity.Success);
    }
    
    #endregion
}