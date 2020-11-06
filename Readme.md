
##Reference - https://docs.microsoft.com/en-us/dotnet/architecture/containerized-lifecycle/design-develop-containerized-apps/build-aspnet-core-applications-linux-containers-aks-kubernetes

1. Create Flippy Database in MongoDB using MongoDBCompass
2. Create 3 collections - Flippy, DeliveryAgent, ShopPartner using MongoDBCompass
3. Create the Apache Kafka topics using file 'TopicScript.txt'
4. Create Docker Images for all 3 services
5. Push the images to Azure ACR
5. Run below Kubernetes commands to manage the containers
   and deploy the 3 services to AZURE Cloud

az aks get-credentials --resource-group explore-docker-aks-rg --name explore-docker-aks

az aks update --name explore-docker-aks --resource-group explore-docker-aks-rg --attach-acr exploredocker

kubectl apply -f deploy-Flippy.yml

kubectl apply -f deploy-DeliveryAgent.yml

kubectl apply -f deploy-ShopPartner.yml

kubectl get all

kubectl create clusterrolebinding kubernetes-dashboard --clusterrole=cluster-admin --serviceaccount=kube-system:kubernetes-dashboard

az aks browse --resource-group exploredocker-aks-rg --name explore-docker-aks