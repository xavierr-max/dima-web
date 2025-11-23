using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

public class ProductHandler(IHttpClientFactory httpClientFactory) : IProductHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    //ESTRUTURA NOVA, ORIGINAL DEU BUG
    public async Task<PagedResponse<List<Product>?>> GetAllAsync(GetAllProductsRequest request)
    {
        //recebe e faz uma requisição HTTP GET para /v1/products
        var response =
            await _client.GetAsync($"v1/products");
        //caso nao tenha sucesso na requisicao, retorna esse response de erro
        if (!response.IsSuccessStatusCode)
            return new PagedResponse<List<Product>?>(null, 400, "Não foi possível obter os produtos");

        //recebe apos ler o corpo da resposta (JSON) e converte para List<Product>
        var products = await response.Content.ReadFromJsonAsync<List<Product>?>();
        var count = products?.Count ?? 0; // recebe o total do numero de pedidos
        return new PagedResponse<List<Product>?>(products, count, request.PageNumber, request.PageSize);
    }

    public async Task<Response<Product?>> GetBySlugAsync(GetProductBySlugRequest request)
        => await _client.GetFromJsonAsync<Response<Product?>>($"v1/products/{request.Slug}")
           ?? new Response<Product?>(null, 400, "Não foi possível obter o produto");
}