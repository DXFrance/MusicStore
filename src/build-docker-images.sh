#!/bin/bash

# get app version from project.json
appVersion=$(cat MusicStore/project.json | jq -r '.version')
echo "Building Docker images for MusicStore Version $appVersion"

echo "Building MusicStore Catalog Docker Image"
docker build -t jcorioland/musicstore-catalog -f Dockerfile.catalog .
docker tag jcorioland/musicstore-catalog jcorioland/musicstore-catalog:$appVersion

echo "Building MusicStore Checkout Docker Image"
docker build -t jcorioland/musicstore-checkout -f Dockerfile.checkout .
docker tag jcorioland/musicstore-checkout jcorioland/musicstore-checkout:$appVersion

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front -f Dockerfile.store .
docker tag jcorioland/musicstore-front jcorioland/musicstore-front:$appVersion

echo "Done building images"
echo "Pushing images..."

docker login --username="$2" --password="$3" --email="$4"

echo "Pushing Catalog image..."
docker push jcorioland/musicstore-catalog:$appVersion

echo "Pushing Checkout image..."
docker push jcorioland/musicstore-checkout:$appVersion

echo "Pushing Front image..."
docker push jcorioland/musicstore-front:$appVersion

docker logout

exit 0