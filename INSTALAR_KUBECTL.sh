#!/bin/bash

echo "╔════════════════════════════════════════════════════════════╗"
echo "║                                                            ║"
echo "║         Instalando kubectl no Debian 11                    ║"
echo "║                                                            ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Atualizar repositórios
echo "[1/5] Atualizando repositórios..."
apt-get update -qq

# Instalar dependências
echo "[2/5] Instalando dependências..."
apt-get install -y apt-transport-https ca-certificates curl gnupg

# Adicionar chave GPG do Kubernetes
echo "[3/5] Adicionando chave GPG..."
mkdir -p /etc/apt/keyrings
curl -fsSL https://pkgs.k8s.io/core:/stable:/v1.28/deb/Release.key | gpg --dearmor -o /etc/apt/keyrings/kubernetes-apt-keyring.gpg

# Adicionar repositório Kubernetes
echo "[4/5] Adicionando repositório Kubernetes..."
echo 'deb [signed-by=/etc/apt/keyrings/kubernetes-apt-keyring.gpg] https://pkgs.k8s.io/core:/stable:/v1.28/deb/ /' | tee /etc/apt/sources.list.d/kubernetes.list

# Instalar kubectl
echo "[5/5] Instalando kubectl..."
apt-get update -qq
apt-get install -y kubectl

# Verificar instalação
echo ""
echo "════════════════════════════════════════════════════════════"
echo "✅ kubectl instalado com sucesso!"
echo ""
kubectl version --client
echo ""
echo "════════════════════════════════════════════════════════════"
echo ""
echo "PRÓXIMO PASSO: Configurar acesso ao cluster Kubernetes"
echo ""
echo "Você precisa de um arquivo kubeconfig para acessar o cluster."
echo "Normalmente fica em: ~/.kube/config"
echo ""
echo "Se você já tem o arquivo, copie para:"
echo "  mkdir -p ~/.kube"
echo "  cp /caminho/para/kubeconfig ~/.kube/config"
echo ""
echo "Ou configure usando:"
echo "  kubectl config set-cluster ..."
echo ""
echo "Depois teste com:"
echo "  kubectl get nodes"
echo ""

