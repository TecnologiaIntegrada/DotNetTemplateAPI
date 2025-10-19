# 🔴 CORREÇÃO URGENTE DO ARGOCD

## Problema

**Erro**: `k8s: app path does not exist`

O ArgoCD não está encontrando o diretório `charts/` no GitHub.

## Investigação

✅ **Local**: Diretório `charts/` existe  
✅ **Git**: Commits feitos corretamente  
❌ **GitHub/ArgoCD**: Não está sincronizando

## Causas Possíveis

1. **targetRevision: HEAD** → GitHub não reconhece
2. **Cache do ArgoCD** desatualizado
3. **Repositório privado** sem credenciais no ArgoCD

## ✅ SOLUÇÃO 1: Corrigir Application.yaml

Mudei de `HEAD` para `main`:

```yaml
targetRevision: main  # ← Era HEAD antes
```

## ✅ SOLUÇÃO 2: Forçar Refresh no ArgoCD

### Via Interface

1. Abra o ArgoCD
2. Clique na aplicação `canadasoftware-api`
3. Clique em **"APP DETAILS"**
4. Clique em **"HARD REFRESH"**
5. Aguarde 30 segundos
6. Clique em **"SYNC"** → **"SYNCHRONIZE"**

### Via kubectl (se tiver acesso)

```bash
# Deletar application
kubectl delete -f argocd/application.yaml -n argocd

# Aguardar 10 segundos

# Recriar application
kubectl apply -f argocd/application.yaml -n argocd

# Forçar sync
kubectl patch app canadasoftware-api -n argocd \
  --type merge \
  -p '{"operation":{"initiatedBy":{"username":"admin"},"sync":{"revision":"main"}}}'
```

## ✅ SOLUÇÃO 3: Verificar se Repositório é Público

O repositório `TecnologiaIntegrada/DotNetTemplateAPI` deve ser **PÚBLICO** ou você precisa configurar credenciais no ArgoCD.

### Se for Privado

1. No ArgoCD, vá em **Settings** → **Repositories**
2. Clique em **"CONNECT REPO"**
3. Configure:
   - **Method**: HTTPS
   - **Repository URL**: `https://github.com/TecnologiaIntegrada/DotNetTemplateAPI.git`
   - **Username**: Seu usuário GitHub
   - **Password**: Token PAT (Personal Access Token)
4. Salve

## ✅ SOLUÇÃO 4: Usar Commit SHA Específico

Mudança temporária para garantir:

```yaml
source:
  targetRevision: 6c354d4  # ← Último commit conhecido
```

## 📋 CHECKLIST DE VERIFICAÇÃO

### No GitHub

- [ ] Acessar: https://github.com/TecnologiaIntegrada/DotNetTemplateAPI
- [ ] Verificar se repositório é público ou privado
- [ ] Verificar se diretório `charts/` está visível
- [ ] Verificar branch padrão (deve ser `main`)

### No ArgoCD

- [ ] Application existe?
- [ ] URL do repositório está correta?
- [ ] Branch `main` existe?
- [ ] Hard Refresh foi feito?
- [ ] Credenciais configuradas (se repositório privado)?

## 🎯 AÇÃO IMEDIATA

Execute AGORA no ArgoCD:

1. **HARD REFRESH** (botão de 3 pontos → Hard Refresh)
2. Aguarde 30-60 segundos
3. **SYNC** manual
4. Verifique logs da aplicação

## 🔧 Se Nada Funcionar

Última opção: Recriar tudo do zero

```bash
# 1. Deletar application antiga
kubectl delete application canadasoftware-api -n argocd

# 2. Aguardar limpar
kubectl get application canadasoftware-api -n argocd
# (deve dar erro: not found)

# 3. Aplicar nova
kubectl apply -f argocd/application.yaml -n argocd

# 4. Ver logs
kubectl logs -f -n argocd deployment/argocd-repo-server
```

---

**Status**: Correção aplicada no `application.yaml` (HEAD → main)  
**Próximo passo**: HARD REFRESH no ArgoCD

