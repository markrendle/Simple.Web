#I @"tools/FAKE"
#r "FakeLib.dll"
open Fake

let buildDir = @"./build/"
let testDir = @"./test/"

let fxReferences = !! @"*/*.csproj"
let testReferences = !! @"Tests/**/*.csproj"
let buildTargets = environVarOrDefault "BUILDTARGETS" ""

let isMono = System.Environment.OSVersion.Platform = System.PlatformID.Unix

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "Build" (fun _ ->
    MSBuild buildDir "Build" ["Configuration","Release"; "VSToolsPath",buildTargets] fxReferences
        |> Log "Build-Output: "
)

Target "BuildTest" (fun _ ->
    MSBuildRelease testDir "Build" testReferences
        |> Log "Test-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + @"/*.Tests.dll")
        |> xUnit (fun p ->
            { p with
                ShadowCopy = true;
                HtmlOutput = not isMono;
                XmlOutput = not isMono;
                OutputDir = testDir })
)

"Clean"
  ==> "Build"

"BuildTest"
  ==> "Test"

Target "Default" DoNothing

RunParameterTargetOrDefault "target" "Default"
