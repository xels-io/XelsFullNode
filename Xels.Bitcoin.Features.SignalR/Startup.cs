﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xels.Bitcoin.Utilities.JsonConverters;

namespace Xels.Bitcoin.Features.SignalR
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "CorsPolicy",
                        builder =>
                        {
                            var allowedDomains = new[] { "http://localhost", "http://localhost:4200" };

                            builder
                                .WithOrigins(allowedDomains)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                });
            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                var settings = new JsonSerializerSettings();
                Serializer.RegisterFrontConverters(settings);
                options.PayloadSerializerSettings = settings;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseSignalR(route => { route.MapHub<EventsHub>("/events-hub"); });
        }
    }
}