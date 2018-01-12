   Function Trigger-SchedulerFunction {
    param (
        [Parameter(Mandatory=$true)][string]$resourceGroupName,
        [Parameter(Mandatory=$true)][string]$pairName,
        [Parameter(Mandatory=$true)][string]$backupConnection,
        [Parameter(Mandatory=$true)][string]$productionConnection,
        [Parameter(Mandatory=$true)][string]$startTime
    )

        Login-AzureRmAccount
        
        $functionName = (Get-AzureRmResource | where {$_.ResourceGroupName –eq "$resourceGroupName" -and $_.ResourceType –eq "Microsoft.Web/sites"}).Name

        #when debugging functions set url to local
        #$url = "https://" + (Get-AzureWebsite -Name "$functionName").EnabledHostNames[0]               
        $url = "http://localhost:7071/api"

        $body = '{ "ResourceGroup": "' + $resourceGroupName + '", "PairName": "' + $pairName + '", "BackupConnection" : "'+ $backupConnection + '", "ProductionConnection": "' + $productionConnection + '", "StartTime": "' + $startTime + '" }'
        
        $body | ConvertTo-Json

        Invoke-RestMethod "$url/GetPowerShellTrigger" -Method Post -Body $body 
    }

    Export-ModuleMember -Function Trigger-SchedulerFunction

