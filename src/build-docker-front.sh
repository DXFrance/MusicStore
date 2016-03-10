#!/bin/bash

# get app version from project.json
appVersion=$(cat MusicStore/project.json | jq -r '.version')
echo "Building Docker Front for MusicStore Version $appVersion"

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front -f Dockerfile.store .
docker tag jcorioland/musicstore-front jcorioland/musicstore-front:$appVersion

docker login --username="$2" --password="$3" --email="$4"

echo "Pushing Front image..."
docker push jcorioland/musicstore-front:$appVersion

docker logout

exit 0