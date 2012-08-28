#!/bin/bash
TARGET=$1 
BUILDTARGETS=$2
MONOVERSION=`mono --version`

if [ -z "$BUILDTARGETS" ]
	then
	BUILDTARGETS="/Library/Frameworks/Mono.framework/Libraries/mono/xbuild/Microsoft/VisualStudio/v9.0"
fi

if [ -z "$TARGET" ]
	then
	CTARGET="Default"
	echo -e "No build action specified, assuming '${CTARGET}'.\nAvailable actions are \"Clean\", \"Build\", \"BuildTest\", or \"Test\".\n"
else
	CTARGET=`echo ${TARGET:0:1} | tr "[:lower:]" "[:upper:]"``echo ${TARGET:1} | tr "[:upper:]" "[:lower:]"`
fi

if [ ! -d "$BUILDTARGETS" ]
	then
	echo "BuildTargets directory '${BUILDTARGETS}' does not exist."
	exit $?
else
	export BUILDTARGETS="$BUILDTARGETS"
fi

if [[ ! "$MONOVERSION" =~ "version "[2-9]"."[1-9][1-9]"." ]]
	then
	echo -e "You must have Mono version 2.11+ installed to compile Simple.Web.\nVisit http://www.go-mono.com/mono-downloads/download.html for the latest version."
	exit 1
fi

echo "Executing command: $CTARGET"

mono packages/FAKE.1.64.7/tools/Fake.exe build.fsx target=$CTARGET verbose #details