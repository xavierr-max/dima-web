using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

//IHttpClientFactory: Objeto que permite os métodos de requisicao 
public class CategoryHandler(IHttpClientFactory httpClientFactory): ICategoryHandler
{
    //instancia do objeto IHttpClientFactory
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        //define o endpoint do tipo POST e envia o conteúdo em JSON fornecido pela página
        var result = await _client.PostAsJsonAsync("v1/categories", request);
        //está desserializando a resposta em JSON e retornado para a API
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
               ?? new Response<Category?>(null, 400, "Falha ao criar a categoria");
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
               ?? new Response<Category?>(null, 400, "Falha ao atualizar a categoria");
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        var result = await _client.DeleteAsync($"v1/categories/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
               ?? new Response<Category?>(null, 400, "Falha ao excluir a categoria");
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
        => await _client.GetFromJsonAsync<Response<Category?>>($"v1/categories/{request.Id}")
           ?? new Response<Category?>(null, 400, "Não foi possível obter a categoria");
    //obs: nao possui um ReadFromJsonAsync pois a API somente retorna a informacao
    

    public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Category>>>("v1/categories")
           ?? new PagedResponse<List<Category>>(null, 400, "Não foi possível obter as categorias");
}