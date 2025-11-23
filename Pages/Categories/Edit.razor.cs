using Dima.Core.Handlers;
using Dima.Core.Requests.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Categories;

public partial class EditCategoryPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; } = false;
    public UpdateCategoryRequest InputModel { get; set; } = new();

    #endregion
    
    #region Parameters

    [Parameter]
    public string Id { get; set; } =  string.Empty;
    
    #endregion
    
    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject] 
    public NavigationManager NavigationManager { get; set; } = null!;
    
    [Inject]
    public ICategoryHandler Handler { get; set; } = null!;
    
    #endregion
    
    #region Overrides
    
    //assim que a página for chamada, o método é acionado
    protected override async Task OnInitializedAsync()
    {
        GetCategoryByIdRequest? request = null;
        try
        {
            //cria e recebe um request(modelo de receber dados do frontend) de obter uma categoria pelo Id
            request = new GetCategoryByIdRequest
            {
                //Id do request recebe o valor do parametro
                Id = long.Parse(Id)
            };
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        IsBusy = true;
        try
        {
            await Task.Delay(1000);
            //faz a busca pelas informacoes da categoria
            var response = await Handler.GetByIdAsync(request);
            //se o retorno for 200 e o Data possuir um corpo do response, cria-se um objeto para mostrar as informacoes atuais no front
            if (response.IsSuccess && response.Data is not null)
                InputModel = new UpdateCategoryRequest
                {
                    Id = response.Data.Id,
                    Title = response.Data.Title,
                    Description = response.Data.Description,
                };
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
    
    #region Methods
    
    //chamado quando um formulário é submetido e chama o método
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            //atualiza e recebe o InputModel que está no formalário
            var result = await Handler.UpdateAsync(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add("Categoria atualizada", Severity.Success);
                NavigationManager.NavigateTo("/categorias");
            }
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