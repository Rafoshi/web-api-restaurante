using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_api_restaurante.Entidades;

namespace web_api_restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly string? _connectionString;

        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            var result = await dbConnection.QueryAsync<Produto>("select id, nome, descricao, imagemUrl from Produto;");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var produto = await dbConnection.QueryAsync<Produto>("select id, nome, descricao, imagemUrl from produto where id = @id;", new { id });
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();

            dbConnection.Execute("insert into Produto(nome, descricao, imagemUrl) values(@Nome, @Descricao, @ImagemUrl)", produto);
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Produto produto)
        {

            using IDbConnection dbConnection = OpenConnection();

            // Atualiza o produto
            var query = @"UPDATE Produto SET 
                          Nome = @Nome,
                          Descricao = @Descricao,
                          ImagemUrl = @ImagemUrl
                          WHERE Id = @Id";

            dbConnection.Execute(query, produto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var produto = await dbConnection.QueryAsync<Produto>("delete from produto where id = @id;", new { id });
            return Ok();
        }
    }
}
