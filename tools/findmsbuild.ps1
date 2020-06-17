return (Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\' | 
    Get-ItemProperty -Name MSBuildToolsPath | 
    Sort-Object PSChildName | 
    Select-Object -ExpandProperty MSBuildToolsPath -last 1)