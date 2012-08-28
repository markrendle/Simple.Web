Simple.Web 
==========

http://blog.markrendle.net/2012/06/01/simple-web/

TODO & Issues
=============

OWIN
----
* Some exceptions aren't being bubbled up to the selfhost level, causing the host to crash.
* Authentication & cookies

MONO
----
Some effort has been made to make Simple.Web play nicely on Mono, but there are some caveats;

* Due to a [variance validity bug] [1] in Mono 2.10 you will require version 2.11+ (MDK Installer recommended).
* Due to an [xbuild bug] [2] in Mono 2.11.1 and Mono 2.11.3 we have (with permission) included a local version for use with FAKE. This does unfortunately mean an execute xbuild script in the repository root, but it's a [temporary evil](https://github.com/mono/mono/pull/422) until 2.11.5 is released.
* Simple.Web uses [FAKE F# Make] [3] as it's build system which requires FSI (FSharp Interactive); as later versions of Mono appear to have renamed FSI and FSC it is recommended you have a bash script redirect in `/usr/local/bin` (or other search path); an example FSI script [is shown here](https://gist.github.com/3260447) and equivilent for FSC [here](https://gist.github.com/3260445).
* FAKE is setup to look for a BUILDTARGETS environment variable to find WebApplication target files; [default behaviour](https://github.com/markrendle/Simple.Web/blob/master/fake.sh#L8) in `fake.sh` points to an OS X (Darwin) installation path; we would welcome a pull-request to cater for generic *nix installs.

Once you've absorbed the above, simply type the following at the repository root:

	`./fake.sh <command>`

Commands available are:

	`Clean`
	`Build`
	`BuildTest`
	`Test`

Any problems [raise an issue](https://github.com/markrendle/Simple.Web/issues?state=open) or [shout](https://groups.google.com/forum/?fromgroups#!forum/simpleweb-dev) and will get straight on it.

[1]: https://bugzilla.xamarin.com/show_bug.cgi?id=6360     "Xamarin bugzilla report 6360 - Variance validity in 2.10.x"
[2]: https://bugzilla.xamarin.com/show_bug.cgi?id=6389     "Xamarin bugzilla report 6389 - xbuild absolute path issue in 2.11.2.0"
[3]: https://github.com/fsharp/FAKE     "F# Make Build System"