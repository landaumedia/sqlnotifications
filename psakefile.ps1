$erroractionpreference = "Stop"

properties {
    $location = (get-location);
    $outdir = (join-path $location "Build");
    $artifactsdir = (join-path $outdir "Artifacts");
    $bindir = (join-path $outdir "Bin");
	$lmNuget = "\\artus\8 EDV\8-2 Entwicklung\NuGet";
}

task ci -depends rebuild

task Help {
}

task Check {

}

task Clean {
	[void](rmdir -force -recurse $outdir -ea SilentlyContinue)
}

task Prepare {
  exec { .nuget\nuget.exe restore }
}


task Rebuild -depends Clean,Prepare {
	exec { msbuild /nologo /v:minimal /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/SqlNotifications/;SolutionDir=$solution/" "Source/SqlNotifications/SqlNotifications.csproj" }
}

task Test -depends Clean {
    [void](mkdir $artifactsdir)

  exec { .nuget\nuget.exe restore }

  exec {.nuget\nuget install Machine.Specifications.Runner.Console -OutputDirectory Packages}
	$mspecdir = (resolve-path ".\Packages\Machine.Specifications.Runner.Console.0.*\")
	$mspec = @("$mspecdir\tools\mspec-x86-clr4.exe", "--xml", "$artifactsdir\mspec-results.xml", "--html", "$artifactsdir\mspec-results.html");

	foreach($testProj in (dir -Filter ".\Source\*.Tests")){
		exec { msbuild /nologo /v:m /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/$testProj" "Source/$testProj/$testProj.csproj" }
		$mspec += "$bindir\$testProj\$testProj.dll";
	}

	exec { &([scriptblock]::create($mspec -join ' ')) }

	$xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
  $xslt.Load("Packages\MSpecToJUnit\MSpecToJUnit.xlt");
  $xslt.Transform("$artifactsdir\mspec-results.xml", "$artifactsdir\junit-results.xml");
}

task Deploy {
    exec { msbuild /nologo /v:minimal /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/SqlNotifications/;SolutionDir=$solution/" "Source/SqlNotifications/SqlNotifications.csproj" }
    push-location "$bindir/"
    copy "$location/Deploy/SqlNotifications.nuspec" $bindir
    $version = ([System.Diagnostics.FileVersionInfo]::GetVersionInfo("$bindir\SqlNotifications\CommandCenter.SqlNotifications.dll").productVersion);
    exec { ..\..\.NuGet\NuGet.exe pack "$bindir\SqlNotifications.nuspec" /version "$version" }
	move "$bindir\LandauMedia.CommandCenter.SqlNotifications.*.nupkg" $lmNuget
    pop-location
}
