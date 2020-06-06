using System;

namespace TokenContracts
{
    public interface TokenRejected : BaseContract
    {
        string Token { get; }
        string Reason { get; }
    }
}
