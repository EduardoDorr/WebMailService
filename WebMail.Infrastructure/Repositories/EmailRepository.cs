using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using WebMail.Domain.Models;
using WebMail.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebMail.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ILogger<EmailRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public EmailRepository(IConfiguration configuration, ILogger<EmailRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration["DatabaseSettings:ConnectionString"];
            _logger = logger;
        }

        public async Task<int> CreateEmail(Email email)
        {
            _logger.LogInformation("Requisição para criação de um e-mail", email);
            var query = new StringBuilder();
            query.Clear()
                 .AppendLine("INSERT INTO SendEmail ")
                 .AppendLine("OUTPUT Inserted.ID    ")
                 .AppendLine("VALUES                ")
                 .AppendLine("  (@Origin,           ")
                 .AppendLine("   @Destiny,          ")
                 .AppendLine("   @Subject,          ")
                 .AppendLine("   @Body,             ")
                 .AppendLine("   @GenerationDate,   ")
                 .AppendLine("   NULL)              ");

            var parameters = new { email.Origin, email.Destiny, email.Subject, email.Body, email.GenerationDate };

            using var sqlConnection = new SqlConnection(_connectionString);
            
            return await sqlConnection.ExecuteScalarAsync<int>(query.ToString(), parameters);
        }

        public async Task<Email> GetEmailById(int id)
        {
            _logger.LogInformation("Requisição para buscar um e-mail de id {id}", id);
            var query = new StringBuilder();
            query.Clear()
                 .AppendLine("SELECT TOP 1  ")
                 .AppendLine("  *           ")
                 .AppendLine("FROM          ")
                 .AppendLine("  SendEmail   ")
                 .AppendLine("WHERE         ")
                 .AppendLine("  Id = @Id    ");

            var parameters = new { id };

            using var sqlConnection = new SqlConnection(_connectionString);

            return await sqlConnection.QueryFirstOrDefaultAsync<Email>(query.ToString(), parameters);
        }

        public async Task<IEnumerable<Email>> GetEmails()
        {
            var query = new StringBuilder();
            query.Clear()
                 .AppendLine("SELECT        ")
                 .AppendLine("  *           ")
                 .AppendLine("FROM          ")
                 .AppendLine("  SendEmail   ");

            using var sqlConnection = new SqlConnection(_connectionString);

            return await sqlConnection.QueryAsync<Email>(query.ToString());
        }

        public async Task<IEnumerable<Email>> GetEmailsNotSended()
        {
            _logger.LogInformation("Busca por e-mails não enviados");
            var query = new StringBuilder();
            query.Clear()
                 .AppendLine("SELECT                ")
                 .AppendLine("  *                   ")
                 .AppendLine("FROM                  ")
                 .AppendLine("  SendEmail           ")
                 .AppendLine("WHERE                 ")
                 .AppendLine("  SendDate is NULL    ");

            using var sqlConnection = new SqlConnection(_connectionString);

            return await sqlConnection.QueryAsync<Email>(query.ToString());
        }

        public async Task<bool> UpdateEmail(int id)
        {
            _logger.LogInformation("E-mail teve a data de envio atualizada", id);
            var query = new StringBuilder();
            query.Clear()
                 .AppendLine("UPDATE                    ")
                 .AppendLine("  SendEmail               ")
                 .AppendLine("SET                       ")
                 .AppendLine("  SendDate = @SendDate    ")
                 .AppendLine("WHERE                     ")
                 .AppendLine("  Id = @Id                ");

            var parameters = new { SendDate = DateTime.Now, Id = id };

            using var sqlConnection = new SqlConnection(_connectionString);

            return await sqlConnection.ExecuteAsync(query.ToString(), parameters) > 0;
        }
    }
}
