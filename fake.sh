#!/bin/bash
TARGET=$1 

if [ -z "$TARGET" ]
	then
	TARGET="Default"
fi

mono packages/FAKE.1.64.6/tools/FAKE.exe build.fsx target=$TARGET