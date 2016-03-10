#!/bin/bash

# get app version from project.json
appVersion=$(cat MusicStore/project.json | jq -r '.version')
echo "Building Docker Front for MusicStore Version $appVersion"

echo "Building MusicStore Front Docker Image"
docker build -t jcorioland/musicstore-front:$appVersion -f Dockerfile.store .

docker login --username="$1" --password="$2" --email="$3"

echo "Pushing Front image..."
docker push jcorioland/musicstore-front:$appVersion

docker logout

exit 0