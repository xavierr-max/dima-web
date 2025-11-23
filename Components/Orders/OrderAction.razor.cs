using System.Runtime.InteropServices;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Requests.Stripe;
using Dima.Web.Pages.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Dima.Web.Components.Orders;

public partial class OrderActionComponent : ComponentBase
{
    #region Parameters

    [CascadingParameter]
    public DetailsPage Parent { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public Order Order { get; set; } = null!;

    #endregion
    
    #region Servies

    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;

    [Inject] public IStripeHandler StripeHandler { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    #endregion
    
    #region Public Methos

    public async void OnCancelButtonClickAsync()
    {
        var result = await DialogService.ShowMessageBox(
            "ATENÇÃO",
            "Deseja realmente cancelar o pedido?",
            yesText: "Sim", cancelText: "Não");

        if (result is not null && result == true)
            await CancelOrderAsync();
    }

    public async void OnPayButtonOnClickedAsync()
    {
        await PayOrderAsync();
    }
    
    public async void OnRefundButtonClickAsync()
    {
        var result = await DialogService.ShowMessageBox(
            "ATENÇÃO",
            "Deseja realmente estornar o pedido?",
            yesText: "Sim", cancelText: "Não");

        if (result is not null && result == true)
            await RefundOrderAsync();
    }
    
    #endregion
    
    #region Private Methods

    private async Task CancelOrderAsync()
    {
        var request = new CancelOrderRequest
        {
            Id = Order.Id,
        };

        var result = await OrderHandler.CancelAsync(request);
        if (result.IsSuccess)
            Parent.RefreshState(result.Data!);
        else
            Snackbar.Add(result.Message!,  Severity.Error);
    }

    private async Task PayOrderAsync()
    {
        //preenchea a sessao do stripe com as informacoes da ordem/pedido
        var request = new CreateSessionRequest
        {
            OrderNumber = Order.Number,
            OrderTotal = (int)Math.Round(Order.Total * 100, 2), //799.90 -> 79990 
            ProductTitle = Order.Product.Title,
            ProductDescription = Order.Product.Description,
        };

        try
        {
            //recebe e cria uma sessao com base nas informacoes fornecidas pelo pedido, usando a chave secreta do stripe
            var result = await StripeHandler.CreateSessionAsync(request);
            if (result.IsSuccess == false)
            {
                Snackbar.Add(result.Message!, Severity.Error);
                return;
            }

            if (result.Data is null)
            {
                Snackbar.Add(result.Message!, Severity.Error);
                return;
            }

            //cria a sessao para o cliente e redireciona para a página de pagamento
            await JsRuntime.InvokeVoidAsync("checkout", Configuration.StripePublicKey, result.Data);

            //error401 - acredito que esteja acontecendo por estar em uma conta de teste no stripe
        }
        catch 
        {
            Snackbar.Add("Não foi possível iniciar a sessão", Severity.Error);
        }
    }
    
    private async Task RefundOrderAsync()
    {
        var request = new RefundOrderRequest
        {
            Id = Order.Id,
        };

        var result = await OrderHandler.RefundAsync(request);
        if (result.IsSuccess)
            Parent.RefreshState(result.Data!);
        else
            Snackbar.Add(result.Message!, Severity.Error);
    }
    
    #endregion
}