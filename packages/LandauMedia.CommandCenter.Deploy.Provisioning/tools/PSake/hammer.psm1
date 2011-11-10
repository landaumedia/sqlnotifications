#Requires -Version 2.0

param(

  [Parameter(Position=0, Mandatory=0)]
  [string]$scriptPath = $(Split-Path -parent $MyInvocation.MyCommand.path),
  
  [Parameter(Position=1, Mandatory=0)]
  [string]$AppMarketPath = '\\artus\8 EDV\8-2 Entwicklung\AppMarket',
  
  [Parameter(Position=2, Mandatory=0)]
  [string]$HammerOutDir = 'build\Hammer'
)

function Export-Hammer
{  

	[CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)] [string]$ServiceSpecFile,
		[Parameter(Position=1,Mandatory=1)] [string]$BinaryDirectory,
		[Parameter(Position=2,Mandatory=1)] [string]$AssemblyName,
		[Parameter(Position=3,Mandatory=1)] [string]$Environment
    )
	
	$hammerToolPath = join-path $scriptPath "..\Hammer\Hammer.exe"
	
	write-host "Start Hammer Deploy of '$ServiceSpecFile' from '$binaryDirectory' of Environment:'$Environment'"
	write-host "Use Assembly: $AssemblyName"
	
	mkdir $HammerOutDir
	
	$tempspecFileName = [System.IO.Path]::GetFilename("$ServiceSpecFile")
	$tempSpecPath = join-path $HammerOutDir $tempspecFileName
	
	$assemblyPath = join-path "$BinaryDirectory" "$AssemblyName"
	$version = ([System.Diagnostics.FileVersionInfo]::GetVersionInfo($assemblyPath).productVersion);
	
	$xml = [xml](get-content $ServiceSpecFile);
	$xml.package.metadata.id = $($xml.package.metadata.id) -replace '{env}', $Environment.ToLower();
	$xml.package.metadata.version = $version;
	$xml.Save($tempSpecPath);
	
	
	& "$hammerToolPath" pack -f:"$tempSpecPath" -s:"$BinaryDirectory" -o:"$HammerOutDir"
	copy -Force "$HammerOutDir\*.nupkg" -Destination $AppMarketPath
	
	rmdir -Force -recurse $HammerOutDir
}
