function Get-Dir([string]$Path = "C:\TEMP")
{
    if(Test-Path -Path $Path -PathType Container)
    {
        [string[]]$Items = @("");
        Get-ChildItem -Path $Path | ForEach-Object { $Items += $_.BaseName}
        Write-Host $Items
    }
    else
    {
        Write-Error "Could not find the folder or it do not exist: `"$path`"";
    }
}
function Test()
{
    Write-Host "TEST:$Global:yn";
}