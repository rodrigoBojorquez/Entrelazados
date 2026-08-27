using DatePlanner.Application.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.Database;

public class DatePlannerDbContext(DbContextOptions<DatePlannerDbContext> options) : DbContext(options), IUnitOfWork
{
}