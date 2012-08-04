#!/bin/bash
TARGET=$1
BUILDTARGETS=$2

if [ -z "$BUILDTARGETS" ]
	then
	BUILDTARGETS="/Library/Frameworks/Mono.framework/Libraries/mono/xbuild/Microsoft/VisualStudio/v9.0"
fi

if [ -z "$TARGET" ]
	then
	CTARGET="Default"
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

echo "Executing command: $CTARGET"

mono packages/FAKE.1.64.6/tools/Fake.exe build.fsx target=$CTARGET verbose #details