# Simple.Web 

Simple.Web is a REST-focused, object-oriented Web Framework for .NET 4. More information on it's background can be found [here](http://blog.markrendle.net/2012/06/01/simple-web/).
	
### BUILD

You can build Simple.Web using the command-line options outlined below, or simply through MS Visual Studio or MonoDevelop. It is recommended that anyone planning to contribute to the project familiarise themselves with the command-line as it will be the successful completion of this build process that will stand your pull-request in good stead.

#### Command-Line
##### Ruby
The command-line process uses [RAKE](http://rake.rubyforge.org/), a [Ruby](http://www.ruby-lang.org/en/) build system. It will therefore be necessary to have Ruby >= 1.9.2 installed. Dependent on your operating system there are several options to accomplish this but we first recommend you can check current installation by typing `ruby -v` from console prompt.

For example:

    > ruby -v
    ruby 1.9.2p290 (2011-07-09 revision 32553) [x86_64-darwin11.1.0]

If you do not yet have Ruby installed on your system check out [RubyInstaller (for Windows)](http://rubyinstaller.org/downloads/) or [RVM (for *nix)](https://rvm.io/). You can also use [RVM](https://rvm.io/) to install and manage multiple versions of Ruby in a *nix environment.

##### Rake
[RubyGems](http://rubygems.org/) are Ruby's own managed packages and it's through this system we need to install some build dependencies. To simplify their installation simply type the following at the console prompt:

Windows:

    > .\InstallGems.bat  

*nix:

    > ./installgem.sh

Using Ruby's `gem` utility it should install the `Bundler`, `RAKE`, and `Albacore` gems. You can check currently installed gems at any time by typing `gem list --local` from a console prompt.

#### Building
From the root of the repository you can see all available build tasks by typing:

    > rake -T

    rake build     # Build
    rake clean     # Remove any temporary products.
    rake clobber   # Remove any generated file.
    rake full      # Build + Tests + Specs
    rake publish   # Build + Tests + Specs + Publish (remote)
    rake publocal  # Build + Tests + Specs + Publish (local)
    rake quick     # Build + Unit tests
    rake test      # Build + Tests (default)

You can specify any of these tasks when building Simple.Web, or simply just type `rake` for it's configured default.

##### Task: Build + ...
The variation available is to faciliate faster repetitive build times depending on your current workflow. For example if you are regularly compiling mid-development you may choose `rake quick` which confines it's boundary to unit tests. If you are looking for greater assurance `rake test` (the default) will run both unit and integration tests. Finally you can choose `rake full` to run all tests and specifications.

##### Task: Clean
This will remove all intermediate files creating during the build process, but leave build artifacts and results intact.

##### Task: Clobber
This will remove all build-time generated files, effectively restoring your repository to that in source control. This should be the same as performing `git clean -xfd`.

##### Task: Publish / Publocal
Allows you to package Simple.Web's nuget packages and publish to either NuGet.org using `rake publish`, or an alternative nuget server using `rake publocal`. 

To enable publishing you will need to set the appropriate environment variables:

    rake publish
      apiurl_local -- NuGet server url (e.g. "http://nuget.org")
      apikey_local -- ServerAPI key that authenticates your publish

    rake publocal
      apiurl_remote -- NuGet server url (e.g. "http://nuget.local")
      apikey_remote -- Server API key that authenticates your publish

### NOTES
##### Mono 2.10.x
Due to a [variance validity bug] [1] in Mono 2.10.x you will require version 3.x (MDK Installer recommended). We continue to petition Xamarin to make another 2.x STABLE release to address this. In the meantime our continuous integration setup will continue to run against 2.x STABLE and 3.x BETA releases of Mono.

##### NuGet.exe on *nix with Mono
If you are running on *nix or OS X Darwin (before Mountain Lion) you may need to tell Mono which certificate authorities to trust when performing NuGet operations over https:

    > sudo mozroots --import --sync 

    Mozilla Roots Importer - version 3.0.2.0
    Download and import trusted root certificates from Mozilla's MXR.
    Copyright 2002, 2003 Motus Technologies. Copyright 2004-2008 Novell. BSD licensed.

    Downloading from 'http://mxr.mozilla.org/seamonkey/source/security/nss/lib/ckfw/builtins/certdata.txt?raw=1'...
    Importing certificates into user store...
    140 new root certificates were added to your trust store.
    Import process completed.

### CONTRIBUTE
Contributions to Simple.Web are gratefully received but we do ask you to follow certain conditions:

* Fork the main [markrendle/Simple.Web](http://github.com/markrendle/Simple.Web.git)
* Use a branch when developing in your own forked repository, DO NOT work against master
* Write a unit test to validate new logic, ideally using TDD
* Ensure all projects build and tests pass, use the command-line option `rake full`
* Make a pull request from `your-fork/your-branch` to `Simple.Web/master`
* Provide a description of the motivation behind the changes

.. but all that said, don't be afraid :-)

### VERSION
Versioning is of assemblies and nuget packages is dictated by that specified through `VERSION.txt`. The main contributors to the project will manage releases and [SemVer-compliant](http://semver.org/) version numbers. We ask you do not include VERSION.txt in any of your pull-requests, just indicate in it's accompanying description any thoughts in this direction.

### SUPPORT
Any problems [raise an issue](https://github.com/markrendle/Simple.Web/issues?state=open) or [shout](https://groups.google.com/forum/?fromgroups#!forum/simpleweb-dev) and will get straight on it.

[1]: https://bugzilla.xamarin.com/show_bug.cgi?id=6360     "Xamarin bugzilla report 6360 - Variance validity in 2.10.x"