# Xunit.Runners.Maui 

## Ported From
* https://github.com/xunit/devices.xunit
* https://github.com/dotnet/maui/tree/main/src/TestUtils

## Setup

1. Create a new MAUI application.
2. Delete the standard App.xaml & App.xaml.cs
3. Install Xunit.Runners.Maui nuget
4. Create tests in this project or reference another project containing your tests.
5. Create a MauiProgram like shown below - NOTE: It is not recommended you put any dependencies here

```csharp
using Xunit.Runners.Maui;

namespace Sample
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp() => MauiApp
			.CreateBuilder()
			.ConfigureTests(new TestOptions
			{
				Assemblies =
				{
					typeof(MauiProgram).Assembly
				}
			})
			.UseVisualRunner()
			.Build();
	}
}
```


Here are the key steps that got this project to work with the mobile app. 

* Create an AndroidTestRunner MAUI project within the same solution as your MAUI app you would like to test.
* Create a Test configuration for the app to test 
    * Open Build -> Configuration Manager
    * Under your app, add a new Configuration called "Test" and base it on Debug so that you can use the debugger on your unit tests
    
![image](https://github.com/user-attachments/assets/5b389b9c-12fc-4bfc-a286-d8bff80beb2a)

* You will create a new compile flag called ANDROID_TEST_RUNNER for the Test configuration only. On your app to test, right click the .csproj file in the solution explorer and choose Properties. In the properties, choose Build -> General and find the Test configuration where you can add the DEBUG and ANDROID_TEST_RUNNER flags:
    
![image](https://github.com/user-attachments/assets/70c003df-1e42-4906-a5aa-555a0ff17e22)

*  You will want your test runner to be its own application, so you must leave out the `[Application]` attribute in your app's MainApplication.cs file by using the new flag:
```
#if (ANDROID_TEST_RUNNER == false)
[Application]
#endif
```

* You also only want one main activity, so modify MainActivity in the mobile app to hide the activity annotation:

```
#if (ANDROID_TEST_RUNNER == false)
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Platform.Intent.ActionAppAction },
              Categories = new[] { global::Android.Content.Intent.CategoryDefault })]
#endif

public class MainActivity : MauiAppCompatActivity
```

* For your AndroidTestRunner, delete the following files:
    * App.xaml & App.xaml.cs
    * AppShell.xaml & AppShell.xaml.cs
    * all of the platform folders except Android
* Add the Shiny.Xunit.Runners.Maui NuGet package
* Replace the entire contents of MauiProgram.cs in your AndroidTestRunner project with the following:
```
using Xunit.Runners.Maui;

namespace AndroidTestRunner;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
       .CreateBuilder()
       .ConfigureTests(new TestOptions
       {
           Assemblies =
           {
                typeof(MauiProgram).Assembly
           }
       })
       .UseVisualRunner()
       .Build();
}
```

* You will also need to add all of the packages used by your app to test to the AndroidTestRunner project.
* Add a UnitTests folder to AndroidTestRunner project
* You can start with this SampleTests.cs from the Shinyorg repo:
```
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class SampleTests
{
    readonly ITestOutputHelper _output;

    public SampleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void SuccessfulTest()
    {
        Assert.True(true);
    }

    [Fact(Skip = "This test is skipped.")]
    public void SkippedTest()
    {
    }

    [Fact]
    public void FailingTest()
    {
        throw new Exception("This is meant to fail.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ParameterizedTest(int number)
    {
        Assert.NotEqual(0, number);
    }

    [Fact]
    public void OutputTest()
    {
        _output.WriteLine("This is test output.");
    }

    [Fact]
    public void FailingOutputTest()
    {
        _output.WriteLine("This is test output.");
        throw new Exception("This is meant to fail.");
    }
}
```

* Return to the Configuration Manager by going to Build -> Configuration Manager. Set the app to test to use the Test configuration and the AndroidTestRunner to use the Debug configuration.

![image](https://github.com/user-attachments/assets/26b1abf4-fb6f-4411-ba3e-95307862afdd)

* Now build the app to test by right clicking on it in the solution explorer and choosing Build.
* There should now be a Test folder under the app to test's bin folder.
* You should now be able to add a shared project link to your app to test. In the solution explorer, right click on Dependencies under AndroidTestRunner and choose Add Shared Project Reference. Click the Browse button and find the AppToTest.dll file under the bin\Test\net8.0-android folder of your app to test. Click Add button.
* At this point you should be able to run the AndroidTestRunner project on an Android device or simulator and execute the tests.
