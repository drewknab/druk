#!/bin/bash

# this is needed to get dotnet to run on netlify build image
export DOTNET_ROOT=/opt/buildhome/.dotnet

./dotnet-install.sh -c --runtime 3.1.0

dotnet --list-runtimes

dotnet tool install -g fornax

fornax build