rem https://docs.microsoft.com/ru-ru/visualstudio/ide/managing-external-tools?view=vs-2019
rem Project property-События сборки - Коммандная строка события перед сборкой -Изменение перед сборкой 
rem $(ProjectDir)PreBuildEvent.cmd $(ConfigurationName)
@echo off
echo ------------
echo %0
echo ------------
set App_config=App.config
set FolderScript=%~d0%~p0
echo FolderScript=%FolderScript%
set ConfigurationName=%1
set Project.exe=%2
echo Project.exe=%Project.exe%
set FolderSourceModScripting=%3
echo FolderSourceModScripting=%FolderSourceModScripting%
echo ConfigurationName=%ConfigurationName%
echo Hostname=%COMPUTERNAME%
cd
goto :ALL
:DEBUG
IF /I NOT "%ConfigurationName%"=="DEBUG" GOTO :RELEASE
echo DEBUG Config
set FileConfig=%FolderScript%%COMPUTERNAME%.%App_config%
echo %FileConfig%
IF EXIST "%FileConfig%" copy /y "%FileConfig%" "%FolderScript%%App_config%"&goto :EOF
set file=%FolderScript%DEBUG.%App_config%
echo %FileConfig%
IF EXIST "%FileConfig%" copy /y "%FileConfig%" "%FolderScript%%App_config%"&goto :EOF

:RELEASE
if /I NOT "%ConfigurationName%"=="RELEASE" GOTO :NO_CONFIG
echo RELEASE Config
set FileConfig=%FolderScript%RELEASE.%App_config%
echo %FileConfig%
IF EXIST "%FileConfig%" copy /y "%FileConfig%" "%FolderScript%%App_config%"&goto :EOF

:ALL
copy /y %Project.exe% %FolderSourceModScripting%
rem copy /y "%FolderScript%\xcopy01.cmd" "%FolderScript%\BIN\%ConfigurationName%"
goto :EOF 

:NO_CONFIG
Echo NO App.config