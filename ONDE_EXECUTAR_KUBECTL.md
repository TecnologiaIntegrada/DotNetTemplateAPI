# 📍 Onde Executar os Comandos kubectl?

## ⚠️ Situação Atual

**Este servidor** (`server.canada-software.com`) **NÃO TEM** kubectl instalado.

Você precisa executar os comandos kubectl em uma **máquina que tenha acesso ao cluster Kubernetes**.

---

## 🖥️ Onde Executar?

### Opção 1: Máquina de Administração do Cluster

Se você tem uma máquina/servidor específico para gerenciar o Kubernetes:

1. Acesse essa máquina (SSH, RDP, etc)
2. Verifique se kubectl está instalado:
   ```bash
   kubectl version --client
   ```
3. Execute os comandos

### Opção 2: Seu Computador Local

Se você gerencia o cluster do seu computador:

1. Abra o terminal (Linux/Mac) ou PowerShell (Windows)
2. Verifique se kubectl está instalado:
   ```bash
   kubectl version --client
   ```
3. Execute os comandos

### Opção 3: Cloud Provider Console

Se o cluster está em um provedor de nuvem:

#### AWS EKS
1. Acesse: AWS Console → EKS → Clusters
2. Clique no seu cluster
3. Clique em **"Connect"** ou **"Cloud Shell"**
4. Execute: `kubectl create namespace dev`

#### Google GKE
1. Acesse: Google Cloud Console → Kubernetes Engine
2. Clique em **"Activate Cloud Shell"** (ícone >_)
3. Execute: `gcloud container clusters get-credentials <CLUSTER_NAME>`
4. Execute: `kubectl create namespace dev`

#### Azure AKS
1. Acesse: Azure Portal → Kubernetes services
2. Clique no seu cluster
3. Clique em **"Cloud Shell"**
4. Execute: `az aks get-credentials --resource-group <RG> --name <CLUSTER>`
5. Execute: `kubectl create namespace dev`

---

## 📋 Comandos a Executar (Nessa Ordem)

```bash
# 1. Criar namespace
kubectl create namespace dev

# 2. Verificar que foi criado
kubectl get namespaces | grep dev

# 3. Ver application do ArgoCD
kubectl get applications -n argocd

# 4. Após ArgoCD sincronizar, ver pods
kubectl get pods -n dev

# 5. Ver services
kubectl get services -n dev

# 6. Obter IP do LoadBalancer (IMPORTANTE para Cloudflare!)
kubectl get service api-service -n dev -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
```

---

## 🎯 Alternativa: Via Interface do ArgoCD

Se você NÃO conseguir executar kubectl em NENHUM lugar:

### Configure CreateNamespace Automático

1. No ArgoCD, edite a application:
   - Clique em **APP DETAILS**
   - Clique em **EDIT**
   - Em **SYNC OPTIONS**, adicione:
     - ☑ **CREATE NAMESPACE**
   - Salve

2. Isso faz o ArgoCD criar o namespace automaticamente

---

## 🔧 Instalar kubectl Neste Servidor (Opcional)

Se quiser instalar kubectl neste servidor (`server.canada-software.com`):

```bash
cd /projetos/CanadaSoftware.ApiDotNet
sudo ./INSTALAR_KUBECTL.sh
```

**Mas você também precisará**:
- Arquivo kubeconfig (`~/.kube/config`)
- Credenciais do cluster
- Permissões de acesso

---

## ✅ Resumo

### Você Tem 3 Opções:

**1. Executar kubectl em outra máquina** (recomendado)
   - Máquina de admin do cluster
   - Seu computador local
   - Cloud Shell do provedor

**2. Configurar CreateNamespace no ArgoCD** (mais fácil)
   - ArgoCD cria namespace automaticamente
   - Não precisa de kubectl

**3. Instalar kubectl aqui** (mais trabalhoso)
   - Precisa configurar kubeconfig
   - Precisa credenciais do cluster

---

## 🎯 RECOMENDAÇÃO

**Use a OPÇÃO 2** (Configure CreateNamespace no ArgoCD):

1. ArgoCD → dotnettemplateapi → APP DETAILS → EDIT
2. SYNC OPTIONS → ☑ **CREATE NAMESPACE**
3. SAVE
4. SYNC → SYNCHRONIZE

Isso resolve SEM precisar de kubectl!

---

**Me diga qual opção você vai usar!**

