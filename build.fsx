#I @"packages/FAKE.1.64.7/tools"
#r "FakeLib.dll"
open Fake

let buildDir = @"./build/"
let testDir = @"./test"

let fxReferences = !! @"*/*.csproj"
let testReferences = !! @"Tests/**/*.csproj"
let buildTargets = environVarOrDefault "BUILDTARGETS" ""

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "Build" (fun _ ->
    MSBuild buildDir "Build" ["Configuration","Debug"; "VSToolsPath",buildTargets] fxReferences
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
                HtmlOutput = true;
                XmlOutput = true;
                OutputDir = testDir })
)

"Clean"
  ==> "Build"

"Build"
  ==> "BuildTest"

Target "Default" DoNothing

RunParameterTargetOrDefault "target" "Default"