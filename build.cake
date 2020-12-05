// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0

// Install addins.
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Coveralls&version=0.10.2"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Twitter&version=0.10.1"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Gitter&version=0.11.1"

// Install tools.
#tool "nuget:https://api.nuget.org/v3/index.json?package=coveralls.io&version=1.4.2"
#tool "nuget:https://api.nuget.org/v3/index.json?package=OpenCover&version=4.7.922"
#tool "nuget:https://api.nuget.org/v3/index.json?package=ReportGenerator&version=4.7.1"
#tool "nuget:https://api.nuget.org/v3/index.json?package=nuget.commandline&version=5.7.0"

// Install .NET Core Global tools.
#tool "dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.1.2"
#tool "dotnet:https://api.nuget.org/v3/index.json?package=SignClient&version=1.2.109"
#tool "dotnet:https://api.nuget.org/v3/index.json?package=GitReleaseManager.Tool&version=0.11.0"

// Load other scripts.
#load "./build/parameters.cake"

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup<BuildParameters>(context =>
{
    var parameters = new BuildParameters(context);

    // Increase verbosity?
    Information("Increasing verbosity to diagnostic.");
    context.Log.Verbosity = Verbosity.Diagnostic;

    Information("Building version {0} of Cake ({1}, {2}) using version {3} of Cake. (IsTagged: {4})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target,
        parameters.Version.CakeVersion,
        parameters.IsTagged);

    foreach(var assemblyInfo in GetFiles("./src/**/AssemblyInfo.cs"))
    {
        CreateAssemblyInfo(
            assemblyInfo.ChangeExtension(".Generated.cs"),
            new AssemblyInfoSettings { Description = parameters.Version.SemVersion });
    }

    if(!parameters.IsRunningOnWindows)
    {
        var frameworkPathOverride = context.Environment.Runtime.IsCoreClr
                                        ?   new []{
                                                new DirectoryPath("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono"),
                                                new DirectoryPath("/usr/lib/mono"),
                                                new DirectoryPath("/usr/local/lib/mono")
                                            }
                                            .Select(directory =>directory.Combine("4.5"))
                                            .FirstOrDefault(directory => context.DirectoryExists(directory))
                                            ?.FullPath + "/"
                                        : new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        // Use FrameworkPathOverride when not running on Windows.
        Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
        parameters.MSBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    Information("Starting Teardown...");

    if(context.Successful)
    {
        if(parameters.ShouldPublish)
        {
            if(parameters.CanPostToTwitter)
            {
                var message = "Version " + parameters.Version.SemVersion + " of Cake has just been released, https://www.nuget.org/packages/Cake.";

                TwitterSendTweet(parameters.Twitter.ConsumerKey, parameters.Twitter.ConsumerSecret, parameters.Twitter.AccessToken, parameters.Twitter.AccessTokenSecret, message);
            }

            if(parameters.CanPostToGitter)
            {
                var message = "@/all Version " + parameters.Version.SemVersion + " of the Cake has just been released, https://www.nuget.org/packages/Cake.";

                var postMessageResult = Gitter.Chat.PostMessage(
                    message: message,
                    messageSettings: new GitterChatMessageSettings { Token = parameters.Gitter.Token, RoomId = parameters.Gitter.RoomId}
                );

                if (postMessageResult.Ok)
                {
                    Information("Message {0} succcessfully sent", postMessageResult.TimeStamp);
                }
                else
                {
                    Error("Failed to send message: {0}", postMessageResult.Error);
                }
            }
        }
    }

    Information("Finished running tasks.");
});


Task("Build")
    .Does<BuildParameters>((context, parameters) =>
{
    // Build the solution.
    var path = MakeAbsolute(new DirectoryPath("./src/Cake.sln"));
    DotNetCoreBuild(path.FullPath, new DotNetCoreBuildSettings()
    {
        Configuration = parameters.Configuration,
        NoRestore = true,
        MSBuildSettings = parameters.MSBuildSettings
    });
});



Task("Watch")
    .Does<BuildParameters>((context, parameters) =>
{
 DotNetCoreWatch("./src/Cake/Cake.csproj", 
    new ProcessArgumentBuilder()
            .Append("build"),
    new DotNetCoreWatchSettings()
    {
        List = true,
    });

 DotNetCoreWatch("./src/Cake/Cake.csproj", 
    new ProcessArgumentBuilder(),
    new DotNetCoreWatchSettings()
    {
        Version = true,
    });
});

RunTarget(Argument("target", "Watch"));
