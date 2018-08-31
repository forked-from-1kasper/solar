param (
    [string] $Target = "build",
    [System.ConsoleColor] $CommandColor = [System.ConsoleColor]::Gray,
    [System.ConsoleColor] $InfoColor = [System.ConsoleColor]::White
)

function New-Target() {
    param (
        [parameter(Mandatory=$true)][ScriptBlock] $Actions,
        [parameter(Mandatory=$true)][string] $Comment,
        [string[]] $Depends = @()
    )
    New-Object -TypeName PSObject -Property @{
        'Actions' = $Actions;
        'Comment' = $Comment;
        'Depends' = $Depends
    }
}

function PrintAndEvaluate() {
    param (
        [parameter(Mandatory=$true)][string] $Comment,
        [parameter(Mandatory=$true)][ScriptBlock] $ScriptBlock
    )
    Write-Host "Info: $Comment" -ForegroundColor $InfoColor

    $ExecutionContext.InvokeCommand.ExpandString(
        $ScriptBlock.ToString()
    ) -Split "`n" | % {
        Write-Host -NoNewline '>'
        Write-Host -ForegroundColor $CommandColor $_
    }
    & $ScriptBlock
}

function Build([string] $Target, $Targets) {
    if (-Not $Targets.ContainsKey($Target)) {
        Write-Error ('target "{0}" not found' -f $Target)
        exit
    }

    $default = $Targets[$Target]
    $default.Depends |% { Build $_ $Targets }
    PrintAndEvaluate $default.Comment $default.Actions
}

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
    Remove-Item -Recurse $OutputDir
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
        New-Target -Actions $Clean `
                   -Comment "removing $OutputDir"
}

Build $Target $Targets