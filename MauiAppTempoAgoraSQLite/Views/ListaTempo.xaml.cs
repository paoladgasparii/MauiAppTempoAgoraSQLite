using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiAppTempoAgoraSQLite.Views
{
    // Classe que representa a página de lista de tempos
    public partial class ListaTempo : ContentPage
    {
        // ObservableCollection para armazenar os tempos e atualizar automaticamente a UI
        ObservableCollection<Tempo> lista = new ObservableCollection<Tempo>();

        // Construtor da página, inicializa os componentes e associa a lista ao ItemsSource da ListView
        public ListaTempo()
        {
            InitializeComponent();
            lst_tempos.ItemsSource = lista; // Associa a lista de tempos à ListView
        }

        // Método que é chamado quando a página aparece
        protected async override void OnAppearing()
        {
            try
            {
                // Obtém todos os tempos do banco de dados e os adiciona à lista
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

        // Método chamado quando a ListView é atualizada (pull-to-refresh)
        private async void lst_tempos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear(); // Limpa a lista antes de recarregar

                // Obtém novamente os tempos e os adiciona à lista
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

            lista.Clear(); // Limpa a lista de tempos

            // Realiza a busca no banco de dados com o termo pesquisado
            List<Tempo> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i)); // Atualiza a lista com os tempos encontrados
        }

        // Método chamado quando um item da ListView é selecionado
        private void lst_tempos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Tempo t = e.SelectedItem as Tempo; // Obtém o tempo selecionado

                if (t != null)
                {
                    string dados_previsao = "";

                    dados_previsao = $"Latitude: {t.lat} \n" +
                                     $"Longitude: {t.lon} \n" +
                                     $"Nascer do Sol: {t.sunrise} \n" +
                                     $"Pôr do Sol: {t.sunset} \n" +
                                     $"Temp Máx: {t.temp_max} \n" +
                                     $"Temp Mín: {t.temp_min} \n";

                    string mapa = $"https://embed.windy.com/embed.html?" +
                    $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
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

        // Método chamado quando o item "Remover" de um tempo é clicado no menu de contexto
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem; // Obtém o item do menu
                Tempo t = selecionado.BindingContext as Tempo; // Obtém o tempo associado ao menu

                // Exibe uma confirmação antes de remover o tempo
                bool confirm = await DisplayAlert(
                    "Tem certeza?", $"Remover dados de {t.cidade}?", "Sim", "Não");

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