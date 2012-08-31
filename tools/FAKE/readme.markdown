# What is "FAKE - F# Make"?

## Introduction

Modern build automation systems are not limited to simply recompile programs if source code has changed. 
They are supposed to get the latest sources from a source code management system, build test databases, 
run automatic tests, check guidelines, create documentation files, install setup projects and much more. 
Some companies are even deploying virtual machines, which are created during a nightly build process. 
In order to simplify the writing of such build scripts and to provide reusability of common tasks 
most build automation systems are using a domain-specific language (DSL). 
These tools can be divided into tools using external DSLs with a custom syntax like **make**, 
tools using external DSLs with an XML based syntax like **MSBuild** or **Apache Ant** and 
tools using internal DSLs which are integrated in a host language like **Rake**, which uses Ruby.

## FAKE - An integrated DSL

"FAKE - F# Make" is a build automation system, which is intended to combine the advantages 
of the above mentioned tools but to provide a better tooling support. 
Due to its integration in F#, all benefits of the .NET Framework and functional programming can be used, 
including the extensive class library, powerful debuggers and integrated development environments 
like Visual Studio 2008 or SharpDevelop, which provide syntax highlighting and code completion. 

The new language was designed to be succinct, typed, declarative, extensible and easy to use. 
For instance custom build tasks can be added simply by referencing .NET assemblies and using the corresponding classes.

## Lastest builds and changelog

You can download the latest builds from http://teamcity.codebetter.com. You don't need to register, a guest login is ok.

* [Latest stable build](http://teamcity.codebetter.com/viewLog.html?buildId=lastSuccessful&buildTypeId=bt335&tab=artifacts&guest=1)
* [Latest development build](http://teamcity.codebetter.com/viewLog.html?buildId=lastSuccessful&buildTypeId=bt166&tab=artifacts&guest=1)
* [Changelog](http://github.com/forki/FAKE/blob/develop/changelog.markdown)

## Nuget package

We have a Nuget package at http://nuget.org/packages/FAKE. You can install it with:

	install-package FAKE

## How to contribute code

* Login in github (you need an account)
* Fork the main repository from [Github](https://github.com/forki/FAKE)
* Push your changes to your fork
* Send me a pull request

## Mailing list

The "FAKE - F# Make" mailing list can be found at [http://groups.google.com/group/fsharpMake](http://groups.google.com/group/fsharpMake).

## Articles

* [Getting started with "FAKE - F# Make"](http://www.navision-blog.de/2009/04/01/getting-started-with-fake-a-f-sharp-make-tool): This tutorial shows you how to build the CalculatorSample project.
* [Adding FxCop to a "FAKE" build script](http://www.navision-blog.de/2009/04/02/adding-fxcop-to-a-fake-build-script)
* [Debugging "FAKE - F# Make" build scripts](http://www.navision-blog.de/2009/04/05/debugging-fake-f-make-build-scripts)
* [Modifying AssemblyInfo and Version via "FAKE - F# Make"](http://www.navision-blog.de/2009/04/04/modifying-assemblyinfo-and-version-via-fake-f-make)
* [Writing custom tasks for "FAKE - F# Make"](http://www.navision-blog.de/2009/04/14/writing-custom-tasks-for-fake-f-make)
* [Integrating a "FAKE - F# Make" build script into TeamCity](http://www.navision-blog.de/2009/04/15/integrate-a-fake-f-make-build-script-into-teamcity)
* [Integrating a "FAKE - F# Make" build script into CruiseControl.NET](http://www.navision-blog.de/2009/10/14/integrating-a-fake-f-make-build-script-into-cruisecontrol-net)
* [Building FAKE scripts with Jenkins](http://www.navision-blog.de/2012/01/16/building-fake-scripts-with-jenkins/)
* [Running specific targets in "FAKE – F# Make"](http://www.navision-blog.de/2010/11/03/running-specific-targets-in-fake-f-make/)

## Main Features

* Simple build infrastructure
* Easy systax
* Full power of .NET Framework
* Simple [TeamCity](http://www.jetbrains.com/teamcity) integration (see [Integrating a "FAKE - F# Make" build script into TeamCity](http://www.navision-blog.de/2009/04/15/integrate-a-fake-f-make-build-script-into-teamcity))
* Simple CruiseControl.NET integration (see [Integrating a "FAKE - F# Make" build script into CruiseControl.NET](http://www.navision-blog.de/2009/10/14/integrating-a-fake-f-make-build-script-into-cruisecontrol-net))
* FinalTarget feature (to release resources even if build fails)
* Extensible platform - [Write your own tasks](http://www.navision-blog.de/2009/04/14/writing-custom-tasks-for-fake-f-make)
* Easy debugging
* Intellisense support (when using Visual Studio)

## Predefined tasks

* Clean task
* [NUnit](http://www.nunit.org) support
* [xUnit.net](http://www.codeplex.com/xunit) support
* [MSpec](https://github.com/machine/machine.specifications) support
* NCover support
* FxCop support
* ExecProcess task (To run tools via the command line)
* MSBuild task (to compile *.csproj and *.fsproj projects or run MSBuild scripts)
* XMLRead task
* VSS task (Get sources from Visual Source Safe)
* XCopy task
* Zip task
* [git](http://git-scm.com/) tasks
* AssemblyInfo task
* MSI task (to run msi-setups with msiexec)
* RegAsm task (to create TLBs from a .dll)
* ...

# Using FAKE

## Targets

Targets are the main unit of work in a "FAKE - F# Make" script. 
Targets have a name and an action (given as a code block).

	// The clean target cleans the build and deploy folders
	Target "Clean" (fun _ -> 
		CleanDirs ["./build/"; "./deploy/"]
	)

### Build target order

You can specify the build order using the ==> operator:
	
	// "FAKE - F# Make" will run these targets in the order Clean, BuildApp, Default
	"Clean" 
	  ==> "BuildApp" 
	  ==> "Default"

If one target should only be run on a specific condition you can use the =?> operator:
	
	"Clean" 
	  ==> "BuildApp"
	  =?> ("Test",hasBuildParam "test")  // runs the Test target only if FAKE was called with parameter test
	  ==> "Default"

It's also possible to specify the dependencies for targets:

    // Target Default is dependent from target Clean and BuildApp
    // "FAKE - F# Make" will ensure to run these targets before Default
    "Default"  <== ["Clean"; "BuildApp"]

### Running targets

You can execute targets with the "run"-command:

	// Executes Default target
	Run "Default"

### Final targets

Final target can be used for TearDown functionality. 
These targets will be executed even if the build fails but have to be activated via ActivateFinalTarget().

	// FinalTarget will be excuted even if build fails
	FinalTarget "CloseSomePrograms" (fun _ ->
		// close stuff and release resources
	)

	// Activate FinalTarget somewhere during build
	ActivateFinalTarget "CloseSomePrograms"

## FileSets

"FAKE - F# Make" uses similar include and exclude patterns as NAnt and MSBuild. 

### File includes

	// Includes all *.csproj files under /src/app by using the !+ operator
	!+ "src/app/**/*.csproj"

	// Includes all *.csproj files under /src/app and /test with the ++ operator
	!+ "src/app/**/*.csproj"
	  ++ "test/**/*.csproj"

### File excludes

	// Includes all files under /src/app but excludes *.zip files
	!+ "src/app/**/*.*"
	  -- "*.zip"

### Scan vs. ScanImmediately

"FAKE - F# Make" provides two scan methods: Scan() and ScanImmediately().

Scan is a lazy method and evaluates the FileSet as late as possible ("on-demand").
If the FileSet is used twice, it will be reevaluated.

The following code defines a lazy FileSet:

	// Includes all *.csproj files under /src/app and scans them lazy
	let apps = 
	  !+ "src/app/**/*.csproj"
		|> Scan

The same FileSet by using the !! operator:

    // Includes all *.csproj files under /src/app and scans them lazy
    let apps = !! "src/app/**/*.csproj"

ScanImmediately() scans the FileSet immediatly at time of its definition
and memoizes it. 

	// Includes all files under /src/app but excludes *.zip files
	//	  eager scan ==> All files memoized at the time of this definition
	let files = 
	  !+ "src/app/**/*.csproj"
		-- "*.zip"
		|> ScanImmediately

## UnitTests

### NUnit

	// define test dlls
	let testDlls = !! (testDir + @"/Test.*.dll")
	 
	Target "NUnitTest" (fun _ ->
		testDlls
			|> NUnit (fun p -> 
				{p with 
					ToolPath = nunitPath; 
					DisableShadowCopy = true; 
					OutputFile = testDir + "TestResults.xml"})
    )
							 
### MSpec
	// define test dlls
	let testDlls = !! (testDir + @"/Test.*.dll")
	 
	Target "MSpecTest" (fun _ ->
		testDlls
			|> MSpec (fun p -> 
				{p with 
					ExcludeTags = ["LongRunning"]
					HtmlOutputDir = testOutputDir						  
					ToolPath = ".\toools\MSpec\mspec.exe"})
    )

### xUnit.net

	// define test dlls
	let testDlls = !! (testDir + @"/Test.*.dll")

	Target "xUnitTest" (fun _ ->
        testDlls
            |> xUnit (fun p -> 
                {p with 
                    ShadowCopy = false;
                    HtmlOutput = true;
                    XmlOutput = true;
                    OutputDir = testDir })
    )

## Sample script

This sample script
  
* Assumes "FAKE - F# Make" is located at ./tools/FAKE
* Assumes NUnit is located at ./tools/NUnit  
* Cleans the build and deploy paths
* Builds all C# projects below src/app/ and puts the output to .\build
* Builds all NUnit test projects below src/test/ and puts the output to .\build
* Uses NUnit to test the generated Test.*.dll's
* Zips all generated files to deploy\MyProject-0.1.zip
  
You can read [Getting started with FAKE](http://www.navision-blog.de/2009/04/01/getting-started-with-fake-a-f-sharp-make-tool) to build such a script.

    // include Fake libs
    #I "tools\FAKE"
    #r "FakeLib.dll"
    
    open Fake
    
    // Directories
    let buildDir  = @".\build\"
    let testDir   = @".\test\"
    let deployDir = @".\deploy\"
    
    // tools
    let nunitPath = @".\Tools\NUnit"
    let fxCopRoot = @".\Tools\FxCop\FxCopCmd.exe"
    
    // Filesets
    let appReferences  = 
        !+ @"src\app\**\*.csproj" 
          ++ @"src\app\**\*.fsproj" 
            |> Scan
    
    let testReferences = !! @"src\test\**\*.csproj"
        
    // version info
    let version = "0.2"  // or retrieve from CI server
    
    // Targets
    Target "Clean" (fun _ -> 
        CleanDirs [buildDir; testDir; deployDir]
    )
    
    Target "BuildApp" (fun _ ->
        AssemblyInfo 
            (fun p -> 
            {p with
                CodeLanguage = CSharp;
                AssemblyVersion = version;
                AssemblyTitle = "Calculator Command line tool";
                AssemblyDescription = "Sample project for FAKE - F# MAKE";
                Guid = "A539B42C-CB9F-4a23-8E57-AF4E7CEE5BAA";
                OutputFileName = @".\src\app\Calculator\Properties\AssemblyInfo.cs"})
                  
        AssemblyInfo 
            (fun p -> 
            {p with
                CodeLanguage = CSharp;
                AssemblyVersion = version;
                AssemblyTitle = "Calculator library";
                AssemblyDescription = "Sample project for FAKE - F# MAKE";
                Guid = "EE5621DB-B86B-44eb-987F-9C94BCC98441";
                OutputFileName = @".\src\app\CalculatorLib\Properties\AssemblyInfo.cs"})          
          
        // compile all projects below src\app\
        MSBuildRelease buildDir "Build" appReferences
            |> Log "AppBuild-Output: "
    )
    
    Target "BuildTest" (fun _ ->
        MSBuildDebug testDir "Build" testReferences
            |> Log "TestBuild-Output: "
    )
    
    Target "NUnitTest" (fun _ ->  
        !! (testDir + @"\NUnit.Test.*.dll")
            |> NUnit (fun p -> 
                {p with 
                    ToolPath = nunitPath; 
                    DisableShadowCopy = true; 
                    OutputFile = testDir + @"TestResults.xml"})
    )
    
    Target "xUnitTest" (fun _ ->  
        !! (testDir + @"\xUnit.Test.*.dll")
            |> xUnit (fun p -> 
                {p with 
                    ShadowCopy = false;
                    HtmlOutput = true;
                    XmlOutput = true;
                    OutputDir = testDir })
    )
    
    Target "FxCop" (fun _ ->
        !+ (buildDir + @"\**\*.dll") 
            ++ (buildDir + @"\**\*.exe") 
            |> Scan  
            |> FxCop (fun p -> 
                {p with                     
                    ReportFileName = testDir + "FXCopResults.xml";
                    ToolPath = fxCopRoot})
    )
    
    Target "Deploy" (fun _ ->
        !+ (buildDir + "\**\*.*") 
            -- "*.zip" 
            |> Scan
            |> Zip buildDir (deployDir + "Calculator." + version + ".zip")
    )
    
    // Build order
	"Clean"
      ==> "BuildApp" <=> "BuildTest"
      ==> "FxCop"
      ==> "NUnitTest"
      =?> ("xUnitTest",hasBuildParam "xUnitTest")  // runs the target only if FAKE was called with parameter xUnitTest
      ==> "Deploy"
     
    // start build
    Run "Deploy"

## Deployment using FAKE

    * Assumes Fake.Deploy.exe is available in the current directory or path.

### Introduction

The FAKE deployment tool allows users to deploy applications to remote computers and to run scripts on these remote agents. A typical scenario maybe as follows: 


* Build an application -> run tests -> create artifacts and save on build server (Classical FAKE build workflow)
* Extract artifacts from build server and create a NuGet deployment package
* Push the NuGet package to the desired computer this will run the package's FAKE script on the remote machine

### Installing Fake deployment services

In order to deploy application to a remote computer a deployment agent needs to be running on that server.

To run an agent in a console, simply run:
    
    Fake.Deploy

To install a windows service on that agent:
 
   * Open a command prompt with Administrator Priviledges
   * Run Fake.Deploy /install

By default the service starts a listener on port 8080. This can however be configured by editing the Fake.Deploy.exe.config file
and changing
    
    <add key="ServerName" value="localhost" />
    <add key="Port" value="8080" />

to the desired value. If you use the asterisk as port no. then Fake.Deploy will assign the first open port behind of 8080.

To ensure the service is running you can navigate to http://{computer}:{port}/fake/ and you should be presented with a page giving the 
status if the service

### Uninstalling Fake deployment services

To uninstall an agent

   * Open a command prompt with Administrator Priviledges
   * Run Fake.Deploy /uninstall     

### Running a FAKE Deployment Package

### Getting help

If you want to learn about Fake.Deploy's command line switches then run:

    Fake.Deploy /help

### Creating a Deployment package

Since Fake.Deploy uses Nuget packages for deployment you only need to create one of those and include a .fsx file in the root folder of the package.

Instructions for creating nuget packages can be found [at the NuGet document page](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package)  

### Running deployment

Fake deployment packages can be run manually on the current machine or they can be pushed to an agent on a remote machine.

To run a package on the local machine located at C:\Appdev\MyDeployment.nupkg you would run the following command:

    Fake.Deploy /deploy C:\Appdev\MyDeployment.nupkg
    
To run the same package on a remote computer (e.g. integration-1) you can run:

    Fake.Deploy /deployRemote http://integration-1:8080 C:\Appdev\MyDeployment.nupkg 

It's also possible to just make a HTTP-POST with the package to http://integration-1:8080/fake

This will push the directory to the given url. It is worth noting that the port may well be different, as this depends on the configuration of the 
listening agent (see. Installing Fake deployment service)

### Getting information about the deployments

    The following assumes you have Fake.Deploy running.

It's easy to get information about the deployments. Just make a HTTP request to server with:
    
    fake/deployments/                     -> gives all releases
    fake/deployments?status=active        -> gives all active releases
    fake/deployments/{app}                -> gives all releases of app
    fake/deployments/{app}?status=active  -> gives the active release of the app

### Rollback of releases

If you want to perform a rollback of a release so do a HTTP-PUT to:

    fake/deployments/{app}?version={version} -> rolls the app back to the given version
    fake/deployments/{app}?version=HEAD~2    -> relative rollback of the app (two versions earlier)