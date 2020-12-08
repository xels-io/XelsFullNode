using System;

namespace Xels.Bitcoin.Features.Wallet.Interfaces
{
    public interface ITransactionContext : IDisposable
    {
        void Rollback();
        void Commit();
    }
}
