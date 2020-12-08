using System;
using System.Linq;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Xels.Bitcoin.Features.Wallet.Controllers
{
    /// <summary>
    /// Xels X has a version prefix starting xq5h. Full node has a version prefix xpub for extended public key. 
    /// 3rd parties like Ledger haven't update yet to the full node version prefix.
    /// This class provides a way to convert from the legacy format to the current format.
    /// </summary>
    /// <remarks>This class can be removed when the 3rd parties update their software.</remarks>
    public class LegacyExtPubKeyConverter
    {
        /// <summary>
        /// Converts a legacy Xels format into a current Xels format Base58 extended public key.
        /// </summary>
        /// <param name="extPubKey">The extended public key that may or may not need converting.</param>
        /// <param name="network">The network to get the version bytes for.</param>
        /// <returns>The same extended public key if version bytes is already the Xels full node one,
        /// or the corrected extended public key if the version bytes was the Xels X one.</returns>
        public static string ConvertIfInLegacyXelsFormat(string extPubKey, Network network)
        {
            byte[] xelsVersionBytes = network.GetVersionBytes(Base58Type.EXT_PUBLIC_KEY, true);
            byte[] extPubKeyBytes = Encoders.Base58Check.DecodeData(extPubKey);

            if (IsXelsExtPubKey(extPubKeyBytes, xelsVersionBytes))
            {
                return extPubKey;
            }

            if (IsLegacyXelsExtpubKey(extPubKeyBytes))
            {
                return ReplaceBase58Prefix(extPubKeyBytes, network.GetVersionBytes(Base58Type.EXT_PUBLIC_KEY, true));
            }

            throw new FormatException($"ExtPubKey {extPubKey} could not be parsed.");
        }

        private static bool IsXelsExtPubKey(byte[] extPubKey, byte[] xelsVersionBytes)
        {
            byte[] version = extPubKey.Take(4).ToArray();
            return version.SequenceEqual(xelsVersionBytes);
        }

        private static bool IsLegacyXelsExtpubKey(byte[] extPubKey)
        {
            var legacyXelsVersionBytes = new byte[] { (0x04), (0x88), (0xC2), (0x1E) };
            byte[] version = extPubKey.Take(4).ToArray();
            return version.SequenceEqual(legacyXelsVersionBytes);
        }

        private static string ReplaceBase58Prefix(byte[] extPubKeyBytes, byte[] replacement)
        {
            byte[] extPubKeyWithoutVersionsBytes = extPubKeyBytes.Skip(4).ToArray();
            string converted = Encoders.Base58Check.EncodeData(replacement
                .Concat(extPubKeyWithoutVersionsBytes)
                .ToArray());
            return converted;
        }
    }
}