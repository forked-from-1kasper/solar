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
    New-Object Target -Property @{
        'Actions' = $Actions;
        'Comment' = $Comment;
        'Depends' = $Depends
    }
}
Export-ModuleMember -Function 'New-Target'

function PrintAndEvaluate() {
    param (
        [parameter(Mandatory=$true)][string] $Comment,
        [parameter(Mandatory=$true)][ScriptBlock] $ScriptBlock,
        [System.ConsoleColor] $CommandColor = [System.ConsoleColor]::Gray,
        [System.ConsoleColor] $InfoColor = [System.ConsoleColor]::White
    )
    Write-Host "Info: $Comment" -ForegroundColor $InfoColor

    $ScriptBlock.ToString() -Split "`n" | % {
        Write-Host -NoNewline '>'
        Write-Host -ForegroundColor $CommandColor $_
    }
    & $ScriptBlock
}

function Start-Building() {
    param (
        [string] $Target,
        $Targets,
        [System.ConsoleColor] $CommandColor = [System.ConsoleColor]::Gray,
        [System.ConsoleColor] $InfoColor = [System.ConsoleColor]::White
    )
    if (-Not $Targets.ContainsKey($Target)) {
        Write-Error ('target "{0}" not found' -f $Target)
        exit
    }

    $default = $Targets[$Target]
    $default.Depends |% { Start-Building $_ $Targets }
    PrintAndEvaluate -CommandColor $CommandColor -InfoColor $InfoColor `
                     $default.Comment $default.Actions
}
Export-ModuleMember -Function 'Start-Building'