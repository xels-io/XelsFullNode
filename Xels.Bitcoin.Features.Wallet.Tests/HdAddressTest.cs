﻿using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using Xunit;

namespace Xels.Bitcoin.Features.Wallet.Tests
{
    public class HdAddressTest
    {
        [Fact]
        public void IsChangeAddressWithValidHdPathForChangeAddressReturnsTrue()
        {
            var address = new HdAddress
            {
                HdPath = "0/1/2/3/1"
            };

            bool result = address.IsChangeAddress();

            Assert.True(result);
        }

        [Fact]
        public void IsChangeAddressWithValidHdPathForNonChangeAddressReturnsFalse()
        {
            var address = new HdAddress
            {
                HdPath = "0/1/2/3/0"
            };

            bool result = address.IsChangeAddress();

            Assert.False(result);
        }

        [Fact]
        public void IsChangeAddressWithTextInHdPathReturnsFormatException()
        {
            Assert.Throws<FormatException>(() =>
            {
                var address = new HdAddress
                {
                    HdPath = "0/1/2/3/A"
                };

                bool result = address.IsChangeAddress();
            });
        }

        [Fact]
        public void IsChangeAddressWithInvalidHdPathThrowsFormatException()
        {
            Assert.Throws<FormatException>(() =>
            {
                var address = new HdAddress
                {
                    HdPath = "0/1/2"
                };

                bool result = address.IsChangeAddress();
            });
        }

        [Fact]
        public void IsChangeAddressWithEmptyHdPathThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
           {
               var address = new HdAddress
               {
                   HdPath = string.Empty
               };

               bool result = address.IsChangeAddress();
           });
        }

        [Fact]
        public void IsChangeAddressWithNulledHdPathThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var address = new HdAddress
                {
                    HdPath = null
                };

                bool result = address.IsChangeAddress();
            });
        }

        [Fact]
        public void UnspentTransactionsWithAddressHavingUnspentTransactionsReturnsUnspentTransactions()
        {
            var address = new HdAddress(new List<TransactionData> {
                    new TransactionData { Id = new uint256(15)},
                    new TransactionData { Id = new uint256(16), SpendingDetails = new SpendingDetails() },
                    new TransactionData { Id = new uint256(17)},
                    new TransactionData { Id = new uint256(18), SpendingDetails = new SpendingDetails() }
                });

            IEnumerable<TransactionData> result = address.UnspentTransactions();

            Assert.Equal(2, result.Count());
            Assert.Equal(new uint256(15), result.ElementAt(0).Id);
            Assert.Equal(new uint256(17), result.ElementAt(1).Id);
        }

        [Fact]
        public void UnspentTransactionsWithAddressNotHavingUnspentTransactionsReturnsEmptyList()
        {
            var address = new HdAddress(new List<TransactionData> {
                    new TransactionData { Id = new uint256(16), SpendingDetails = new SpendingDetails() },
                    new TransactionData { Id = new uint256(18), SpendingDetails = new SpendingDetails() }
                });

            IEnumerable<TransactionData> result = address.UnspentTransactions();

            Assert.Empty(result);
        }

        [Fact]
        public void UnspentTransactionsWithAddressWithoutTransactionsReturnsEmptyList()
        {
            var address = new HdAddress();

            IEnumerable<TransactionData> result = address.UnspentTransactions();

            Assert.Empty(result);
        }
    }
}
