using System.Net.Http.Json;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Web.Handlers;

public class VoucherHandler(IHttpClientFactory httpClientFactory) : IVoucherHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    
    public async Task<Response<Voucher?>> GetByNumberAsync(GetVoucherByNumberRequest request)
    {
        var response =
            await _client.GetAsync($"v1/vouchers");
        if (!response.IsSuccessStatusCode)
            return new Response<Voucher?>(null, 400, "Não foi possível obter o voucher (Handler)");
        
        var voucher = await response.Content.ReadFromJsonAsync<Voucher?>();
        return new Response<Voucher?>(voucher);
    }
}