[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

class PowershellHandler {
    hidden [scriptblock] $ScriptBlock = {};
    [string] $LastInput = "";
    [string] $LastOutput = "";
    [string[]] $InputHistory = @("");
    [string[]] $OutputHistory = @("");

    hidden [void] Run($Command) {
            $this.LastInput = $Command;
            $this.InputHistory += $Command;
            Invoke-Expression -Command "$Command";
            $this.LastOutput = Get-History -Count 1 | Select-Object -ExpandProperty CommandLine;
            $this.OutputHistory = Get-History | Select-Object -ExpandProperty CommandLine;
    }
    [void] LoadScript([string]$path) {
        try {
            
            if ((Test-Path -Path "$path" -PathType Leaf) -and $path -like "*.ps1") {
                $scriptContent = Get-Content -Path "$path" -Raw;
                $this.ScriptBlock = [ScriptBlock]::Create($scriptContent);
            }
            else {
                Write-Error "'$path' do not exist or is not a .ps1 file";
            }
        }
        catch {
            Write-Error "Unknow error happen while reading: '$path'";
        }
    }
    [void] Start() {
        [string]$ReadInput = ""
        Invoke-Expression $this.ScriptBlock.ToString()
        do {
            $ReadInput = Read-Host -Prompt "PowershellHandler waiting for input function or command";
            [string]$Variables = Get-Content "$PSScriptRoot`\Variables.ps1" -Force;
            Invoke-Expression -Command "$Variables";
            $this.Run($ReadInput);
            Start-Sleep -Milliseconds 1000;
        }
        while ($true)
    }
}

[string]$LogFile = "C:\LogFiles\UPrompt" + ( Get-Date -Format "yyyy-MM-dd hh-mm" ) + ".log";
Start-Transcript -Path $LogFile;
Write-Host "Create the powershell handler instance";
$PSH = New-Object PowershellHandler;
Write-Host "Loading script file `"$Path`"";
$PSH.LoadScript($Path); 
if($? -eq $false){Write-Error -Message "Fail to load the script file now exiting script!!!"; exit;}
Write-Host "Starting handler..."
$PSH.Start();
Stop-Transcript;