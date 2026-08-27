using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Common.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
