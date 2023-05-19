using Coravel.Scheduling.Schedule.Interfaces;

namespace ZoaIdsBackend.Common.Interfaces;

public interface ISchedulerConfigurator
{
    public Action<IScheduler> ConfigureScheduler();
}
