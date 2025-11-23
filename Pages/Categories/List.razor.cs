using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Categories;

public partial class ListCategoryPage : ComponentBase
{
    #region Properties

    public bool IsBusy;
    public List<Category> Categories { get; set; } = [];
    
    //propriedade para o filtro no front, de dados do método Filter
    public string SearchTerm { get; set; } = string.Empty;

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    //permite o uso de caixas de texto sobrepor a aplicacao
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ICategoryHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            //recebe e cria um objeto para receber o modelo de dados do GetAll 
            var request = new GetAllCategoriesRequest();
            //recebe o modelo de dados preenchido pelo método
            var result = await Handler.GetAllAsync(request);
            if (result.IsSuccess)
                //caso tenha sucesso, a Propriedade da classe Categories recebe o Data(dados da requisicao)
                Categories = result.Data ?? [];
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
    
    #region Methods
    
    //cria uma caixa de texto para aviso de deletar
    public async void OnDeleteButtonClickedAsync(long id, string title)
    {
        //recebe e cria um objeto de caixa de texto
        var result = await DialogService.ShowMessageBox(
            "ATENÇÃO",
            $"Ao prosseguir a categoria {title} será excluída. Esta é uma ação irreversível! Deseja continuar?",
            yesText: "EXCLUIR",
            cancelText: "Cancelar");

        //caso confirme a caixa de texto o método de exclusao é executado
        if (result is true)
            await OnDeleteAsync(id, title);

        //atualiza os componentes na tela (quase um F5)
        StateHasChanged();
    }

    //metodo para excluir uma categoria apos confirmar a exclusao na caixa de texto
    public async Task OnDeleteAsync(long id, string title)
    {
        try
        {
            //recebe e cria um request de delete para passar para o handler 
            var request = new DeleteCategoryRequest { Id = id };
            //deleta a categoria no banco
            await Handler.DeleteAsync(request);
            //deleta os itens relacionados aquele ID de categoria da tela
            Categories.RemoveAll(x => x.Id == id);
            Snackbar.Add($"Categoria {title} excluída", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    
    // function que filtra itens a partir do if e retorna o conteúdo caso seja true
    public Func<Category, bool> Filter => category =>
    {
        //se a propriedade SearchTerm for vazia ou nula, ele retorna todos o itens (ou seja nao tem filtro)
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;

        // filtra por Id
        if (category.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (category.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (category.Description is not null &&
            category.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    #endregion
    
}