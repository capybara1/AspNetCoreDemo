﻿using AspNetCoreDemo.Utils;
using AspNetCoreDemo.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Localization
{
    [Trait("Category", "ASP.NET Core MVC / Localization")]
    public class LocalizationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LocalizationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use string localization with resx file")]
        public async Task UseStringLocalizationWithResxFile()
        {
            // This example does not use the AddMvcLocalization extension method
            // to demonstrate that the components in Microsoft.Extensions.Localization
            // are sufficient. Those are added by the AddLocalization extension method.

            // Important: create a resource file in Visual Studio with a culture in the file name
            // Access modifier setting of the resx file must be set to: no code generation

            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddLocalization();
                    services.AddMvcCore()
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseRequestLocalization(opt =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("de"),
                            new CultureInfo("en"),
                        };

                        opt.DefaultRequestCulture = new RequestCulture("en");
                        opt.SupportedCultures = supportedCultures;
                        opt.SupportedUICultures = supportedCultures;
                    });
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute("test", "/", new
                        {
                            controller = nameof(Controllers.DemoController).RemoveControllerSuffix(),
                            action = nameof(Controllers.DemoController.ActionForCommonLocalization),
                        });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            await client.GetAsync("/?culture=de");
        }

        [Fact(DisplayName = "Use localization of data annotation")]
        public async Task UseLocalizationOfDataAnnotation()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore()
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                        .AddDataAnnotations()
                        .AddDataAnnotationsLocalization(options =>
                        {
                            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resources.ModelValidation));
                        });
                })
                .Configure(app =>
                {
                    app.UseRequestLocalization(opt =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("de"),
                            new CultureInfo("en"),
                        };

                        opt.DefaultRequestCulture = new RequestCulture("en");
                        opt.SupportedCultures = supportedCultures;
                        opt.SupportedUICultures = supportedCultures;
                    });
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute("test", "/", new
                            {
                                controller = nameof(Controllers.DemoController).RemoveControllerSuffix(),
                                action = nameof(Controllers.DemoController.ActionForDataAnnotationLocalization),
                            });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/?culture=de&value=0");
        }
    }
}
