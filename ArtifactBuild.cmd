@echo off
pushd "%~dp0"
powershell Compress-7Zip "mine-sweeper\bin\x86\Release" -ArchiveFileName "SampleX86.zip" -Format Zip -Path ./
powershell Compress-7Zip "mine-sweeper\bin\x64\Release" -ArchiveFileName "SampleX64.zip" -Format Zip -Path ./
:exit
popd
@echo on