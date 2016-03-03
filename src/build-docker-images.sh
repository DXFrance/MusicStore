#!/bin/bash

echo "Building MusicStore Catalog Docker Image"
docker build -t jcorioland/musicstore-catalog:$1 -f Dockerfile.catalog .

echo "Building MusicStore Checkout Docker Image"
docker build -t jcorioland/musicstore-checkout:$1 -f Dockerfile.checkout .

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front:$1 -f Dockerfile.store .

echo "Done building images"
echo "Pushing images..."

docker login --username="$2" --password="$3" --email="$4"
docker push jcorioland/musicstore-catalog:$1
docker push jcorioland/musicstore-checkout:$1
docker push jcorioland/musicstore-front:$1

docker logout

exit 0