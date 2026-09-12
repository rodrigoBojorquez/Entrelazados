using Quartz;

namespace DatePlanner.Infrastructure.DatePlans;

[DisallowConcurrentExecution]
public sealed class DatePlanAttendanceJob(
    DatePlanAttendanceProcessor processor) : IJob
{
    public Task Execute(IJobExecutionContext context) =>
        processor.ProcessAsync(context.CancellationToken);
}
