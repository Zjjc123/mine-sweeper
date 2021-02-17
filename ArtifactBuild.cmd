@echo off
powershell Compress-7Zip -Path ".\mine-sweeper\bin\x86\Release" -ArchiveFileName "SampleX86.zip" -Format Zip -OutputPath "./"
powershell Compress-7Zip -Path ".\mine-sweeper\bin\x64\Release" -ArchiveFileName "SampleX64.zip" -Format Zip -OutputPath "./"
:exit
@echo on