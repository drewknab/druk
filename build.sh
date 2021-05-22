#!/bin/bash

# this is needed to get dotnet to run on netlify build image
export DOTNET_ROOT=/opt/buildhome/.dotnet

wget https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y dotnet-runtime-3.1

dotnet --list-runtimes

dotnet tool install -g fornax

fornax build