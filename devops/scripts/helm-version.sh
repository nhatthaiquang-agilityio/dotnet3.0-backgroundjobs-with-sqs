#!/bin/bash

buildNumber=$bamboo_buildNumber

version="0."$buildNumber".0"

sed -i "s/versionhere/$version/g" ./devops/dotnet-app/Chart.yaml