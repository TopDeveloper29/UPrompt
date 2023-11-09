class PsHamdler
{
    [scriptblock] $ScriptBlock = {};
    [string] $LastInput = "";
    [string] $LastOutput = "";
    [string[]] $InputHistory = @("");
    [string[]] $OutputHistory = @();

    hidden [void] Run($Command)
    {
            $this.LastInput = $Command;
            $this.InputHistory += $Command;
            Invoke-Expression $Command;
            $this.LastOutput = Get-History -Count 1 | Select-Object -ExpandProperty CommandLine
            $this.OutputHistory = Get-History | Select-Object -ExpandProperty CommandLine
    }

    [void] Start()
    {
        [string]$ReadInput = "";
        Invoke-Expression $this.ScriptBlock.ToString();

        do
        {
            $ReadInput = Read-Host;
            $this.Run($ReadInput);
            Start-Sleep -Milliseconds 10
        }
        while($true)
    }
}