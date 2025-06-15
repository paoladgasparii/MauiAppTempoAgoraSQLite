using MauiAppTempoAgoraSQLite.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class NovoTempo : ContentPage
    {
        public double lat { get; set; }
        public double lon { get; set; }

        public NovoTempo()
        {
            InitializeComponent();
        }

        // Método executado quando o botão "Salvar" da toolbar é clicado
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verifica se foi inserida uma cidade
                if (string.IsNullOrEmpty(txt_cidade.Text))
                {
                    await DisplayAlert("Erro", "Digite o nome de uma cidade", "OK");
                    return;
                }

                // Busca a previsão do tempo para a cidade informada
                Tempo? tempo = await DataService.GetPrevisao(txt_cidade.Text);

                if (tempo != null)
                {
                    // Define a cidade no objeto tempo
                    tempo.cidade = txt_cidade.Text;

                    // Salva o tempo no banco de dados
                    await App.Db.Insert(tempo);

                    await DisplayAlert("Sucesso", "Tempo salvo com sucesso!", "OK");

                    // Retorna para a página anterior
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter a previsão do tempo para esta cidade.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
            }
        }

        // Método executado quando o botão "Minha Localização" é clicado
        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    lat = location.Latitude;
                    lon = location.Longitude;

                    lbl_coords.Text = $"Lat: {lat:F6}, Lon: {lon:F6}";

                    // Carrega o mapa com a localização
                    CarregarMapa();
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível obter sua localização.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao obter localização: {ex.Message}", "OK");
            }
        }

        // Método executado quando o botão "Buscar Previsão" é clicado
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_cidade.Text))
                {
                    await DisplayAlert("Erro", "Digite o nome de uma cidade", "OK");
                    return;
                }

                // Busca a previsão do tempo
                Tempo? tempo = await DataService.GetPrevisao(txt_cidade.Text);

                if (tempo != null)
                {
                    lat = tempo.lat ?? 0;
                    lon = tempo.lon ?? 0;

                    // Exibe as informações do tempo
                    lbl_res.Text = $"Cidade: {txt_cidade.Text}\n" +
                                   $"Temperatura Min: {tempo.temp_min:F1}°C\n" +
                                   $"Temperatura Max: {tempo.temp_max:F1}°C\n" +
                                   $"Descrição: {tempo.description}\n" +
                                   $"Velocidade do Vento: {tempo.speed:F1} m/s\n" +
                                   $"Visibilidade: {tempo.visibility} m\n" +
                                   $"Nascer do Sol: {tempo.sunrise}\n" +
                                   $"Pôr do Sol: {tempo.sunset}";

                    lbl_coords.Text = $"Lat: {lat:F6}, Lon: {lon:F6}";

                    // Carrega o mapa com a localização da cidade
                    CarregarMapa();
                }
                else
                {
                    lbl_res.Text = "Não foi possível obter a previsão do tempo para esta cidade.";
                }
            }
            catch (Exception ex)
            {
                lbl_res.Text = $"Erro: {ex.Message}";
            }
        }

        // Método para carregar o mapa no WebView
        private void CarregarMapa()
        {
            string html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Mapa</title>
                <style>
                    #map {{ height: 400px; width: 100%; }}
                    body {{ margin: 0; padding: 0; }}
                </style>
            </head>
            <body>
                <div id='map'></div>
                <script>
                    function initMap() {{
                        var location = {{ lat: {lat.ToString().Replace(',', '.')}, lng: {lon.ToString().Replace(',', '.')} }};
                        var map = new google.maps.Map(document.getElementById('map'), {{
                            zoom: 10,
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
                    src='https://maps.googleapis.com/maps/api/js?key=SUA_CHAVE_API&callback=initMap'>
                </script>
            </body>
            </html>";

            wv_mapa.Source = new HtmlWebViewSource { Html = html };
        }
    }
}