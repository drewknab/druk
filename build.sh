#!/bin/bash

# this is needed to get dotnet to run on netlify build image
export DOTNET_ROOT=/opt/buildhome/.dotnet

dotnet tool install -g fornax

fornax build