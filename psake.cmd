@rem ==========================================================================
@rem =                                                                        =
@rem =            Autodeploy Psake script from SW                             =
@rem =            v1.0                                                        =
@rem =                                                                        =
@rem ==========================================================================

@ECHO OFF
SET BASEPATH=%~dp0%
SET NUGETPATH=%BASEPATH%.nuget\
SET NUGET=%NUGETPATH%nuget.exe

rem check for nuget
IF NOT EXIST %NUGET% (
	ECHO .nuget\nuget.exe dose not exists. Please activate nuget package restore.
	goto END
)

rem remove installed version to get the latest
FOR /D %%p IN ("%BASEPATH%Packages\LandauMedia.PSake.Extended*") DO rmdir "%%p" /s /q >nul 2>nul

rem ensure psake.extend is installed
%NUGET% install -OutputDirectory Packages LandauMedia.PSake.Extended >nul

rem get psake path from existing folder with version number
for /f "tokens=*" %%F in ('dir /b /a:d "%BASEPATH%Packages\LandauMedia.PSake.Extended*"') do call set PSAKEPATH=%BASEPATH%Packages\%%F\

SET PSAKE=%PSAKEPATH%psake.cmd
%PSAKE% -nologo %*

:END 
