#!/bin/bash

# get app version from project.json
appVersion=$(cat MusicStore/project.json | jq -r '.version')
echo "Building Docker images for MusicStore Version $appVersion"

echo "Building MusicStore Catalog Docker Image"
docker build -t jcorioland/musicstore-catalog:$appVersion -f Dockerfile.catalog .

echo "Building MusicStore Checkout Docker Image"
docker build -t jcorioland/musicstore-checkout:$appVersion -f Dockerfile.checkout .

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front:$appVersion -f Dockerfile.store .

echo "Done building images"
echo "Pushing images..."

docker login --username="$1" --password="$2" --email="$3"

echo "Pushing Catalog image..."
docker push jcorioland/musicstore-catalog:$appVersion

echo "Pushing Checkout image..."
docker push jcorioland/musicstore-checkout:$appVersion

echo "Pushing Front image..."
docker push jcorioland/musicstore-front:$appVersion

docker logout

exit 0