using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class RemoverTempo : ContentPage
    {
        private ObservableCollection<Tempo> _todosTempos = new ObservableCollection<Tempo>();
        private ObservableCollection<Tempo> _temposFiltrados = new ObservableCollection<Tempo>();
        private List<Tempo> _temposSelecionados = new List<Tempo>();

        public RemoverTempo()
        {
            InitializeComponent();
            cv_tempos.ItemsSource = _temposFiltrados;
            CarregarTempos();
        }

        private async void CarregarTempos()
        {
            try
            {
                var tempos = await App.Db.GetAll();

                _todosTempos.Clear();
                _temposFiltrados.Clear();

                foreach (var tempo in tempos)
                {
                    _todosTempos.Add(tempo);
                    _temposFiltrados.Add(tempo);
                }

                AtualizarLabelSelecionados();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar tempos: {ex.Message}", "OK");
            }
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? "";

            _temposFiltrados.Clear();

            var temposFiltrados = string.IsNullOrWhiteSpace(searchText)
                ? _todosTempos
                : _todosTempos.Where(t =>
                    (t.cidade?.ToLower().Contains(searchText) ?? false) ||
                    (t.description?.ToLower().Contains(searchText) ?? false));

            foreach (var tempo in temposFiltrados)
            {
                _temposFiltrados.Add(tempo);
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var tempo = checkBox?.BindingContext as Tempo;

            if (tempo != null)
            {
                if (e.Value)
                {
                    if (!_temposSelecionados.Contains(tempo))
                    {
                        _temposSelecionados.Add(tempo);
                    }
                }
                else
                {
                    _temposSelecionados.Remove(tempo);
                }

                AtualizarLabelSelecionados();
            }
        }

        private void cv_tempos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Não usado mais, pois estamos usando CheckBox individualmente
        }

        private void AtualizarLabelSelecionados()
        {
            var count = _temposSelecionados.Count;
            lbl_selecionados.Text = count == 0
                ? "Nenhum item selecionado"
                : $"{count} item(ns) selecionado(s)";
        }

        private async void Button_Clicked_RemoverSelecionados(object sender, EventArgs e)
        {
            if (_temposSelecionados.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado para remoção.", "OK");
                return;
            }

            var confirmacao = await DisplayAlert("Confirmação",
                $"Deseja realmente remover {_temposSelecionados.Count} item(ns) selecionado(s)?",
                "Sim", "Não");

            if (confirmacao)
            {
                await RemoverItensSelecionados();
            }
        }

        private async void Button_Clicked_SelecionarTodos(object sender, EventArgs e)
        {
            _temposSelecionados.Clear();
            _temposSelecionados.AddRange(_temposFiltrados);

            // Atualizar visualmente todos os checkboxes
            await AtualizarCheckboxes(true);
            AtualizarLabelSelecionados();
        }

        private async void Button_Clicked_LimparSelecao(object sender, EventArgs e)
        {
            _temposSelecionados.Clear();

            // Atualizar visualmente todos os checkboxes
            await AtualizarCheckboxes(false);
            AtualizarLabelSelecionados();
        }

        private async Task AtualizarCheckboxes(bool isChecked)
        {
            // Force a refresh da CollectionView para atualizar os checkboxes
            var source = cv_tempos.ItemsSource;
            cv_tempos.ItemsSource = null;
            cv_tempos.ItemsSource = source;
        }

        private async Task RemoverItensSelecionados()
        {
            try
            {
                foreach (var tempo in _temposSelecionados.ToList())
                {
                    await App.Db.Delete(tempo.Id);
                    _todosTempos.Remove(tempo);
                    _temposFiltrados.Remove(tempo);
                }

                _temposSelecionados.Clear();
                AtualizarLabelSelecionados();

                await DisplayAlert("Sucesso", "Itens removidos com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao remover itens: {ex.Message}", "OK");
            }
        }

        private async void ToolbarItem_Clicked_Confirmar(object sender, EventArgs e)
        {
            if (_temposSelecionados.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado para remoção.", "OK");
                return;
            }

            var confirmacao = await DisplayAlert("Confirmação Final",
                $"Esta ação irá remover permanentemente {_temposSelecionados.Count} item(ns). Continuar?",
                "Confirmar", "Cancelar");

            if (confirmacao)
            {
                await RemoverItensSelecionados();
                await Navigation.PopAsync(); // Voltar para a tela anterior
            }
        }
    }
}