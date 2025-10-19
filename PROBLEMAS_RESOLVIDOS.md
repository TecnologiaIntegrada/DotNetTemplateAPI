# ✅ PROBLEMAS DO ARGOCD RESOLVIDOS

## 🔴 Problemas Identificados

### 1. ❌ Imagem Docker Incorreta

**Antes**:
```yaml
image:
  repository: canadasoftware/api-dotnet
  tag: "1.0.0"
```

**Problema**: Esta imagem NÃO EXISTE no Docker Hub ou GitHub Container Registry!

**Depois** (✅ CORRIGIDO):
```yaml
image:
  repository: ghcr.io/tecnologiaintegrada/dotnettemplateapi
  tag: "main"
  pullPolicy: Always
```

**Por quê?**
- `ghcr.io` = GitHub Container Registry
- `tecnologiaintegrada/dotnettemplateapi` = Nome correto do repositório
- `main` = Tag da branch principal (atualizada pelo CI/CD)
- `pullPolicy: Always` = Sempre puxa a imagem mais recente

---

### 2. ❌ Service Type Errado

**Antes**:
```yaml
service:
  type: ClusterIP
```

**Problema**: `ClusterIP` é um IP **INTERNO** do cluster, sem acesso externo!

**Depois** (✅ CORRIGIDO):
```yaml
service:
  type: LoadBalancer
```

**Por quê?**
- `LoadBalancer` cria um IP **EXTERNO** acessível pela internet
- Você receberá um IP público para configurar no Cloudflare
- Kubernetes do seu provedor (AWS/GCP/Azure) provisionará automaticamente

---

### 3. ⚠️ Ingress Desnecessário

**Antes**: Ingress estava sempre habilitado

**Depois** (✅ MELHORADO):
```yaml
ingress:
  enabled: false  # Desabilitado por padrão
```

**Por quê?**
- Com `LoadBalancer`, você NÃO precisa de Ingress
- Ingress é útil quando se usa `ClusterIP` + Nginx Ingress Controller
- LoadBalancer expõe diretamente, mais simples!

---

## 🚀 Como Vai Funcionar Agora

### Fluxo de Deploy

1. **GitHub Actions** compila e gera imagem Docker
2. **Push** para `ghcr.io/tecnologiaintegrada/dotnettemplateapi:main`
3. **ArgoCD** detecta mudança no repositório
4. **Helm** gera manifestos com valores corretos
5. **Kubernetes** cria:
   - Pod da API (usando imagem correta)
   - Service do tipo LoadBalancer
   - PostgreSQL (StatefulSet)
   - SEQ (Deployment)
6. **LoadBalancer** provisiona IP externo
7. **Você** copia o IP e configura no Cloudflare

---

## 📋 Próximos Passos

### 1. Aguardar ArgoCD Sincronizar (2-3 minutos)

Na interface do ArgoCD:
- Clique em **"REFRESH"**
- Ou clique em **"SYNC"** → **"SYNCHRONIZE"**

### 2. Verificar Pods Criados

```bash
kubectl get pods -n dev

# Deve mostrar:
# api-deployment-xxxxx    1/1  Running
# postgresql-0            1/1  Running
# seq-xxxxx               1/1  Running
```

### 3. Obter IP do LoadBalancer

```bash
kubectl get service api-service -n dev

# Saída esperada:
# NAME          TYPE           CLUSTER-IP      EXTERNAL-IP       PORT(S)
# api-service   LoadBalancer   10.x.x.x        203.0.113.45      80:30123/TCP
#                                              ↑ ESTE IP AQUI
```

**IMPORTANTE**: O `EXTERNAL-IP` pode demorar 2-5 minutos para aparecer!

Se mostrar `<pending>`, aguarde mais um pouco.

### 4. Configurar no Cloudflare

1. Acesse o painel do Cloudflare
2. Vá em **DNS** → **Add Record**
3. Configure:
   - **Type**: `A`
   - **Name**: `dotnettemplateapi` (ou `@` para raiz)
   - **IPv4**: `203.0.113.45` ← IP do LoadBalancer
   - **Proxy status**: Orange cloud (proxied) ✅
   - **TTL**: Auto

4. Salve

### 5. Testar Acesso

Aguarde propagação DNS (1-5 minutos) e acesse:

```
http://dotnettemplateapi.canada-software.com/healthz
http://dotnettemplateapi.canada-software.com/swagger
```

---

## 🔧 Comandos Úteis

### Ver Logs da API

```bash
kubectl logs -f deployment/api-deployment -n dev
```

### Ver Status dos Pods

```bash
kubectl get pods -n dev -w
```

### Ver Eventos

```bash
kubectl get events -n dev --sort-by='.lastTimestamp'
```

### Ver IP do LoadBalancer (copiar para Cloudflare)

```bash
kubectl get service api-service -n dev -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
```

---

## ⚠️ Se Ainda Houver Problemas

### Imagem não baixa (ImagePullBackOff)

**Causa**: Imagem privada ou não existe

**Solução**:
```bash
# Verificar se imagem existe
docker pull ghcr.io/tecnologiaintegrada/dotnettemplateapi:main

# Se precisar autenticar no GHCR
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=SEU_USUARIO \
  --docker-password=SEU_TOKEN \
  -n dev
```

### LoadBalancer fica em Pending

**Causa**: Cluster não suporta LoadBalancer

**Opções**:
1. Usar Ingress (habilitar `ingress.enabled: true`)
2. Usar NodePort
3. Configurar MetalLB (para bare-metal)

### PostgreSQL não sobe (PVC pending)

**Causa**: Cluster sem StorageClass

**Solução**:
```bash
# Ver StorageClasses disponíveis
kubectl get storageclass

# Se não houver nenhuma, criar:
# (depende do seu provedor)
```

---

## ✅ Status Atual

- ✅ Imagem Docker corrigida
- ✅ LoadBalancer habilitado
- ✅ Ingress desabilitado (opcional)
- ✅ Push para GitHub completo
- ⏳ ArgoCD sincronizando...
- ⏳ Aguardando IP do LoadBalancer...

**Próximo**: Copiar IP do LoadBalancer e configurar no Cloudflare! 🎉

