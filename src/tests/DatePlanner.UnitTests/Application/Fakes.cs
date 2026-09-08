using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.Users;

namespace DatePlanner.UnitTests.Application;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCalls { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCalls++;
        return Task.FromResult(1);
    }
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
}

internal sealed class FakeUserRepository(params User[] users) : IUserRepository
{
    private readonly List<User> _users = [.. users];

    public IReadOnlyCollection<User> Users => _users;

    public Task<User?> GetUserByEmailAsync(string email) =>
        Task.FromResult(_users.FirstOrDefault(user => user.Email == email));

    public Task<User?> GetUserByIdAsync(Guid userId) =>
        Task.FromResult(_users.FirstOrDefault(user => user.Id == userId));

    public Task<User?> GetUserByPartnerCodeAsync(string partnerCode) =>
        Task.FromResult(_users.FirstOrDefault(user => user.PartnerCode == partnerCode));

    public void Insert(User user) => _users.Add(user);
}

internal sealed class FakeCoupleRepository(params Couple[] couples) : ICoupleRepository
{
    private readonly List<Couple> _couples = [.. couples];

    public IReadOnlyCollection<Couple> Couples => _couples;

    public Task<Couple?> GetCoupleByIdAsync(Guid coupleId) =>
        Task.FromResult(_couples.FirstOrDefault(couple => couple.Id == coupleId));

    public Task<Couple?> GetCoupleByUserIdAsync(Guid userId) =>
        Task.FromResult(_couples.FirstOrDefault(couple =>
            couple.IsActive &&
            (couple.PartnerOneId == userId || couple.PartnerTwoId == userId)));

    public void Insert(Couple couple) => _couples.Add(couple);
}

internal sealed class FakeDatePlanRepository(params DatePlan[] datePlans) : IDatePlanRepository
{
    private readonly List<DatePlan> _datePlans = [.. datePlans];

    public IReadOnlyCollection<DatePlan> DatePlans => _datePlans;

    public Task<DatePlan?> GetDateByIdAsync(Guid dateId) =>
        Task.FromResult(_datePlans.FirstOrDefault(datePlan => datePlan.Id == dateId));

    public void Insert(DatePlan datePlan) => _datePlans.Add(datePlan);
}

internal sealed class FakeTurnGameRepository(params TurnGame[] turnGames) : ITurnGameRepository
{
    private readonly List<TurnGame> _turnGames = [.. turnGames];

    public IReadOnlyCollection<TurnGame> TurnGames => _turnGames;

    public Task<TurnGame?> GetTurnGameByIdAsync(Guid turnGameId) =>
        Task.FromResult(_turnGames.FirstOrDefault(turnGame => turnGame.Id == turnGameId));

    public void Insert(TurnGame turnGame) => _turnGames.Add(turnGame);
}
