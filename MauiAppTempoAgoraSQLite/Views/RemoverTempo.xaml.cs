using MauiAppTempoAgoraSQLite.Models;
using System.Collections.ObjectModel;

namespace MauiAppTempoAgoraSQLite.Views
{
    public partial class RemoverTempo : ContentPage
    {
        private ObservableCollection<Tempo> todosTempos;
        private List<Tempo> temposSelecionados;

        public RemoverTempo()
        {
            InitializeComponent();
            todosTempos = new ObservableCollection<Tempo>();
            temposSelecionados = new List<Tempo>();
            cv_tempos.ItemsSource = todosTempos;
        }

        // Método chamado quando a página aparece
        protected async override void OnAppearing()
        {
            await CarregarTempos();
        }

        // Carrega todos os tempos do banco de dados
        private async Task CarregarTempos()
        {
            try
            {
                var tempos = await App.Db.GetAll();
                todosTempos.Clear();

                foreach (var tempo in tempos)
                {
                    todosTempos.Add(tempo);
                }

                AtualizarContadorSelecionados();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar tempos: {ex.Message}", "OK");
            }
        }

        // Método chamado quando o texto da barra de pesquisa muda
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termoPesquisa = e.NewTextValue;

            try
            {
                List<Tempo> temposFiltrados;

                if (string.IsNullOrEmpty(termoPesquisa))
                {
                    // Se não há termo de pesquisa, mostra todos os tempos
                    temposFiltrados = await App.Db.GetAll();
                }
                else
                {
                    // Se há termo de pesquisa, filtra os resultados
                    temposFiltrados = await App.Db.Search(termoPesquisa);
                }

                todosTempos.Clear();
                foreach (var tempo in temposFiltrados)
                {
                    todosTempos.Add(tempo);
                }

                // Limpa a seleção ao filtrar
                temposSelecionados.Clear();
                AtualizarContadorSelecionados();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao pesquisar: {ex.Message}", "OK");
            }
        }

        // Método chamado quando a seleção da CollectionView muda
        private void cv_tempos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            temposSelecionados.Clear();

            if (e.CurrentSelection != null)
            {
                foreach (Tempo tempo in e.CurrentSelection)
                {
                    temposSelecionados.Add(tempo);
                }
            }

            AtualizarContadorSelecionados();
        }

        // Método chamado quando um CheckBox é marcado/desmarcado
        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            AtualizarContadorSelecionados();
        }

        // Atualiza o contador de itens selecionados
        private void AtualizarContadorSelecionados()
        {
            int quantidadeSelecionados = temposSelecionados.Count;

            if (quantidadeSelecionados == 0)
            {
                lbl_selecionados.Text = "Nenhum item selecionado";
                lbl_selecionados.TextColor = Colors.Gray;
            }
            else
            {
                lbl_selecionados.Text = $"{quantidadeSelecionados} item(ns) selecionado(s)";
                lbl_selecionados.TextColor = Colors.Blue;
            }
        }

        // Seleciona todos os itens
        private void Button_Clicked_SelecionarTodos(object sender, EventArgs e)
        {
            cv_tempos.SelectionMode = SelectionMode.Multiple;
            cv_tempos.SelectedItems.Clear();

            foreach (var tempo in todosTempos)
            {
                cv_tempos.SelectedItems.Add(tempo);
            }
        }

        // Desmarca todos os itens
        private void Button_Clicked_DesmarcarTodos(object sender, EventArgs e)
        {
            cv_tempos.SelectedItems.Clear();
            temposSelecionados.Clear();
            AtualizarContadorSelecionados();
        }

        // Remove os itens selecionados
        private async void Button_Clicked_RemoverSelecionados(object sender, EventArgs e)
        {
            if (temposSelecionados.Count == 0)
            {
                await DisplayAlert("Aviso", "Selecione pelo menos um item para remover.", "OK");
                return;
            }
        }

        // Confirma a remoção via toolbar
        private async void ToolbarItem_Clicked_Confirmar(object sender, EventArgs e)
        {
            if (temposSelecionados.Count == 0)
            {
                await DisplayAlert("Aviso", "Selecione pelo menos um item para remover.", "OK");
                return;
            }

            bool confirmacao = await DisplayAlert("Confirmar Remoção",
                $"Deseja remover {temposSelecionados.Count} item(ns) selecionado(s)?",
                "Sim", "Não");

            if (confirmacao)
            {
                await RemoverItensSelecionados();
            }
        }

        // Remove os itens selecionados do banco de dados
        private async Task RemoverItensSelecionados()
        {
            try
            {
                int removidos = 0;

                foreach (var tempo in temposSelecionados.ToList())
                {
                    await App.Db.Delete(tempo.Id);
                    removidos++;
                }

                await DisplayAlert("Sucesso", $"{removidos} item(ns) removido(s) com sucesso!", "OK");

                // Recarrega a lista
                await CarregarTempos();

                // Limpa a seleção
                cv_tempos.SelectedItems.Clear();
                temposSelecionados.Clear();
                AtualizarContadorSelecionados();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao remover itens: {ex.Message}", "OK");
            }
        }
    }
}