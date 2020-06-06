using System;

namespace TokenContracts
{
    public interface BaseContract
    {
        Guid EventId { get; }
        DateTime Timestamp { get; }
    }
}
