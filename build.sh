#!/bin/bash

# this is needed to get dotnet to run on netlify build image
export DOTNET_ROOT=/opt/buildhome/.dotnet
#install dotent 2.x, required for wyam to work
./dotnet-install.sh --version 3.0.103
# instal waym tools
dotnet tool install -g fornax
# run wyam build
fornax build
