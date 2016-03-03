#!/bin/bash

echo "Building MusicStore Catalog Docker Image"
docker build -t jcorioland/musicstore-catalog -f Dockerfile.catalog .

echo "Building MusicStore Checkout Docker Image"
docker build -t jcorioland/musicstore-checkout -f Dockerfile.checkout .

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front -f Dockerfile.store .

exit 0