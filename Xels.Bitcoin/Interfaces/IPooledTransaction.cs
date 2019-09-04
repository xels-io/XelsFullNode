using System.Threading.Tasks;
using NBitcoin;

namespace Xels.Bitcoin.Interfaces
{
    public interface IPooledTransaction
    {
        Task<Transaction> GetTransaction(uint256 trxid);
    }
}
