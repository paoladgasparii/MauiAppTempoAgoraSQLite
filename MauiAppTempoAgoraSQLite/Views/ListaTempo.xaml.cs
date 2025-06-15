using MauiAppTempoAgoraSQLite.Models;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class ListaTempo : ContentPage
    {
        public ListaTempo()
        {
            InitializeComponent();
        }

        // Método chamado quando a página aparece
        protected async override void OnAppearing()
        {
            // Carrega todos os tempos do banco de dados na lista
            lst_tempos.ItemsSource = await App.Db.GetAll();
        }

        // Método chamado quando o usuário puxa para atualizar a lista
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            // Recarrega todos os tempos do banco de dados
            lst_tempos.ItemsSource = await App.Db.GetAll();

            // Finaliza o indicador de carregamento
            lst_tempos.IsRefreshing = false;
        }

        // Método chamado quando o texto da barra de pesquisa muda
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termoPesquisa = e.NewTextValue;

            if (string.IsNullOrEmpty(termoPesquisa))
            {
                // Se não há termo de pesquisa, mostra todos os tempos
                lst_tempos.ItemsSource = await App.Db.GetAll();
            }
            else
            {
                // Se há termo de pesquisa, filtra os resultados
                lst_tempos.ItemsSource = await App.Db.Search(termoPesquisa);
            }
        }

        // Método chamado quando um item da lista é selecionado
        private async void lst_tempos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

                // Limpa a seleção
                lst_tempos.SelectedItem = null;
            }
        }
    }