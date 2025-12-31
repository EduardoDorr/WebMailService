using System.Text;

using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using WebMail.Domain.Entities;
using WebMail.Domain.Repositories;

namespace WebMail.Infrastructure.Repositories;

public class EmailRepository : IEmailRepository
{
    private const int MaxRetries = 5;
    private readonly string? _connectionString;

    public EmailRepository(IConfiguration configuration)
    {
        _connectionString = configuration["DatabaseSettings:ConnectionString"];
    }

    public async Task<int> CreateAsync(Email email)
    {
        var query = new StringBuilder();
        query.Clear()
             .AppendLine("INSERT INTO SendEmail ")
             .AppendLine("OUTPUT Inserted.ID    ")
             .AppendLine("VALUES                ")
             .AppendLine("  (@Origin,           ")
             .AppendLine("   @Destiny,          ")
             .AppendLine("   @Subject,          ")
             .AppendLine("   @Body,             ")
             .AppendLine("   @Attachment,       ")
             .AppendLine("   @GenerationDate,   ")
             .AppendLine("   NULL)              ");

        var parameters = new { email.Origin, email.Destiny, email.Subject, email.Body, email.Attachment, email.GenerationDate };

        using var sqlConnection = new SqlConnection(_connectionString);

        return await sqlConnection.ExecuteScalarAsync<int>(query.ToString(), parameters);
    }

    public async Task<Email?> GetByIdAsync(int id)
    {
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

    public async Task<IEnumerable<Email>> GetAllAsync()
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

    public async Task<IEnumerable<Email>> GetAllNotSendedAsync()
    {
        var query = new StringBuilder();
        query.Clear()
             .AppendLine("SELECT                        ")
             .AppendLine("  *                           ")
             .AppendLine("FROM                          ")
             .AppendLine("  SendEmail                   ")
             .AppendLine("WHERE                         ")
             .AppendLine("  SendDate is NULL            ")
             .AppendLine("  AND Sended = 0              ")
             .AppendLine("  AND Retries < @MaxRetries   ");

        var parameters = new { MaxRetries };

        using var sqlConnection = new SqlConnection(_connectionString);

        return await sqlConnection.QueryAsync<Email>(query.ToString(), parameters);
    }

    public async Task<bool> IncrementAttemptsAsync(int id)
    {
        var query = new StringBuilder();
        query.Clear()
             .AppendLine("UPDATE                                ")
             .AppendLine("  SendEmail                           ")
             .AppendLine("SET                                   ")
             .AppendLine("  Retries = Retries + 1,              ")
             .AppendLine("  LastAttemptDate = @LastAttemptDate  ")
             .AppendLine("WHERE                                 ")
             .AppendLine("  Id = @Id                            ");

        var parameters = new
        {
            LastAttemptDate = DateTime.UtcNow,
            Id = id
        };

        using var sqlConnection = new SqlConnection(_connectionString);

        return await sqlConnection.ExecuteAsync(query.ToString(), parameters) > 0;
    }

    public async Task<bool> MarkAsSentAsync(int id)
    {
        var query = new StringBuilder();
        query.Clear()
             .AppendLine("UPDATE                                ")
             .AppendLine("  SendEmail                           ")
             .AppendLine("SET                                   ")
             .AppendLine("  SendDate = @SendDate,               ")
             .AppendLine("  Sent = 1,                           ")
             .AppendLine("  LastAttemptDate = @LastAttemptDate  ")
             .AppendLine("WHERE                                 ")
             .AppendLine("  Id = @Id                            ");

        var parameters = new
        {
            SendDate = DateTime.UtcNow,
            LastAttemptDate = DateTime.UtcNow,
            Id = id
        };

        using var sqlConnection = new SqlConnection(_connectionString);

        return await sqlConnection.ExecuteAsync(query.ToString(), parameters) > 0;
    }
}