﻿using Microsoft.Extensions.Logging;
//using Microsoft.Maui.TestUtils.DeviceTests.Runners.HeadlessRunner;
using Microsoft.Maui.TestUtils.DeviceTests.Runners.VisualRunner;

namespace Microsoft.Maui.TestUtils.DeviceTests.Runners
{
	public static class AppHostBuilderExtensions
	{
		public static MauiAppBuilder ConfigureTests(this MauiAppBuilder appHostBuilder, TestOptions options)
		{
			appHostBuilder.Services.AddSingleton(options);

			return appHostBuilder;
		}

		public static MauiAppBuilder UseVisualRunner(this MauiAppBuilder appHostBuilder)
		{
			appHostBuilder.UseMauiApp(svc => 
			{
                TestServices.Init(svc);

                return new MauiVisualRunnerApp(
					svc.GetRequiredService<TestOptions>(),
					svc.GetRequiredService<ILoggerFactory>().CreateLogger("TestRun")
				);
			});

			return appHostBuilder;
		}

//		public static MauiAppBuilder UseHeadlessRunner(this MauiAppBuilder appHostBuilder, HeadlessRunnerOptions options)
//		{
//			appHostBuilder.Services.AddSingleton(options);

//#if __ANDROID__ || __IOS__ || MACCATALYST
//			appHostBuilder.Services.AddTransient(svc => new HeadlessTestRunner(
//					svc.GetRequiredService<HeadlessRunnerOptions>(),
//					svc.GetRequiredService<TestOptions>()));
//#endif

//			return appHostBuilder;
//		}
	}
}