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

        // Bot�o para obter localiza��o atual do usu�rio
        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                // Solicita permiss�o de localiza��o
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

                    // Carrega o mapa com a localiza��o atual
                    CarregarMapa(location.Latitude, location.Longitude);
                }
                else
                {
                    await DisplayAlert("Erro", "N�o foi poss�vel obter a localiza��o", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao obter localiza��o: {ex.Message}", "OK");
            }
        }

        // Bot�o para buscar previs�o do tempo por cidade
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

                // Busca a previs�o do tempo atrav�s da API
                Tempo tempo = await DataService.GetPrevisao(cidade);

                if (tempo != null)
                {
                    tempoAtual = tempo;

                    // Exibe as informa��es na label
                    lbl_res.Text = $"Cidade: {cidade}\n" +
                                  $"Temperatura: {tempo.temp_min}�C - {tempo.temp_max}�C\n" +
                                  $"Condi��o: {tempo.main}\n" +
                                  $"Descri��o: {tempo.description}\n" +
                                  $"Vento: {tempo.speed} m/s\n" +
                                  $"Visibilidade: {tempo.visibility}m\n" +
                                  $"Nascer do sol: {tempo.sunrise}\n" +
                                  $"P�r do sol: {tempo.sunset}";

                    // Carrega o mapa com a localiza��o da cidade
                    if (tempo.lat.HasValue && tempo.lon.HasValue)
                    {
                        CarregarMapa(tempo.lat.Value, tempo.lon.Value);
                        lbl_coords.Text = $"Lat: {tempo.lat}, Lon: {tempo.lon}";
                    }
                }
                else
                {
                    await DisplayAlert("Erro", "N�o foi poss�vel obter a previs�o do tempo para esta cidade", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao buscar previs�o: {ex.Message}", "OK");
            }
        }

        // M�todo para carregar o mapa no WebView
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
                                title: 'Localiza��o'
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

        // Evento do bot�o Salvar na toolbar
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verifica se h� dados de tempo para salvar
                if (tempoAtual == null ||
                    (string.IsNullOrEmpty(tempoAtual.description) &&
                     !tempoAtual.lat.HasValue &&
                     !tempoAtual.lon.HasValue))
                {
                    await DisplayAlert("Aviso", "Busque uma previs�o do tempo ou obtenha sua localiza��o antes de salvar", "OK");
                    return;
                }

                // Salva o tempo no banco de dados
                await App.Db.Insert(tempoAtual);

                await DisplayAlert("Sucesso", "Tempo salvo com sucesso!", "OK");

                // Volta para a p�gina anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar tempo: {ex.Message}", "OK");
            }
        }
    }
}