@{

RootModule = 'PSBuild.psm1'

ModuleVersion = '0.0.0.1'
GUID = 'efb3c583-ad7a-42d2-b291-96c81607abea'
Author = 'Siegmentation Fault'
CompanyName = 'unknown'
Copyright = '(c) 2018 Siegmentation Fault. All rights reserved..'
Description = 'Simple GNU Make-like build system'
FunctionsToExport = @('New-Target', 'Start-Building')
CmdletsToExport = @()
VariablesToExport = '*'
AliasesToExport = @()

# Личные данные для передачи в модуль, указанный в параметре RootModule/ModuleToProcess. Он также может содержать хэш-таблицу PSData с дополнительными метаданными модуля, которые используются в PowerShell.
PrivateData = @{

    PSData = @{

        # Теги, применимые к этому модулю. Они помогают с обнаружением модуля в онлайн-коллекциях.
        # Tags = @()

        # URL-адрес лицензии для этого модуля.
        # LicenseUri = ''

        # URL-адрес главного веб-сайта для этого проекта.
        # ProjectUri = ''

        # URL-адрес значка, который представляет этот модуль.
        # IconUri = ''

        # Заметки о выпуске этого модуля
        # ReleaseNotes = ''

    } # Конец хэш-таблицы PSData

}

}