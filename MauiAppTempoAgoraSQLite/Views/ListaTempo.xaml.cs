using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiAppTempoAgoraSQLite.Views
{
    // Classe que representa a p�gina de lista de tempos
    public partial class ListaTempo : ContentPage
    {
        // ObservableCollection para armazenar os tempos e atualizar automaticamente a UI
        ObservableCollection<Tempo> lista = new ObservableCollection<Tempo>();

        // Construtor da p�gina, inicializa os componentes e associa a lista ao ItemsSource da ListView
        public ListaTempo()
        {
            InitializeComponent();
            lst_tempos.ItemsSource = lista; // Associa a lista de tempos � ListView
        }

        // M�todo que � chamado quando a p�gina aparece
        protected async override void OnAppearing()
        {
            try
            {
                // Obt�m todos os tempos do banco de dados e os adiciona � lista
                List<Tempo> tmp = await App.Db.GetAll();

                lista.Clear();
                tmp.ForEach(i => lista.Add(i)); // Atualiza a lista de tempos
            }
            catch (Exception ex)
            {
                // Exibe um alerta caso ocorra um erro
                await DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        // M�todo chamado quando a ListView � atualizada (pull-to-refresh)
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear(); // Limpa a lista antes de recarregar

                // Obt�m novamente os tempos e os adiciona � lista
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
                // Desativa o indicador de refresh ap�s a atualiza��o
                lst_tempos.IsRefreshing = false;
            }
        }

        // M�todo chamado quando o texto da barra de pesquisa muda
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue; // Obt�m o novo valor da pesquisa

            lista.Clear(); // Limpa a lista de tempos

            // Realiza a busca no banco de dados com o termo pesquisado
            List<Tempo> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i)); // Atualiza a lista com os tempos encontrados
        }

        // M�todo chamado quando um item da ListView � selecionado
        private void lst_tempos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Tempo t = e.SelectedItem as Tempo; // Obt�m o tempo selecionado

                if (t != null)
                {
                    string dados_previsao = "";

                    dados_previsao = $"Latitude: {t.lat} \n" +
                                     $"Longitude: {t.lon} \n" +
                                     $"Nascer do Sol: {t.sunrise} \n" +
                                     $"P�r do Sol: {t.sunset} \n" +
                                     $"Temp M�x: {t.temp_max} \n" +
                                     $"Temp M�n: {t.temp_min} \n";

                    string mapa = $"https://embed.windy.com/embed.html?" +
                    $"type=map&location=coordinates&metricRain=mm&metricTemp=�C" +
                    $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                    $"&lat={t.lat.ToString().Replace(",", ".")}&lon=" +
                    $"{t.lon.ToString().Replace(",", ".")}";

                    Debug.WriteLine(mapa);

                }
            }
            catch (Exception ex)
            {
                // Exibe um alerta em caso de erro
                DisplayAlert("Ops", ex.Message, "Ok");
            }
        }

        // M�todo chamado quando o item "Remover" de um tempo � clicado no menu de contexto
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem; // Obt�m o item do menu
                Tempo t = selecionado.BindingContext as Tempo; // Obt�m o tempo associado ao menu

                // Exibe uma confirma��o antes de remover o tempo
                bool confirm = await DisplayAlert(
                    "Tem certeza?", $"Remover dados de {t.cidade}?", "Sim", "N�o");

                if (confirm)
                {
                    // Remove o tempo do banco de dados e da lista
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