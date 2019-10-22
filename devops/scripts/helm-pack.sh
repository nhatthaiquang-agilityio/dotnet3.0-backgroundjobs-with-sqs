
#!/bin/bash

#Helm pack
echo -e "\nPacking Helm chart"

[ -d pkg/helm ] && rm -rf pkg/helm
mkdir -p pkg/helm

/usr/local/bin/helm package -d pkg/helm helm || errorExit "Packing helm chart devops/dotnet-app failed"
