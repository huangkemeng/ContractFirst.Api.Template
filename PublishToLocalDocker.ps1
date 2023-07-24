docker  container stop collection_system_api
docker  container rm collection_system_api
docker  image rm collection_system_api:latest
docker build --tag collection_system_api:latest .
docker run -id --name collection_system_api --restart=always  --network=local-network -p 5080:80 -p 5043:443  -e ASPNETCORE_ENVIRONMENT=Development collection_system_api:latest 