using MauiAppTempoAgora.Services;
using MauiAppTempoAgoraSQLite.Models;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class NovoTempo : ContentPage
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public Tempo tempo { get; set; }

        public NovoTempo()
        {
            InitializeComponent();
            tempo = new Tempo();
        }

        // Método para obter a localização atual do usuário
        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                // Solicita a localização atual
                var localizacao = await Geolocation.GetLocationAsync();

                if (localizacao != null)
                {
                    lat = localizacao.Latitude;
                    lon = localizacao.Longitude;

                    // Exibe as coordenadas na tela
                    lbl_coords.Text = $"Latitude: {lat:F6}, Longitude: {lon:F6}";

                    // Atualiza o mapa com a localização
                    string url_mapa = $"https://www.openstreetmap.org/export/embed.html?bbox={lon - 0.01},{lat - 0.01},{lon + 0.01},{lat + 0.01}&layer=mapnik&marker={lat},{lon}";
                    wv_mapa.Source = url_mapa;
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter a localização.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao obter localização: {ex.Message}", "OK");
            }
        }

        // Método para buscar a previsão do tempo
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a cidade foi digitada
                if (string.IsNullOrEmpty(txt_cidade.Text))
                {
                    await DisplayAlert("Erro", "Digite o nome da cidade.", "OK");
                    return;
                }

                // Busca a previsão do tempo para a cidade
                tempo = await DataService.GetPrevisao(txt_cidade.Text);

                if (tempo != null)
                {
                    // Armazena o nome da cidade digitada pelo usuário
                    tempo.cidade = txt_cidade.Text;

                    // Atualiza as coordenadas
                    lat = tempo.lat ?? 0;
                    lon = tempo.lon ?? 0;

                    // Exibe as informações do tempo
                    lbl_res.Text =$"Latitude: {tempo.lat}\n" +
                                  $"Longitude: {tempo.lon}\n" +
                                  $"Cidade: {tempo.cidade}\n" +
                                  $"Descrição: {tempo.description}\n" +
                                  $"Temperatura Min: {tempo.temp_min:F1}°C\n" +
                                  $"Temperatura Max: {tempo.temp_max:F1}°C\n" +
                                  $"Velocidade do Vento: {tempo.speed:F1} m/s\n" +
                                  $"Visibilidade: {tempo.visibility} m\n" +
                                  $"Nascer do Sol: {tempo.sunrise}\n" +
                                  $"Pôr do Sol: {tempo.sunset}";

                    // Exibe as coordenadas
                    lbl_coords.Text = $"Latitude: {lat:F6}, Longitude: {lon:F6}";

                    // Atualiza o mapa
                    string url_mapa = $"https://www.openstreetmap.org/export/embed.html?bbox={lon - 0.01},{lat - 0.01},{lon + 0.01},{lat + 0.01}&layer=mapnik&marker={lat},{lon}";
                    wv_mapa.Source = url_mapa;
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter os dados do tempo para esta cidade.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao buscar previsão: {ex.Message}", "OK");
            }
        }

        // Método para salvar o tempo no banco de dados
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verifica se há dados para salvar
                if (tempo == null || string.IsNullOrEmpty(tempo.cidade))
                {
                    await DisplayAlert("Erro", "Busque a previsão do tempo antes de salvar.", "OK");
                    return;
                }

                // Salva o tempo no banco de dados
                await App.Db.Insert(tempo);

                // Exibe mensagem de sucesso
                await DisplayAlert("Sucesso", "Tempo salvo com sucesso!", "OK");

                // Volta para a página anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
            }
        }
    }
}