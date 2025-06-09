using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class NovoTempo : ContentPage
    {
        public NovoTempo()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Valida��o b�sica
                if (string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    await DisplayAlert("Aten��o", "Informe o nome da cidade", "OK");
                    return;
                }

                // Cria��o de um novo objeto Tempo
                Tempo t = new Tempo
                {
                    // Ajuste estas propriedades conforme sua classe Tempo
                    Cidade = txt_cidade.Text,
                    DataService = DateTime.Now
                    // Adicione outras propriedades necess�rias
                };

                // Insere o novo tempo no banco de dados
                await App.Db.Insert(t);

                // Exibe uma mensagem de sucesso
                await DisplayAlert("Sucesso!", "Registro Inserido", "Ok");

                // Limpa o campo ap�s salvar
                txt_cidade.Text = string.Empty;
                lbl_res.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Exibe um alerta com a mensagem de erro
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);
                    if (t != null)
                    {
                        string dados_previsao = $"Latitude: {t.lat} \n" +
                                               $"Longitude: {t.lon} \n" +
                                               $"Nascer do Sol: {t.sunrise} \n" +
                                               $"P�r do Sol: {t.sunset} \n" +
                                               $"Temp M�x: {t.temp_max} \n" +
                                               $"Temp M�n: {t.temp_min} \n";

                        lbl_res.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                    $"type=map&location=coordinates&metricRain=mm&metricTemp=�C" +
                                    $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                    $"&lat={t.lat.ToString().Replace(",", ".")}&lon=" +
                                    $"{t.lon.ToString().Replace(",", ".")}";

                        wv_mapa.Source = mapa;
                        Debug.WriteLine(mapa);
                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de Previs�o";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)
                );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                      $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_disp;

                    // Pega nome da cidade que est� nas coordenadas
                    await GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Nenhuma localiza��o";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo n�o suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localiza��o Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permiss�o da Localiza��o", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality ?? place.AdminArea ?? "Cidade n�o encontrada";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obten��o do nome da cidade", ex.Message, "OK");
            }
        }
    }
}