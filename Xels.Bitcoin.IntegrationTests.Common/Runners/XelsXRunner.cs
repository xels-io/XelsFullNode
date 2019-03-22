using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Xels.Bitcoin.Networks;

namespace Xels.Bitcoin.IntegrationTests.Common.Runners
{
    public sealed class XelsXRunner : NodeRunner
    {
        private readonly string xelsDPath;
        private Process process;

        public XelsXRunner(string dataDir, string xelsDPath) : base(dataDir, null)
        {
            this.xelsDPath = xelsDPath;
            this.Network = new XelsRegTest();
        }

        public new bool IsDisposed
        {
            get
            {
                return this.process == null || (this.process?.HasExited == true);
            }
        }

        public override void Stop()
        {
            TimeSpan duration = TimeSpan.FromSeconds(30);
            TestHelper.WaitLoop(() =>
            {
                try
                {
                    if (this.IsDisposed) return true;

                    this.process.Kill();
                    this.process.WaitForExit(15000);

                    return false;
                }
                catch
                {
                    return false;
                }
            }, cancellationToken: new CancellationTokenSource(duration).Token,
                failureReason: $"Failed to kill {this.GetType()} process number:{this.process.Id} within {duration} seconds");
        }

        public override void Start()
        {
            TimeSpan duration = TimeSpan.FromSeconds(15);

            // The complete path xelsd uses to locate (e.g.) the block files consists of the source code build folder,
            // the relative path within the test case folders, and the xelsd network-specific path to its block database
            // This adds roughly 37 characters onto the full data folder path: \regtest\blocks\index/MANIFEST-000001

            // By throwing here we avoid a pointless 5-minute wait for xelsd to start up (it will 'start' and then soon
            // crash, which results in the getblockhash RPC call timing out later on in the startup sequence).
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (new FileInfo(this.DataFolder).FullName.Length > 222))
                throw new Exception("Path is too long for xelsd to function.");

            TestHelper.WaitLoop(() =>
            {
                try
                {
                    this.process = Process.Start(new FileInfo(this.xelsDPath).FullName,
                        $"-conf=xels.conf -datadir={this.DataFolder}");
                    return true;
                }
                catch
                {
                    return false;
                }
            }, cancellationToken: new CancellationTokenSource(duration).Token,
                failureReason: $"Failed to start XelsD within {duration} seconds");
        }

        public override void BuildNode()
        {
        }
    }
}