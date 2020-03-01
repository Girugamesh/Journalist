#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.Testing.XUnit2
open System.IO


let getPath path =
    Path.GetFullPath(path) + "/"

let binariesDir = "out/bin"
let nugetDir = getPath "out/nuget"
let projectToPack = ["src\Journalist.EventStore\Journalist.EventStore.csproj";
                     "src\Journalist.LanguageExtensions\Journalist.LanguageExtensions.csproj";
                     "src\Journalist.WindowsAzure.Storage\Journalist.WindowsAzure.Storage.csproj";
                     "src\Journalist.WindowsAzure.Storage.Abstractions\Journalist.WindowsAzure.Storage.Abstractions.csproj"]

let applicationProjects = !! "src/**/*.fsproj" ++ "src/**/*.csproj"
let testProjects = !! "test/**/*.fsproj" ++ "test/**/*.csproj"

let release = ReadFile "RELEASE_NOTES.md" |> ReleaseNotesHelper.parseReleaseNotes

MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }

Target "Clean" (fun _ ->
    CleanDirs <| !! "src/**/bin/Release"
    CleanDirs <| !! "test/**/bin/Release"
    CleanDirs [ binariesDir; nugetDir ]
)

Target "GenerateAssemblyInfo" (fun _ ->

    CreateCSharpAssemblyInfo "src/SolutionInfo.cs" [
            Attribute.Product "Journalist";
            Attribute.Version release.AssemblyVersion;
            Attribute.InformationalVersion release.AssemblyVersion;
            Attribute.FileVersion release.AssemblyVersion;
            Attribute.Company "Anton Mednonogov" ]
)

Target "BuildApp" (fun _ ->
    MSBuildRelease null "Build" [ "./Journalist.sln" ] |> ignore
)

Target "CopyBuildResults" (fun _ ->
    Copy binariesDir !! ("./src/**/bin/Release/*.dll")
    Copy binariesDir !! ("./src/**/bin/Release/*.xml")
    Copy binariesDir !! ("./src/**/bin/Release/*.pdb")
)

Target "RunUnitTests" (fun _ ->
    !! ("./test/**/bin/Release/*.UnitTests.dll")
    |> xUnit2 (fun p ->
        { p with ToolPath = "packages/xunit.runner.console/tools/xunit.console.exe" })
)

Target "RunIntegrationTests" (fun _ ->
    !! ("./test/**/bin/Release/*.IntegrationTests.dll")
    |> xUnit2 (fun p ->
        { p with ToolPath = "packages/xunit.runner.console/tools/xunit.console.exe" })
)

Target "CreatePackages" (fun _ ->
    for project in projectToPack do
        DotNetCli.Pack (fun p ->
            { p with
                OutputPath = nugetDir
                Project = project})
)

Target "PublishPackages" (fun _ ->
    Paket.Push (fun p ->
        { p with
            WorkingDir = nugetDir})
)

Target "Release" (fun _ ->
    StageAll ""
    Commit "" (sprintf "Bump version to %s" release.NugetVersion)
    Branches.push ""

    Branches.tag "" release.NugetVersion
    Branches.pushTag "" "origin" release.NugetVersion
)

Target "Test" DoNothing
"Clean"
    ==> "BuildApp"
    ==> "RunUnitTests"
    ==> "RunIntegrationTests"
    ==> "Test"

Target "UnitTest" DoNothing
"Clean"
    ==> "BuildApp"
    ==> "RunUnitTests"
    ==> "UnitTest"

Target "Default" DoNothing
"Clean"
    ==> "GenerateAssemblyInfo"
    ==> "BuildApp"
    ==> "RunUnitTests"
    //==> "RunIntegrationTests"
    ==> "CopyBuildResults"
    ==> "CreatePackages"
    ==> "Default"
    //==> "PublishPackages"
    //==> "Release"

RunTargetOrDefault "Default"
