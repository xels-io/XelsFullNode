namespace Xels.SmartContracts.CLR
{
    /// <summary>
    /// Provides functionality to encode keys before they're stored in a KV store.
    /// </summary>
    public interface IKeyEncodingStrategy
    {
        byte[] GetBytes(byte[] key);
    }
}
