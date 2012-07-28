#!/bin/bash
TARGET=$1 

if [ -z "$TARGET" ]
	then
	CTARGET="Default"
else
	CTARGET=`echo ${TARGET:0:1} | tr "[:lower:]" "[:upper:]"``echo ${TARGET:1} | tr "[:upper:]" "[:lower:]"`
fi

echo "Executing command: $CTARGET"
mono packages/FAKE.1.64.6/tools/FAKE.exe build.fsx target=$CTARGET