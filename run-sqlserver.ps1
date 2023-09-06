$volumes = docker volume ls
$volumeExists = $volumes | Where-Object { $_ -like "*mssql-volume*" }
if (!$volumeExists)
{
    docker volume create mssql-volume
}
$networks = docker network ls
$networkExists = $networks | Where-Object { $_ -like "*cs-network*" }
if(!$networkExists )
{
   docker network create cs-network
}
docker run --name mssql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=local.nopwd"  --network=cs-network -v mssql-volume:/var/opt/mssql -p 1433:1433 --restart=always -d mcr.microsoft.com/mssql/server:2017-latest
$hostsFile = "$env:SystemRoot\System32\drivers\etc\hosts"
$hostname = "mssql-server"
$ipAddress = "127.0.0.1"
$existingEntry = Get-Content $hostsFile | Where-Object { $_ -like "*$hostname*" }
if ($existingEntry) {
    Write-Host "Host entry for $hostname already exists."
} else {
    $newEntry = "$ipAddress`t$hostname"
    Add-Content -Path $hostsFile -Value $newEntry
    Write-Host "Host entry for $hostname added successfully."
}