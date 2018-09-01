class Target {
    [parameter(Mandatory=$true)][ScriptBlock] $Actions
    [parameter(Mandatory=$true)][string] $Comment
    [string[]] $Depends
}

function New-Target() {
    param (
        [parameter(Mandatory=$true)][ScriptBlock] $Actions,
        [parameter(Mandatory=$true)][string] $Comment,
        [string[]] $Depends = @()
    )
    $res = New-Object Target
    $res.Actions = $Actions
    $res.Comment = $Comment
    $res.Depends = $Depends
    return $res
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