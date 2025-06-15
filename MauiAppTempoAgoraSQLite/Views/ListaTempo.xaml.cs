using MauiAppTempoAgoraSQLite.Models;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class ListaTempo : ContentPage
    {
        public ListaTempo()
        {
            InitializeComponent();
        }

        // M�todo chamado quando a p�gina aparece
        protected async override void OnAppearing()
        {
            // Carrega a lista de tempos do banco de dados
            await CarregarListaTempos();
        }

        // M�todo para carregar todos os tempos do banco de dados
        private async Task CarregarListaTempos()
        {
            try
            {
                // Busca todos os tempos no banco de dados
                List<Tempo> tempos = await App.Db.GetAll();

                // Define a fonte de dados da ListView
                lst_tempos.ItemsSource = tempos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar tempos: {ex.Message}", "OK");
            }
        }

        // Evento disparado quando o texto da SearchBar � alterado
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string query = e.NewTextValue;

                // Se o texto de busca estiver vazio, carrega todos os tempos
                if (string.IsNullOrWhiteSpace(query))
                {
                    await CarregarListaTempos();
                }
                else
                {
                    // Busca tempos que contenham o texto digitado na descri��o
                    List<Tempo> resultados = await App.Db.Search(query);
                    lst_tempos.ItemsSource = resultados;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro na busca: {ex.Message}", "OK");
            }
        }

        // Evento disparado quando o usu�rio puxa a lista para atualizar
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                // Recarrega a lista de tempos
                await CarregarListaTempos();

                // Para a anima��o de refresh
                lst_tempos.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                lst_tempos.IsRefreshing = false;
                await DisplayAlert("Erro", $"Erro ao atualizar: {ex.Message}", "OK");
            }
        }

        // Evento disparado quando o item "Remover" � clicado no menu de contexto
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                Tempo tempo = (Tempo)menuItem.CommandParameter;

                // Confirma se o usu�rio realmente quer deletar
                bool confirmacao = await DisplayAlert(
                    "Confirma��o",
                    $"Deseja realmente remover o tempo de {tempo.description}?",
                    "Sim",
                    "N�o"
                );

                if (confirmacao)
                {
                    // Remove o tempo do banco de dados
                    await App.Db.Delete(tempo.Id);

                    // Recarrega a lista
                    await CarregarListaTempos();

                    await DisplayAlert("Sucesso", "Tempo removido com sucesso!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao remover tempo: {ex.Message}", "OK");
            }
        }
    }
}