#!/bin/bash
HELM_REPO=$bamboo_HELM_REPO

#create env if not present
/usr/local/bin/kubectl create ns staging

#kubectl run app-dotnet --image=nhatthai/dotnet3.0-backgroundjobs:demo --port=8080 -n staging

#rm -rf helm
/usr/local/bin/helm repo add helm $HELM_REPO
/usr/local/bin/helm repo update

#helm install
/usr/local/bin/helm install --namespace staging --name dotnetbackgroundjobs-demo-stage --set image.repository=nhatthai/dotnet3.0-backgroundjobs,image.tag=demo helm
