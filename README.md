# ShoppingAppMicroServices
Clone Repository, To Use your code edit deployment yaml files accordingly to use your docker account.  

Docker > Settings > Kubernetes > Enable Kubernetes
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml  

Add this line to your hosts file (Nginx needs a domain to work)  
```127.0.0.1 shoppingapp.com  ``` 

#create mssql secret  
```kubectl create secret generic shoppingapp-mssql --from-literal=SA_PASSWORD="LONG3ST_pa55w0rd"```   

#create postgres secret  
```kubectl create secret generic orderingapi-postgres --from-literal=PG_PASSWORD="LONG1SH_pa55w0rd"```   



—> Go To Services/K8S on terminal  

```kubectl apply -f ingress-nginx-srv.yaml```  
```kubectl apply -f shoppingapp-mssql-local-pvc.yaml```  
```kubectl apply -f shoppingapp-mssql-depl.yaml```  
```kubectl apply -f rabbitmq-depl.yaml```  


```kubectl apply -f basketapi/basketapi-redis-local-pvc.yaml```  
```kubectl apply -f basketapi/basketapi-redis-depl.yaml```  
```kubectl apply -f basketapi/basketapi-depl.yaml```  

```kubectl apply -f catalogapi/catalogapi-depl.yaml```  
```kubectl apply -f identityapi/identityapi-depl.yaml```  

```kubectl apply -f orderingapi/orderingapi-postgres-local-pvc.yaml```  
```kubectl apply -f orderingapi/orderingapi-postgres-depl.yaml```  

```kubectl apply -f orderingapi/orderingapi-depl.yaml```  

—> Go to Web/webspa folder on terminal  
```npm install```  
```npm start```  
