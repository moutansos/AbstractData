@echo off
echo AbstractData Build Script 
echo Restoring packages...
nuget restore
echo Building project...
msbuild AbstractData.sln /property:Configuration=msbuild
