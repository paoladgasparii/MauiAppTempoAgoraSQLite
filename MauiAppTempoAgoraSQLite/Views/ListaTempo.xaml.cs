using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;

namespace MauiAppTempoAgoraSQLite.Views
{
    // Classe que representa a página de lista de tempos
    public partial class ListaTempo : ContentPage
    {
        // ObservableCollection para armazenar os produtos e atualizar automaticamente a UI
        ObservableCollection<Tempo> lista = new ObservableCollection<Tempo>();

        // Construtor da página, inicializa os componentes e associa a lista ao ItemsSource da ListView
        public ListaTempo()
        {
            InitializeComponent();
            lst_tempos.ItemsSource = lista; // Associa a lista de produtos à ListView
        }

        // Método que é chamado quando a página aparece
        protected async override void OnAppearing()
        {
            try
            {
                // Obtém todos os produtos do banco de dados e os adiciona à lista
                List<Tempo> tmp = await App.Db.GetAll();

                lista.Clear();
                tmp.ForEach(i => lista.Add(i)); // Atualiza a lista de produtos
            }
            catch (Exception ex)
            {
                // Exibe um alerta caso ocorra um erro
                await DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        // Método chamado quando a ListView é atualizada (pull-to-refresh)
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear(); // Limpa a lista antes de recarregar

                // Obtém novamente os produtos e os adiciona à lista
                List<Tempo> tmp = await App.Db.GetAll();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                // Exibe um alerta em caso de erro
                await DisplayAlert("Ops", ex.Message, "Ok");
            }
            finally
            {
                // Desativa o indicador de refresh após a atualização
                lst_tempos.IsRefreshing = false;
            }
        }

        // Método chamado quando o texto da barra de pesquisa muda
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue; // Obtém o novo valor da pesquisa

            lista.Clear(); // Limpa a lista de produtos

            // Realiza a busca no banco de dados com o termo pesquisado
            List<Tempo> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i)); // Atualiza a lista com os produtos encontrados
        }

        // Método chamado quando o item "Remover" de um produto é clicado no menu de contexto
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem; // Obtém o item do menu
                Tempo t = selecionado.BindingContext as Tempo; // Obtém o produto associado ao menu

                // Exibe uma confirmação antes de remover o produto
                bool confirm = await DisplayAlert(
                    "Tem certeza?", $"Remover {t.description}?", "Sim", "Não");

                if (confirm)
                {
                    // Remove o produto do banco de dados e da lista
                    await App.Db.Delete(t.Id);
                    lista.Remove(t);
                }
            }
            catch (Exception ex)
            {
                // Exibe um alerta caso ocorra um erro
                await DisplayAlert("Ops", ex.Message, "Ok");
            }
        }
    }
}
