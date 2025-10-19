# 🔄 RECRIAR APPLICATION DO ARGOCD

## Problema

A application `canadasoftware-api` está com cache desatualizado e não está encontrando o diretório `charts/`.

## ✅ Solução: Recriar do Zero

### OPÇÃO 1: Via Interface do ArgoCD (MAIS FÁCIL)

#### Passo 1: Deletar Application Antiga

1. Abra o ArgoCD: `https://seu-argocd.com`
2. Encontre a application: **`canadasoftware-api`** ou **`dotnettemplateapi`**
3. Clique nos **3 pontinhos** (⋮) no canto superior direito
4. Clique em **`DELETE`**
5. Confirme a deleção
6. Aguarde 10 segundos

#### Passo 2: Criar Nova Application

1. No ArgoCD, clique em **`+ NEW APP`**
2. Preencha os campos:

```
General:
  Application Name: dotnettemplateapi
  Project Name: default
  Sync Policy: Automatic
  
  ☑ AUTO-CREATE NAMESPACE
  ☑ PRUNE RESOURCES
  ☑ SELF HEAL

Source:
  Repository URL: https://github.com/TecnologiaIntegrada/DotNetTemplateAPI.git
  Revision: main
  Path: charts
  
Destination:
  Cluster URL: https://kubernetes.default.svc
  Namespace: dev
  
Helm:
  VALUES FILES: values.yaml
```

3. Clique em **`CREATE`**
4. Aguarde 30-60 segundos
5. Clique em **`SYNC`** → **`SYNCHRONIZE`**

---

### OPÇÃO 2: Via kubectl (SE TIVER KUBECTL)

```bash
# 1. Deletar application antiga
kubectl delete application canadasoftware-api -n argocd
kubectl delete application dotnettemplateapi -n argocd

# Aguardar 10 segundos

# 2. Criar nova do GitHub
kubectl apply -f https://raw.githubusercontent.com/TecnologiaIntegrada/DotNetTemplateAPI/main/argocd/RECRIAR_APPLICATION.yaml

# 3. Verificar
kubectl get application dotnettemplateapi -n argocd

# 4. Ver status
kubectl describe application dotnettemplateapi -n argocd
```

---

### OPÇÃO 3: Via kubectl com arquivo local

```bash
cd /projetos/CanadaSoftware.ApiDotNet

# 1. Deletar antiga
kubectl delete application canadasoftware-api -n argocd

# 2. Criar nova
kubectl apply -f argocd/RECRIAR_APPLICATION.yaml -n argocd

# 3. Forçar sync
kubectl patch application dotnettemplateapi -n argocd \
  --type merge \
  -p '{"operation":{"sync":{}}}'
```

---

## 🔍 Verificar se Funcionou

Após criar a nova application:

### Via Interface ArgoCD

Você deve ver:
- **Status**: Synced ✅
- **Health**: Healthy ✅
- **Resources**: 6-8 recursos criados

### Via kubectl

```bash
# Ver application
kubectl get application dotnettemplateapi -n argocd

# Ver pods criados
kubectl get pods -n dev

# Deve mostrar:
# api-deployment-xxxxx    1/1  Running
# postgresql-0            1/1  Running
# seq-xxxxx               1/1  Running
```

---

## 📋 Se Ainda Houver Problemas

### Erro: "app path does not exist"

**Causa**: Cache do ArgoCD ou repositório privado

**Solução**:
1. Verificar se repositório é público:
   https://github.com/TecnologiaIntegrada/DotNetTemplateAPI
   
2. Se for privado, adicionar credenciais:
   - ArgoCD → Settings → Repositories
   - CONNECT REPO
   - Adicionar token do GitHub

### Erro: "Failed to load target state"

**Causa**: Branch ou path incorreto

**Solução**:
1. Verificar na interface:
   - Revision: **main** (não HEAD)
   - Path: **charts** (não charts/)
   
2. Editar application:
   - APP DETAILS → EDIT
   - Corrigir valores

### Erro: ImagePullBackOff nos pods

**Causa**: Imagem Docker privada ou não existe

**Solução**:
```bash
# Verificar se imagem existe
docker pull ghcr.io/tecnologiaintegrada/dotnettemplateapi:main

# Se precisar autenticar
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=SEU_USUARIO \
  --docker-password=SEU_TOKEN_PAT \
  -n dev
```

---

## ✅ Configurações Corretas

```yaml
Application Name: dotnettemplateapi
Repository: https://github.com/TecnologiaIntegrada/DotNetTemplateAPI.git
Branch: main
Path: charts
Namespace: dev
Auto-Sync: Enabled
Self-Heal: Enabled
```

---

## 🎯 Resultado Esperado

Após recriar:

```
Application: dotnettemplateapi
Status: Synced ✅
Health: Healthy ✅

Resources:
- Deployment: api-deployment (1/1 Ready)
- Service: api-service (LoadBalancer)
- StatefulSet: postgresql (1/1 Ready)
- Deployment: seq (1/1 Ready)
- Service: postgresql-service
- Service: seq-service
```

---

**Execute a OPÇÃO 1 (via interface) e me diga o resultado!**

