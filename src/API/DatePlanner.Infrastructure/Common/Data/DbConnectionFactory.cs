using System.Data.Common;
using DatePlanner.Application.Common.Data;
using Npgsql;

namespace DatePlanner.Infrastructure.Common.Data;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await dataSource.OpenConnectionAsync();
    }
}
