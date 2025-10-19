# 🏦 CanadaSoftware.ApiDotNet

[![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-316192?logo=postgresql)](https://www.postgresql.org/)
[![Kafka](https://img.shields.io/badge/Kafka-3.0-231F20?logo=apache-kafka)](https://kafka.apache.org/)
[![Kubernetes](https://img.shields.io/badge/Kubernetes-1.27-326CE5?logo=kubernetes)](https://kubernetes.io/)

Microserviço .NET para gestão de clientes e operações bancárias com arquitetura hexagonal, DDD, CQRS e event-driven.

---

## 🚀 Funcionalidades

- ✅ **CRUD Completo de Clientes** com validações
- ✅ **Eventos Kafka** para integração assíncrona
- ✅ **PostgreSQL** como banco de dados
- ✅ **SEQ** para logs centralizados
- ✅ **Swagger/OpenAPI** para documentação
- ✅ **Health Checks** para monitoramento
- ✅ **Polly** para resiliência (retry, circuit breaker)

---

## 🏗️ Arquitetura

### Princípios
- **SOLID** - Responsabilidade única, inversão de dependências
- **DDD** - Domain-Driven Design
- **CQRS** - Separação de comandos e queries
- **Hexagonal** - Portas e adaptadores

### Camadas

```
CanadaSoftware.ApiDotNet/
├── ServiceHost         # API REST, Middlewares
├── Application         # Use Cases, Handlers
├── Domain              # Entidades, Regras de Negócio
├── EntityFramework     # Infraestrutura, Repositórios
├── Common              # Utilitários compartilhados
├── Services            # Serviços de negócio
├── HttpClient          # Integrações externas
├── Globalization       # Internacionalização
├── Application.Ecst    # Event sourcing
└── Application.Tests   # Testes unitários
```

---

## 📦 Pré-requisitos

- **.NET SDK 7.0+**
- **Docker** + **Kubernetes**
- **Helm 3.0+**
- **kubectl** configurado

---

## 🔧 Instalação

### 1. Clonar Repositório

```bash
git clone <repository-url>
cd CanadaSoftware.ApiDotNet
```

### 2. Restaurar Dependências

```bash
dotnet restore src/CanadaSoftware.ApiDotNet.sln
```

### 3. Executar Migrations

```bash
cd src/CanadaSoftware.ApiDotNet.ServiceHost
dotnet ef database update
```

### 4. Executar Aplicação

```bash
dotnet run
```

Acesse:
- **Swagger**: http://localhost:5000/swagger
- **Health**: http://localhost:5000/healthz

---

## ☸️ Deploy no Kubernetes

### Passo 1: Criar Secrets

```bash
kubectl create secret generic app-secrets \
  --from-literal=CONNECTIONSTRINGS__DEFAULT="Host=postgresql-service.dev.svc.cluster.local;Port=5432;Database=appdatabase;Username=postgres;Password=SuaSenhaSegura!" \
  --from-literal=CONNECTIONSTRINGS__KAFKA="broker:9092" \
  -n dev
```

### Passo 2: Deploy com Helm

```bash
helm install api-service ./charts \
  -f ./charts/values.yaml \
  -n dev \
  --create-namespace
```

### Passo 3: Verificar

```bash
kubectl get pods -n dev
kubectl logs -f deployment/api-deployment -n dev
```

### Passo 4: Acessar

```bash
kubectl port-forward svc/api-service 8080:80 -n dev
# http://localhost:8080/swagger
```

---

## 🌐 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/v1/clientes` | Criar cliente |
| GET | `/v1/clientes/{id}` | Buscar por ID |
| GET | `/v1/clientes/cpf/{cpf}` | Buscar por CPF |
| PUT | `/v1/clientes/{id}` | Atualizar cliente |
| GET | `/healthz` | Health check |

---

## 🧪 Testes

```bash
cd src/CanadaSoftware.ApiDotNet.Application.Tests
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true
```

**Total**: 14 testes unitários

---

## 📊 Observabilidade

### Logs (SEQ)
```bash
kubectl port-forward svc/seq-service 5341:80 -n dev
# http://localhost:5341
```

### CAP Dashboard
```bash
kubectl port-forward svc/api-service 8080:80 -n dev
# http://localhost:8080/cap
```

---

## 🔐 Secrets

**Usar Kubernetes Secrets** (nativo, sem dependências externas):

```bash
kubectl create secret generic app-secrets \
  --from-literal=CONNECTIONSTRINGS__DEFAULT="..." \
  --from-literal=CONNECTIONSTRINGS__KAFKA="..." \
  -n dev
```

**Este projeto USA APENAS Kubernetes Secrets** (nativo, sem dependências externas como AWS).

---

## 📚 Documentação

- `docs/ARQUITETURA.md` - Arquitetura detalhada
- `docs/DEPLOY.md` - Guia de deploy
- `docs/KAFKA_EVENTS.md` - Eventos Kafka

---

## 📄 Licença

© Canada Software. Todos os direitos reservados.

---

**Versão**: 1.0.0  
**Status**: ✅ Pronto para Produção

