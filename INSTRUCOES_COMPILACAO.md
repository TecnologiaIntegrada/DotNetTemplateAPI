# 🔨 Instruções de Compilação

## 📋 Pré-requisito

Você precisa do **Docker** instalado:

```bash
# Verificar se Docker está instalado
docker --version

# Se não estiver, instalar:
curl -fsSL https://get.docker.com | sh
```

## 🚀 Compilar a Aplicação

### Método 1: Script Automático (RECOMENDADO)

```bash
cd /projetos/CanadaSoftware.ApiDotNet

# Compilar
./COMPILAR.sh
```

Isso vai:
1. ✅ Baixar dependências NuGet
2. ✅ Restaurar pacotes
3. ✅ Compilar todos os projetos
4. ✅ Rodar testes
5. ✅ Gerar relatório de erros (se houver)

### Método 2: Docker Manual

```bash
cd /projetos/CanadaSoftware.ApiDotNet

# Build
docker build -f Dockerfile.build -t canadasoftware-build:temp .

# Se houver erros, eles aparecerão no terminal
```

## 🔍 Analisar Erros

Se a compilação falhar:

```bash
# Ver análise detalhada
./CORRIGIR_ERROS.sh

# Ver log completo
cat build.log

# Ver apenas erros
cat build.log | grep "error CS"

# Ver erros agrupados
cat erros-detalhados.txt
```

## 🛠️ Tipos Comuns de Erros

### CS0246 - Tipo ou namespace não encontrado

**Causa**: Using incorreto ou referência de projeto faltando

**Solução**:
```csharp
// Mudar de:
using Jazz.Commom;

// Para:
using CanadaSoftware.ApiDotNet.Common;
```

### CS0103 - Nome não existe no contexto

**Causa**: Variável ou classe não declarada

**Solução**: Adicionar using correto ou declarar a variável

### NU1605 - Conflito de versão de pacotes

**Causa**: Versões incompatíveis de pacotes NuGet

**Solução**: Atualizar versões no `.csproj`

## ✅ Compilação Bem-Sucedida

Se tudo funcionar:

```bash
# Testar localmente
docker run -p 8080:80 canadasoftware-build:temp

# Em outro terminal
curl http://localhost:8080/healthz
# Deve retornar: Healthy

curl http://localhost:8080/swagger
# Deve abrir Swagger UI
```

## 📊 Fluxo Completo

```
1. ./COMPILAR.sh
   ↓ (se falhar)
2. ./CORRIGIR_ERROS.sh
   ↓
3. Ver erros-detalhados.txt
   ↓
4. Corrigir erros no código
   ↓
5. git commit && git push
   ↓
6. ./COMPILAR.sh (novamente)
   ↓ (se passar)
7. Deploy no Kubernetes
```

## 🎯 Próximos Passos Após Compilação OK

1. **Testar localmente** (Docker)
2. **Push para GitHub** (CI/CD compila automaticamente)
3. **Deploy no Kubernetes** (ArgoCD)
4. **Obter IP do LoadBalancer**
5. **Configurar no Cloudflare**

## 💡 Dicas

- Sempre compile localmente ANTES de fazer push
- Use `build.log` para ver detalhes
- Erros CS0246 geralmente são namespaces errados
- Limpe antes de compilar: `docker system prune -f`

