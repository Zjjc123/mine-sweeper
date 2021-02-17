@echo off
powershell Compress-7Zip -Path ".\mine-sweeper\bin\x86\Release" -ArchiveFileName "mine-sweeperx86.zip" -Format Zip -OutputPath "./"
powershell Compress-7Zip -Path ".\mine-sweeper\bin\x64\Release" -ArchiveFileName "mine-sweeperx64.zip" -Format Zip -OutputPath "./"
:exit
@echo on