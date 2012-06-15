# This script tries to create or update the IIS Site and HOSTS file entry
param([string]$url = $(throw "Base dns required"), 
      [string]$name = $(throw "Site name required"),
	  [string]$location = $(throw "Filesystem location required")
     )

$base_folder = Split-Path -parent $MyInvocation.MyCommand.Definition # get path to the script
#echo "using $base_folder as base"
if ((Test-Path "$base_folder\Output\SetupErrors.txt") -eq $true) {
	rm "$base_folder\Output\SetupErrors.txt"
}

#Import-Module WebAdministration ## fails on WOW64, use direct assembly instead.
[System.Reflection.Assembly]::LoadFrom( "C:\windows\system32\inetsrv\Microsoft.Web.Administration.dll" ) | out-null 
$iis = new-object Microsoft.Web.Administration.ServerManager

$expected_sites = @{
    $url = @($name, "$base_folder\$location");
}

$required_hosts = $expected_sites.Clone()

try {
    $hosts_file = "C:\Windows\System32\drivers\etc\hosts"
    $usr = [Security.Principal.WindowsIdentity]::GetCurrent().Name
    takeown /F "$hosts_file" | out-null
    $rule=new-object System.Security.AccessControl.FileSystemAccessRule($usr, "Modify", "Allow")
    $acl=get-acl $hosts_file
    $acl.SetAccessRule($rule)
    set-acl $hosts_file $acl | out-null 
} catch {
	echo "Could not get access to system HOSTS file" | out-file "$base_folder\Output\SetupErrors.txt"
    exit 1
}
$hosts_entries = (get-content $hosts_file)
foreach ($hline in $hosts_entries) {
    if ($hline.startsWith("#")) {continue}
    try{$required_hosts.Remove([regex]::split($hline, "\s+")[1])} catch {}
}

try {
    if ($required_hosts.Count -gt 0) {
        Add-Content $hosts_file ("`n")
    }
    foreach ($hname in $required_hosts.keys) {
        Add-Content $hosts_file ("127.0.0.1`t`t"+$hname)
        echo " - " + $hname + " was missing, now added"
    }
} catch {
    exit 1
}
try {
    $path = Resolve-Path "$base_folder\$location"
    $rule=new-object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS","Modify","ContainerInherit,ObjectInherit","None","Allow")
    $acl=get-acl $path
    $acl.SetAccessRule($rule)
    set-acl $path $acl | out-null 
} catch {
	echo "Failed to set IIS root folder permissions" | out-file "$base_folder\Output\SetupErrors.txt"
    exit 1
}

$sites = $iis.sites
$all_hosts = $sites | %{$_.Bindings} | %{$_.Host}

# Now, $required_sites contains binding, name and file path for each new website that needs creating...
$wc = new-object net.WebClient
if (-not ($all_hosts -contains $url)) {
    $bind = $url
    $path = Resolve-Path "$base_folder\$location"
    echo "binding $url"
    try {
		$pool = $iis.applicationpools | ?{$_.name -eq $name}
		if (! $pool) {
			$pool = $iis.ApplicationPools.Add($name)
			$pool.managedRuntimeVersion = "v4.0"
		}
		$webSite = $iis.Sites.Add($name, "http", "*:80:$bind", $path);
		$webSite.Applications[0].ApplicationPoolName = $name;
		$webSite.ServerAutoStart = $TRUE;
		$iis.CommitChanges()
		
        try {
            $probe = $wc.downloadData("http://" + $bind) # Ensures users are created
        } catch {
            # expect an error of no permission, so no real need to handle it
        }
    } catch {
		echo "Failed to create IIS website and/or app pool" | out-file "$base_folder\Output\SetupErrors.txt"
        exit 1
    }
}
