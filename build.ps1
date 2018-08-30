param (
    [string] $Target = "build"
)

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

function PrintAndEvaluate() {
    param (
        [parameter(Mandatory=$true)][ScriptBlock] $ScriptBlock
    )
    $ExecutionContext.InvokeCommand.ExpandString(
        ($ScriptBlock.ToString().Trim() -replace "`n", '' `
                                        -replace '`', '' `
                                        -replace '\s+', ' ')
    ) | write-host -foregroundcolor Green
    & $ScriptBlock
}

function Build($Target) {
    switch ($Target) {
        "build" {
            Build "build-lib"
            mkdir -f $OutputDir | out-null
            PrintAndEvaluate {
                & $FSharpCompiler `
                    -r (join-path $OutputDir $OutputLibrary) `
                    $Main `
                    -o (join-path $OutputDir $OutputBinary) `
                    $FSharpParams
            }
        }
        "build-standalone" {
            Build "build-lib"
            PrintAndEvaluate {
                & $FSharpCompiler `
                    -r (join-path $OutputDir $OutputLibrary) `
                    $Main `
                    -o (join-path $OutputDir $OutputStandaloneBinary) `
                    $FSharpParams --standalone
            }
        }
        "build-lib" {
            PrintAndEvaluate {
                & $FSharpCompiler `
                    $Files -a `
                    -o (join-path $OutputDir $OutputLibrary) `
                    $FSharpParams
            }
        }
        "clean" {
            rm -rec $OutputDir
        }
        default {
            write-error ('unknown target "{0}"' -f $Target)
        }
    }
}

Build $Target