#!/bin/bash

echo "Building MusicStore Catalog Docker Image"
docker build -t jcorioland/musicstore-catalog -f Dockerfile.catalog .
docker tag jcorioland/musicstore-catalog jcorioland/musicstore-catalog:$1

echo "Building MusicStore Checkout Docker Image"
docker build -t jcorioland/musicstore-checkout -f Dockerfile.checkout .
docker tag jcorioland/musicstore-checkout jcorioland/musicstore-checkout:$1

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front -f Dockerfile.store .
docker tag jcorioland/musicstore-front jcorioland/musicstore-front:$1

echo "Done building images"
echo "Pushing images..."

docker login --username="$2" --password="$3" --email="$4"

echo "Pushing Catalog image..."
docker push jcorioland/musicstore-catalog

echo "Pushing Checkout image..."
docker push jcorioland/musicstore-checkout

echo "Pushing Front image..."
docker push jcorioland/musicstore-front

docker logout

exit 0