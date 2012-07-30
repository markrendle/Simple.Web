#I @"packages/FAKE.1.64.6/tools"
#r "FakeLib.dll"
open Fake

let buildDir = @".\build\"
let testDir = @".\test"

let fxReferences = !! @"*\*.csproj"
let testReferences = !! @"Tests\**\*.csproj"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "Build" (fun _ ->
    MSBuildRelease buildDir "Build" fxReferences
        |> Log "Build-Output: "
)

Target "BuildTest" (fun _ ->
    MSBuildRelease testDir "Build" testReferences
        |> Log "Test-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + @"\*.Tests.dll")
        |> xUnit (fun p ->
            { p with
                ShadowCopy = true;
                HtmlOutput = true;
                XmlOutput = true;
                OutputDir = testDir })
)

Target "Default" DoNothing

RunParameterTargetOrDefault "target" "Default"