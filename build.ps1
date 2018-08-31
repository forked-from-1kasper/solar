param (
    [string] $Target = "build",
    [System.ConsoleColor] $CommandColor = [System.ConsoleColor]::Gray,
    [System.ConsoleColor] $InfoColor = [System.ConsoleColor]::White
)

. (Join-Path (Get-Location) buildCore.ps1)

$FSharpCompiler = "fsc"
$FSharpParams = @("--nologo", "--nowarn:82")

$OutputLibrary = "solar.dll"
$OutputBinary = "solar-bin.exe"
$OutputStandaloneBinary = "solar-standalone.exe"
$OutputDir = "Build"

$Files = @('improvements.fs'
           'constants.fs'
           'vector.fs'
           'body.fs'
           'physics.fs'
           'graphics.fs'
           'xml.fs'
           'keymap.fs')
$Main = "main.fs"

$BuildExec = {
    New-Item -Type Directory -Force $OutputDir | Out-Null
    & $FSharpCompiler `
        -r (Join-Path $OutputDir $OutputLibrary) `
        $Main `
        -o (Join-Path $OutputDir $OutputBinary) `
        $FSharpParams
}

$BuildStandalone = {
    & $FSharpCompiler `
        -r (Join-Path $OutputDir $OutputLibrary) `
        $Main `
        -o (Join-Path $OutputDir $OutputStandaloneBinary) `
        $FSharpParams --standalone
}

$BuildLib = {
    & $FSharpCompiler `
        $Files -a `
        -o (Join-Path $OutputDir $OutputLibrary) `
        $FSharpParams
}

$Clean = {
    Remove-Item -Confirm -Recurse $OutputDir
}

$Targets = @{
    'build' =
        New-Target -Actions $BuildExec `
                   -Comment "building executable $OutputBinary" `
                   -Depends @("build-lib");
    'build-standalone' =
        New-Target -Actions $BuildStandalone `
                   -Comment "building standalone executable $OutputStandaloneBinary" `
                   -Depends @("build-lib");
    'build-lib' =
        New-Target -Actions $BuildLib `
                   -Comment "building library $OutputLibrary";
    'clean' =
        New-Target -Comment "removing $OutputDir" `
                   -Actions $Clean
}

Build $Target $Targets