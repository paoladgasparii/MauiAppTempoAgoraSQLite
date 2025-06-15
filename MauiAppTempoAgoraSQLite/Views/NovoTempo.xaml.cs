using MauiAppTempoAgoraSQLite.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class NovoTempo : ContentPage
    {
        private Tempo tempoAtual;

        public NovoTempo()
        {
            InitializeComponent();
            tempoAtual = new Tempo();
        }

        // Botão para obter localização atual do usuário
        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                // Solicita permissão de localização
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    lbl_coords.Text = $"Lat: {location.Latitude}, Lon: {location.Longitude}";

                    // Armazena as coordenadas no objeto tempo
                    tempoAtual.lat = location.Latitude;
                    tempoAtual.lon = location.Longitude;

                    // Carrega o mapa com a localização atual
                    CarregarMapa(location.Latitude, location.Longitude);
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter a localização", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao obter localização: {ex.Message}", "OK");
            }
        }

        // Botão para buscar previsão do tempo por cidade
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                string cidade = txt_cidade.Text?.Trim();

                if (string.IsNullOrEmpty(cidade))
                {
                    await DisplayAlert("Aviso", "Digite o nome da cidade", "OK");
                    return;
                }

                // Busca a previsão do tempo através da API
                Tempo tempo = await DataService.GetPrevisao(cidade);

                if (tempo != null)
                {
                    tempoAtual = tempo;

                    // Exibe as informações na label
                    lbl_res.Text = $"Cidade: {cidade}\n" +
                                  $"Temperatura: {tempo.temp_min}°C - {tempo.temp_max}°C\n" +
                                  $"Condição: {tempo.main}\n" +
                                  $"Descrição: {tempo.description}\n" +
                                  $"Vento: {tempo.speed} m/s\n" +
                                  $"Visibilidade: {tempo.visibility}m\n" +
                                  $"Nascer do sol: {tempo.sunrise}\n" +
                                  $"Pôr do sol: {tempo.sunset}";

                    // Carrega o mapa com a localização da cidade
                    if (tempo.lat.HasValue && tempo.lon.HasValue)
                    {
                        CarregarMapa(tempo.lat.Value, tempo.lon.Value);
                        lbl_coords.Text = $"Lat: {tempo.lat}, Lon: {tempo.lon}";
                    }
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter a previsão do tempo para esta cidade", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao buscar previsão: {ex.Message}", "OK");
            }
        }

        // Método para carregar o mapa no WebView
        private void CarregarMapa(double latitude, double longitude)
        {
            string html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{ margin: 0; padding: 0; }}
                        #map {{ height: 100vh; width: 100%; }}
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        function initMap() {{
                            var location = {{ lat: {latitude.ToString().Replace(',', '.')}, lng: {longitude.ToString().Replace(',', '.')} }};
                            var map = new google.maps.Map(document.getElementById('map'), {{
                                zoom: 12,
                                center: location
                            }});
                            var marker = new google.maps.Marker({{
                                position: location,
                                map: map,
                                title: 'Localização'
                            }});
                        }}
                    </script>
                    <script async defer 
                        src='https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&callback=initMap'>
                    </script>
                </body>
                </html>";

            wv_mapa.Source = new HtmlWebViewSource { Html = html };
        }

        // Evento do botão Salvar na toolbar
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verifica se há dados de tempo para salvar
                if (tempoAtual == null ||
                    (string.IsNullOrEmpty(tempoAtual.description) &&
                     !tempoAtual.lat.HasValue &&
                     !tempoAtual.lon.HasValue))
                {
                    await DisplayAlert("Aviso", "Busque uma previsão do tempo ou obtenha sua localização antes de salvar", "OK");
                    return;
                }

                // Salva o tempo no banco de dados
                await App.Db.Insert(tempoAtual);

                await DisplayAlert("Sucesso", "Tempo salvo com sucesso!", "OK");

                // Volta para a página anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar tempo: {ex.Message}", "OK");
            }
        }
    }
}