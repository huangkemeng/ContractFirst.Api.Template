$networks = docker network ls
$networkExists = $networks | Where-Object { $_ -like "*docker-network*" }
if(!$networkExists )
{
   docker network create docker-network
}
docker  container stop ContractFirst.Api
docker  container rm ContractFirst.Api
docker  image rm ContractFirst.Api:latest
docker build  --tag ContractFirst.Api:latest . 
docker run -id --name ContractFirst.Api --restart=no  --network=docker-network  -p 8006:80 -p 8106:443  -e ASPNETCORE_ENVIRONMENT=Development ContractFirst.Api:latest 