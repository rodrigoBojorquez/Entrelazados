using System.Data.Common;

namespace DatePlanner.Application.Common.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}