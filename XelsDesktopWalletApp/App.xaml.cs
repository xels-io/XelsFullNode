using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xels.Bitcoin;
using Xels.Bitcoin.Base;
using Xels.Bitcoin.Features.Wallet;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using Xels.Bitcoin.Utilities;

namespace XelsDesktopWalletApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            //services.AddSingleton<IFullNode, FullNode>();
            //services.AddSingleton<IChainState, ChainState>();
            //services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IWalletManager, WalletManager>();

        }



        protected override async void OnStartup(StartupEventArgs e)
        {

            await this._host.StartAsync();

            var mainWindow = this._host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);

        }

    }
}
