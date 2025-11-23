using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

public class OrderHandler(IHttpClientFactory httpClientFactory) : IOrderHandler
{
    private readonly HttpClient _client =  httpClientFactory.CreateClient(Configuration.HttpClientName);
    
    //nao deveria ser update?
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        //envia a requisicao com o corpo que o cliente/front definiu
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/cancel", request);
        //le o conteudo que chegou da requisicao 
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível cancelar o pedido!");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders", request);
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível criar o pedido!");
    }

    public async Task<Response<Order?>> PaysAsync(PayOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Number}/pay", request);
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível pagar o pedido!");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/refund", request);
        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
               ?? new Response<Order?>(null, 400, "Não foi possível reembolsar o pedido!");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Order>?>>("v1/orders")
           ?? new PagedResponse<List<Order>?>(null, 400, "Não foi possível obter os produtos");

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
        => await _client.GetFromJsonAsync<Response<Order?>>($"v1/orders/{request.Number}")
           ?? new Response<Order?>(null, 400, "Não foi possível obter o produto");
}