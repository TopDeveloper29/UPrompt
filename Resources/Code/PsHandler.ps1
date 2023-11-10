[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

class PowershellHandler {
    hidden [scriptblock] $ScriptBlock = {}
    [string] $LastInput = ""
    [string] $LastOutput = ""
    [string[]] $InputHistory = @("")
    [string[]] $OutputHistory = @("")

    hidden [void] Run($Command) {
        try {
            $this.LastInput = $Command
            $this.InputHistory += $Command
            Invoke-Expression $Command
            $this.LastOutput = Get-History -Count 1 | Select-Object -ExpandProperty CommandLine
            $this.OutputHistory = Get-History | Select-Object -ExpandProperty CommandLine
        }
        catch {
            Write-Error $Error
        }
    }
    [void] LoadScript([string]$path) {
        try {
            
            if ((Test-Path -Path "$path" -PathType Leaf) -and $path -like "*.ps1") {
                $scriptContent = Get-Content -Path "$path" -Raw
                $this.ScriptBlock = [ScriptBlock]::Create($scriptContent)
            }
            else {
                Write-Error "'$path' do not exist or is not a .ps1 file"
            }
        }
        catch {
            Write-Error "Unknow error happen while reading: '$path'"
        }
    }
    [void] Start() {
        [string]$ReadInput = ""
        Invoke-Expression $this.ScriptBlock.ToString()

        do {
            $ReadInput = Read-Host -Prompt "PowershellHandler waiting for input function or command"
            $this.Run($ReadInput)
            Start-Sleep -Milliseconds 1000
        }
        while ($true)
    }
}

$PSH = New-Object PowershellHandler
$PSH.LoadScript($Path); $PSH.Start()