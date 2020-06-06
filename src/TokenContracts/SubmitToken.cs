using System;

namespace TokenContracts
{
    public interface SubmitToken : BaseContract
    {
        string Token { get; }
    }
}
