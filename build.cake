var target = Argument("target","Clean");
var config = Argument("config", "Debug");

var clean = Task("Clean").Does(() => {
CleanDirectories("./artifacts/");
CleanDirectories("./testartifacts/");
DotNetClean("./SharpLimiter.sln");
});

Task("Restore").IsDependeeOf("Clean").Does(() => {
DotNetRestore("./SharpLimiter.sln");
});

Task("Build")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.Does(() => {
DotNetBuild("./SharpLimiter/SharpLimiter.csproj"  , new DotNetBuildSettings {NoRestore = true  , OutputDirectory = "./artifacts/" , Configuration = config });
});
Task("Test")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() => {
    DotNetTest("SharpLimiter.sln", new DotNetTestSettings{NoRestore = true, NoLogo = true, OutputDirectory="./testartifacts/"});
}).Finally(() => {
CleanDirectories("./artifacts/");
CleanDirectories("./testartifacts/");
DotNetClean("./SharpLimiter.sln");
});


RunTarget(target);