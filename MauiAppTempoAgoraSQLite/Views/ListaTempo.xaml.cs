using MauiAppTempoAgoraSQLite.Models;

namespace MauiAppTempoAgoraSQLite.Views;

public partial class ListaTempo : ContentPage
{
    public ListaTempo()
    {
        InitializeComponent();
    }

    // Método chamado quando a página aparece
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CarregarLista();
    }

    // Método para carregar a lista de tempos do banco de dados
    private async Task CarregarLista()
    {
        try
        {
            List<Tempo> tempos = await App.Db.GetAll();
            lst_tempos.ItemsSource = tempos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao carregar lista: {ex.Message}", "OK");
        }
    }

    // Evento chamado quando o texto da barra de pesquisa muda
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string busca = e.NewTextValue;

            if (string.IsNullOrEmpty(busca))
            {
                // Se a busca estiver vazia, carrega todos os tempos
                await CarregarLista();
            }
            else
            {
                // Realiza a busca no banco de dados
                List<Tempo> tempos = await App.Db.Search(busca);
                lst_tempos.ItemsSource = tempos;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro na pesquisa: {ex.Message}", "OK");
        }
    }

    // Evento de pull-to-refresh da lista
    private async void lst_tempos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            await CarregarLista();
        }
        finally
        {
            lst_tempos.IsRefreshing = false;
        }
    }

    // Menu de contexto - Remover
    private async void MenuItem_Remover_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem item = (MenuItem)sender;
            Tempo tempo = (Tempo)item.BindingContext;

            // Confirma a exclusão
            bool confirmacao = await DisplayAlert(
                "Confirmar Exclusão",
                $"Deseja realmente excluir o tempo de {tempo.cidade}?",
                "Sim",
                "Não");

            if (confirmacao)
            {
                // Remove do banco de dados
                await App.Db.Delete(tempo.Id);

                // Recarrega a lista
                await CarregarLista();

                await DisplayAlert("Sucesso", "Tempo removido com sucesso!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao remover: {ex.Message}", "OK");
        }
    }

    // Botão para adicionar novo tempo
    private async void btn_adicionar_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new NovoTempo());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
        }
    }
}