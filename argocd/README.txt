# ArgoCD Setup

## Aplicar no cluster

kubectl apply -f application.yaml -n argocd

## Verificar

kubectl get applications -n argocd

## Sync manual (se necessário)

argocd app sync canadasoftware-api

## Ver status

argocd app get canadasoftware-api

