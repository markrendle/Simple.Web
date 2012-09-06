#I @"tools/FAKE"
#r "FakeLib.dll"
open Fake

let mutable buildDir = @"./build/"
let testDir = @"./test/"
let fxReferences = !! @"*/*.csproj"
let testReferences = !! @"Tests/**/*.csproj"
let buildTargets = environVarOrDefault "BUILDTARGETS" ""

let isMono = System.Environment.OSVersion.Platform = System.PlatformID.Unix

Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    MSBuild buildDir "Build" ["Configuration","Release"; "VSToolsPath",buildTargets] fxReferences
        |> Log "Build-Output: "
)

Target "OutputToTest" (fun _ ->
    buildDir <- testDir
)

Target "BuildTest" (fun _ ->
    MSBuild buildDir "Build" ["Configuration","Release"; "VSToolsPath",buildTargets] testReferences
        |> Log "Test-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + @"/*.Tests.dll")
        |> xUnit (fun p ->
            { p with
                ShadowCopy = true;
                HtmlOutput = not isMono;
                XmlOutput = not isMono;
                OutputDir = buildDir })
)

"OutputToTest"
  ==> "BuildTest"

"Clean"
  ==> "Build"

"Clean"
  ==> "BuildTest"

"Build"
  =?> ("BuildTest", isMono)

"BuildTest"
  ==> "Test"

Target "Default" DoNothing

RunParameterTargetOrDefault "target" "Default"