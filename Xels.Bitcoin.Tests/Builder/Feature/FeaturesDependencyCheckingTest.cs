﻿using System;
using System.Threading.Tasks;
using Xels.Bitcoin.Builder;
using Xels.Bitcoin.Builder.Feature;
using Xels.Bitcoin.Configuration;
using Xels.Bitcoin.Features.Consensus;
using Xels.Bitcoin.Tests.Common;
using Xunit;

namespace Xels.Bitcoin.Tests.Builder.Feature
{
    /// <summary>
    /// Tests checking for feature dependencies.
    /// </summary>
    public class FeaturesDependencyCheckingTest
    {
        #region Mock Features

        /// <summary>
        /// A mock feature.
        /// </summary>
        private class FeatureBase : IFullNodeFeature
        {
            /// <inheritdoc />
            public bool InitializeBeforeBase { get; set; }

            public string State { get; set; }

            public void LoadConfiguration()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Task InitializeAsync()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void ValidateDependencies(IFullNodeServiceProvider services)
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// A mock feature.
        /// </summary>
        private class FeatureB : FeatureBase
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// A mock feature.
        /// </summary>
        private class FeatureA : FeatureBase
        {
        }

        #endregion Mock Features

        /// <summary>
        /// Test no exceptions fired when checking features that exist.
        /// </summary>
        [Fact]
        public void DependencyCheckWithValidDependencies()
        {
            IFullNodeBuilder builder = new FullNodeBuilder().UseNodeSettings(NodeSettings.Default(KnownNetworks.XelsRegTest));

            builder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<FeatureB>();
            });

            builder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<FeatureA>()
                    .DependOn<FeatureBase>();
            });

            builder.UsePosConsensus().Build();
        }

        /// <summary>
        /// Test that missing feature throws exception.
        /// </summary>
        [Fact]
        public void DependencyCheckWithInvalidDependenciesThrowsException()
        {
            IFullNodeBuilder builder = new FullNodeBuilder().UseNodeSettings(NodeSettings.Default(KnownNetworks.XelsRegTest));
            builder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<FeatureA>()
                    .DependOn<FeatureB>();
            });

            Assert.Throws<MissingDependencyException>(() =>
            {
                builder.Build();
            });
        }
    }
}