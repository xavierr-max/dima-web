using System.Reflection.Metadata;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class CheckoutPage : ComponentBase
{
    #region Parameters

    //serve para o front preencher e acessar o produto pelo parametro da url
    [Parameter] public string ProductSlug { get; set; } = string.Empty;

    //serve para o front preencher e acessar o cupom pela query da url
    [SupplyParameterFromQuery(Name = "voucher")] public string? VoucherNumber { get; set; } //APRESENTOU PROBLEMAS
    
    #endregion
    
    #region Properties
    

    public bool IsBusy { get; set; }
    public bool IsValid { get; set; }
    
    public CreateOrderRequest InputModel { get; set; } = new();
    public Product? Product { get; set; }
    public Voucher? Voucher { get; set; } //APRESENTOU PROBLEMAS
    public decimal Total { get; set; }

    #endregion
    
    #region Services

    [Inject] public IProductHandler ProductHandler { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject] public IVoucherHandler VoucherHandler { get; set; } = null!; //APRESENTOU PROBLEMAS
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    #endregion
    
    #region Methods

    //recupera um produto e o voucher e calcula o total (problema para obter o voucher)
    protected override async Task OnInitializedAsync()
    {
        try
        {
            //recebe o resultado do método que proucura um produto pelo Slug
            var result = await ProductHandler.GetBySlugAsync(new GetProductBySlugRequest()
            {
                Slug = ProductSlug
            });
            //caso o método falhe, definimos como inválido o produto
            if (result.IsSuccess == false)
            {
                Snackbar.Add("Não foi possível obter o produto", Severity.Error);
                IsValid = false;
                return;
            }
            
            Product = result.Data;
        }
        catch 
        {
            Snackbar.Add("Não foi possível obter o produto", Severity.Error);
            IsValid = false;
            return;
        }

        if (Product is null)
        {
            Snackbar.Add("Não foi possível obter o produto", Severity.Error);
            IsValid = false;
            return;
        }
        
        
        IsValid = true;
        Total = Product.Price;
    }

    //cria o pedido e redireciona para a página
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var request = new CreateOrderRequest
            {
                ProductId = Product!.Id,
            };

            var result = await OrderHandler.CreateAsync(request);
            if (result.IsSuccess)
                NavigationManager.NavigateTo($"pedidos/{result.Data!.Number}");
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