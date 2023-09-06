$networks = docker network ls
$networkExists = $networks | Where-Object { $_ -like "*cs-network*" }
if(!$networkExists )
{
   docker network create cs-network
}
docker  container stop csa
docker  container rm csa
docker  image rm collection_system_api:latest
docker build --tag collection_system_api:latest .
docker run -id --name csa --restart=always  --network=cs-network  -p 5080:80 -p 5043:443  -e ASPNETCORE_ENVIRONMENT=Development collection_system_api:latest 