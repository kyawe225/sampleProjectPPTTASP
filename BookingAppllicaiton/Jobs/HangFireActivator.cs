namespace BookingAppllicaiton.Jobs;

using System;
using Hangfire;

public class HangFireActivator : JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    public HangFireActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override object ActivateJob(Type type)
    {
        var _scope=_serviceProvider.CreateScope();
        return _scope.ServiceProvider.GetRequiredService(type);
    }
}