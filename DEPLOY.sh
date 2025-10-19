#!/bin/bash

# ================================================================
# Script de Deploy - CanadaSoftware.ApiDotNet
# ================================================================
# Deploy no Kubernetes usando APENAS Kubernetes Secrets
# SEM dependência de AWS ou Terraform
# ================================================================

set -e

# Cores
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

ENV=${1:-dev}

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Deploy - Canada Software API (${ENV})${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Passo 1: Verificar pré-requisitos
echo -e "${YELLOW}[1/5] Verificando pré-requisitos...${NC}"
command -v kubectl >/dev/null 2>&1 || { echo "kubectl não encontrado!"; exit 1; }
command -v helm >/dev/null 2>&1 || { echo "helm não encontrado!"; exit 1; }
echo -e "${GREEN}✓ kubectl e helm OK${NC}"
echo ""

# Passo 2: Criar namespace
echo -e "${YELLOW}[2/5] Criando namespace ${ENV}...${NC}"
kubectl create namespace ${ENV} --dry-run=client -o yaml | kubectl apply -f -
echo -e "${GREEN}✓ Namespace criado${NC}"
echo ""

# Passo 3: Criar secrets
echo -e "${YELLOW}[3/5] Criando Kubernetes Secrets...${NC}"
kubectl create secret generic app-secrets \
  --from-literal=CONNECTIONSTRINGS__DEFAULT="Host=postgresql-service.${ENV}.svc.cluster.local;Port=5432;Database=appdatabase;Username=postgres;Password=Change@Me123!" \
  --from-literal=CONNECTIONSTRINGS__KAFKA="broker:9092" \
  --from-literal=KAFKA__CLUSTERAPIKEY="" \
  --from-literal=KAFKA__CLUSTERAPISECRET="" \
  -n ${ENV} \
  --dry-run=client -o yaml | kubectl apply -f -
echo -e "${GREEN}✓ Secrets criados${NC}"
echo ""

# Passo 4: Deploy com Helm
echo -e "${YELLOW}[4/5] Deployando com Helm...${NC}"
helm upgrade --install api-service ./charts \
  -f ./charts/values.yaml \
  -n ${ENV} \
  --wait \
  --timeout 10m
echo -e "${GREEN}✓ Deploy concluído${NC}"
echo ""

# Passo 5: Verificar
echo -e "${YELLOW}[5/5] Verificando pods...${NC}"
kubectl get pods -n ${ENV}
echo ""

echo -e "${GREEN}✓ Deploy finalizado com sucesso!${NC}"
echo ""
echo -e "${YELLOW}Acessar aplicação:${NC}"
echo "kubectl port-forward svc/api-service 8080:80 -n ${ENV}"
echo "  → Swagger: http://localhost:8080/swagger"
echo "  → Health: http://localhost:8080/healthz"
echo ""

