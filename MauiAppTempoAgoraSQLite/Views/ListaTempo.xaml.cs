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
            // Carrega todos os tempos do banco de dados na lista
            lst_tempos.ItemsSource = await App.Db.GetAll();
        }

        // M�todo chamado quando o usu�rio puxa para atualizar a lista
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            // Recarrega todos os tempos do banco de dados
            lst_tempos.ItemsSource = await App.Db.GetAll();

            // Finaliza o indicador de carregamento
            lst_tempos.IsRefreshing = false;
        }

        // M�todo chamado quando o texto da barra de pesquisa muda
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termoPesquisa = e.NewTextValue;

            if (string.IsNullOrEmpty(termoPesquisa))
            {
                // Se n�o h� termo de pesquisa, mostra todos os tempos
                lst_tempos.ItemsSource = await App.Db.GetAll();
            }
            else
            {
                // Se h� termo de pesquisa, filtra os resultados
                lst_tempos.ItemsSource = await App.Db.Search(termoPesquisa);
            }
        }

        // M�todo chamado quando um item da lista � selecionado
        private async void lst_tempos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

                // Limpa a sele��o
                lst_tempos.SelectedItem = null;
            }
        }
    }