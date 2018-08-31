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

$Files = @(
    'improvements.fs'
    'constants.fs'
    'vector.fs'
    'body.fs'
    'physics.fs'
    'graphics.fs'
    'xml.fs'
    'keymap.fs'
)
$Main = "main.fs"

$Targets = @{
    'build' =
        New-Target -Comment "building executable $OutputBinary" `
                   -Depends @("build-lib") {
            New-Item -Type Directory -Force $OutputDir | Out-Null
            & $FSharpCompiler `
                -r (Join-Path $OutputDir $OutputLibrary) `
                $Main `
                -o (Join-Path $OutputDir $OutputBinary) `
                $FSharpParams
        };
    'build-standalone' =
        New-Target -Comment "building standalone executable $OutputStandaloneBinary" `
                   -Depends @("build-lib") {
            & $FSharpCompiler `
                -r (Join-Path $OutputDir $OutputLibrary) `
                $Main `
                -o (Join-Path $OutputDir $OutputStandaloneBinary) `
                $FSharpParams --standalone
        };
    'build-lib' =
        New-Target -Comment "building library $OutputLibrary" {
            & $FSharpCompiler `
                $Files -a `
                -o (Join-Path $OutputDir $OutputLibrary) `
            $FSharpParams
        };
    'clean' =
        New-Target -Comment "removing $OutputDir" {
            Remove-Item -Confirm -Recurse $OutputDir
        }
}

Build $Target $Targets