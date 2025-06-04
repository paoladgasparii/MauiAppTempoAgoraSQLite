using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiAppTempoAgoraSQLite.Models;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        // Cria uma conexão assíncrona com o banco de dados SQLite
        readonly SQLiteAsyncConnection _conn;

        // Construtor da classe, inicializa a conexão com o banco de dados e cria a tabela 'Tempo'
        public SQLiteDatabaseHelper(string path)
        {
            // Inicializa a conexão com o banco de dados SQLite usando o caminho fornecido
            _conn = new SQLiteAsyncConnection(path);

            // Cria a tabela 'Produto' caso ainda não exista
            _conn.CreateTableAsync<Tempo>().Wait();
        }

        // Método para inserir um novo produto no banco de dados
        public Task<int> Insert(Tempo t)
        {
            // Insere o produto 'p' no banco de dados
            return _conn.InsertAsync(t);
        }

        // Método para atualizar um tempo existente no banco de dados
        public Task<List<Tempo>> Update(Tempo t)
        {
            // Comando SQL para atualizar o tempo com base no ID
            string sql = "UPDATE Tempo SET lon=?, " +
                "lat=?, temp_min=?, temp_max=?, visibility=?, speed=?, main=?, description=?, sunrise=?, sunset=? WHERE Id=?";

            // Executa o comando SQL com os parâmetros fornecidos
            return _conn.QueryAsync<Tempo>(
                sql, t.lon, t.lat, t.temp_min, t.temp_max, t.visibility, t.speed, t.main, t.description, t.sunrise, t.sunset, t.Id
            );
        }

        // Método para deletar um tempo com base no ID
        public Task<int> Delete(int id)
        {
            // Deleta o tempo com o ID especificado
            return _conn.Table<Tempo>().DeleteAsync(i => i.Id == id);
        }

        // Método para obter todos os produtos do banco de dados
        public Task<List<Tempo>> GetAll()
        {
            // Retorna todos os tempos da tabela 'Tempo'
            return _conn.Table<Tempo>().ToListAsync();
        }

        // Método para buscar tempos no banco de dados pela descricao
        public Task<List<Tempo>> Search(string q)
        {
            // Comando SQL para realizar a busca de tempos com a descrição que contenha o texto 'q'
            string sql = "SELECT * Tempo WHERE description LIKE '%" + q + "%'";

            // Executa a consulta SQL e retorna os resultados
            return _conn.QueryAsync<Tempo>(sql);
        }
    }
}
