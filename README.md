# CanadaSoftware.ApiDotNet

API .NET 7.0 com arquitetura hexagonal, DDD, CQRS e integração com Kafka.

---

## 🏗️ Estrutura do Projeto

### Camadas da Aplicação

```
src/
├── CanadaSoftware.ApiDotNet.ServiceHost/        # API REST, Startup, Middlewares
├── CanadaSoftware.ApiDotNet.Application/        # Use Cases, Handlers CQRS, DTOs
├── CanadaSoftware.ApiDotNet.Domain/             # Entidades, Agregados, Value Objects
├── CanadaSoftware.ApiDotNet.EntityFramework/    # Repositórios, DbContext, Migrations
├── CanadaSoftware.ApiDotNet.Common/             # Utilitários compartilhados
├── CanadaSoftware.ApiDotNet.Services/           # Serviços de negócio
├── CanadaSoftware.ApiDotNet.HttpClient/         # Integrações HTTP externas, Polly
├── CanadaSoftware.ApiDotNet.Globalization/      # Internacionalização, recursos
├── CanadaSoftware.ApiDotNet.Application.Ecst/   # Event Sourcing, DomainEvents
└── CanadaSoftware.ApiDotNet.Application.Tests/  # Testes unitários (xUnit, Moq)
```

### Infraestrutura

```
charts/                      # Helm Charts para Kubernetes
├── templates/
│   ├── deployment.yaml      # Deployment da API
│   ├── service.yaml         # Service (ClusterIP)
│   ├── postgresql-statefulset.yaml  # PostgreSQL (independente)
│   └── seq-deployment.yaml  # SEQ logs (independente)
└── values.yaml              # Configurações

argocd/                      # GitOps com ArgoCD
└── application.yaml         # Application manifest

.github/workflows/           # CI/CD com GitHub Actions
└── deploy.yml               # Pipeline de deploy
```

---

## 🛠️ Tecnologias

- **.NET 7.0** - Framework
- **PostgreSQL 15** - Banco de dados
- **Entity Framework Core** - ORM
- **MediatR** - CQRS
- **FluentValidation** - Validações
- **Serilog** + **SEQ** - Logs centralizados
- **Kafka** + **CAP** - Mensageria
- **Polly** - Resiliência (Retry, Circuit Breaker)
- **Swagger** - Documentação API
- **OpenTelemetry** - Observabilidade
- **xUnit** + **Moq** - Testes

---

## 🏛️ Arquitetura

### Princípios
- **SOLID** - Responsabilidade única, inversão de dependências
- **DDD** - Agregados, Value Objects, Repository Pattern
- **CQRS** - Commands e Queries separados
- **Hexagonal** - Portas (interfaces) e Adaptadores (implementações)

### Fluxo de Requisição

```
HTTP Request
    ↓
ServiceHost (API/Controllers)
    ↓
Application (Handlers CQRS)
    ↓
Domain (Entities, Business Rules)
    ↓
EntityFramework (Repository, DbContext)
    ↓
PostgreSQL
```

---

## 🚀 Deploy no Kubernetes

### Pré-requisitos
- Cluster Kubernetes
- kubectl configurado
- Helm 3+
- Namespace criado

### Deploy Rápido

```bash
# 1. Criar secrets
kubectl create secret generic app-secrets \
  --from-literal=CONNECTIONSTRINGS__DEFAULT="Host=postgresql-service.dev.svc.cluster.local;Port=5432;Database=appdatabase;Username=postgres;Password=postgres" \
  -n dev

# 2. Deploy
./DEPLOY.sh dev

# 3. Verificar
kubectl get pods -n dev
```

### Pods Deployados

| Pod | Tipo | Replicas | Descrição |
|-----|------|----------|-----------|
| **api-deployment** | Deployment | 1-10 (HPA) | API principal |
| **postgresql** | StatefulSet | 1 | Banco de dados (isolado, sempre ativo) |
| **seq** | Deployment | 1 | Logs centralizados (isolado, sempre ativo) |

---

## 🔐 Secrets (Kubernetes Nativo)

Criados via `kubectl`:

```bash
kubectl create secret generic app-secrets \
  --from-literal=CONNECTIONSTRINGS__DEFAULT="..." \
  --from-literal=CONNECTIONSTRINGS__KAFKA="..." \
  --from-literal=KAFKA__CLUSTERAPIKEY="..." \
  --from-literal=KAFKA__CLUSTERAPISECRET="..." \
  -n dev
```

Injetados automaticamente nos pods via `deployment.yaml`.

---

## 🌐 Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/healthz` | Health check |
| GET | `/swagger` | Documentação OpenAPI |
| POST | `/v1/clientes` | Criar cliente |
| GET | `/v1/clientes/{id}` | Buscar cliente por ID |
| GET | `/v1/clientes/cpf/{cpf}` | Buscar cliente por CPF |
| PUT | `/v1/clientes/{id}` | Atualizar cliente |

---

## 📡 Eventos Kafka

| Tópico | Evento | Quando |
|--------|--------|--------|
| `cliente.criado` | ClienteCriadoEvent | POST /v1/clientes |
| `cliente.atualizado` | ClienteAtualizadoEvent | PUT /v1/clientes/{id} |
| `cliente.desativado` | ClienteDesativadoEvent | Soft delete |

---

## 🧪 Testes

```bash
cd src/CanadaSoftware.ApiDotNet.Application.Tests
dotnet test
```

**14 testes unitários** (Domain + Handlers)

---

## 📊 CI/CD

### GitHub Actions
- Build automático
- Testes automáticos
- Push para registry
- Atualização automática via ArgoCD

### ArgoCD
- Sync automático do Git
- Deploy em dev/hml/prod
- Rollback fácil
- Health checks integrados

---

## 🗄️ PostgreSQL

### Configuração

- **Tipo**: StatefulSet (dados persistentes)
- **Versão**: PostgreSQL 15 Alpine
- **Storage**: 10Gi (PersistentVolumeClaim)
- **DNS**: `postgresql-service.dev.svc.cluster.local:5432`

### Acesso

```bash
# Port-forward
kubectl port-forward svc/postgresql-service 5432:5432 -n dev

# Conectar
psql -h localhost -U postgres -d appdatabase
```

**Usuário e Senha** são configurados no `values.yaml` e depois via secret do Kubernetes.

---

## 📝 Observabilidade

### SEQ (Logs)
```bash
kubectl port-forward svc/seq-service 5341:80 -n dev
# http://localhost:5341
```

### CAP Dashboard (Kafka)
```bash
kubectl port-forward svc/api-service 8080:80 -n dev
# http://localhost:8080/cap
```

### Swagger
```bash
# http://localhost:8080/swagger
```

---

## 📄 Licença

© 2025 Canada Software. Todos os direitos reservados.

---

**Versão**: 1.0.0  
**Status**: Pronto para Produção  
**Deploy**: Kubernetes com ArgoCD
