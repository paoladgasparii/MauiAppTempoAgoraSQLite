using MauiAppTempoAgoraSQLite.Helpers;

namespace MauiAppTempoAgoraSQLite
{
    // Classe principal do aplicativo que herda de Application
    public partial class App : Application
    {
        // Instância estática da classe SQLiteDatabaseHelper, que gerencia o banco de dados SQLite
        static SQLiteDatabaseHelper _db;

        // Propriedade que acessa a instância do banco de dados de forma preguiçosa (lazy loading)
        public static SQLiteDatabaseHelper Db
        {
            get
            {
                // Se a instância do banco de dados ainda não foi criada, cria a instância
                if (_db == null)
                {
                    // Define o caminho para o banco de dados no diretório de dados locais da aplicação
                    string path = Path.Combine(
                        Environment.GetFolderPath(
                          Environment.SpecialFolder.LocalApplicationData),
                        "banco_sqlite_tempos.db3"); // Nome do arquivo do banco de dados

                    // Cria uma nova instância de SQLiteDatabaseHelper passando o caminho do banco de dados
                    _db = new SQLiteDatabaseHelper(path);
                }

                // Retorna a instância do banco de dados
                return _db;
            }
        }

        // Construtor do aplicativo, onde a página principal é inicializada
        public App()
        {
            InitializeComponent(); // Inicializa os componentes do aplicativo

            // Define a página principal do aplicativo, utilizando uma NavigationPage com a tela de ListaTempo
            var listaTempo = new Views.ListaTempo();

            // Adiciona um botão na toolbar para adicionar novo tempo
            listaTempo.ToolbarItems.Add(new ToolbarItem
            {
                Text = "Adicionar",
                Command = new Command(async () =>
                {
                    await listaTempo.Navigation.PushAsync(new Views.NovoTempo());
                })
            });

            // Adiciona um botão na toolbar para remover tempos
            listaTempo.ToolbarItems.Add(new ToolbarItem
            {
                Text = "Remover",
                Command = new Command(async () =>
                {
                    await listaTempo.Navigation.PushAsync(new Views.RemoverTempo());
                })
            });

            MainPage = new NavigationPage(listaTempo);
        }
    }
}