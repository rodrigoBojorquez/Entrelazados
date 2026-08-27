using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Common.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
