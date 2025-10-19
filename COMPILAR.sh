#!/bin/bash

echo "╔════════════════════════════════════════════════════════════════════╗"
echo "║                                                                    ║"
echo "║         🔨 COMPILANDO APLICAÇÃO - CanadaSoftware.ApiDotNet        ║"
echo "║                                                                    ║"
echo "╚════════════════════════════════════════════════════════════════════╝"
echo ""

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado!"
    echo ""
    echo "Instale o Docker primeiro:"
    echo "  curl -fsSL https://get.docker.com | sh"
    exit 1
fi

echo "✅ Docker encontrado"
echo ""

# Compilar usando Docker
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 📦 Etapa 1: Restore (Baixar Dependências)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

docker build --target build \
  --build-arg BUILDKIT_INLINE_CACHE=1 \
  -t canadasoftware-build:temp \
  -f Dockerfile.build . 2>&1 | tee build.log

BUILD_EXIT_CODE=${PIPESTATUS[0]}

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 📊 RESULTADO DA COMPILAÇÃO"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    echo "✅ COMPILAÇÃO CONCLUÍDA COM SUCESSO!"
    echo ""
    echo "Imagem criada: canadasoftware-build:temp"
    echo ""
    echo "Log completo salvo em: build.log"
else
    echo "❌ COMPILAÇÃO FALHOU!"
    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo " 🔍 ERROS ENCONTRADOS:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo ""
    grep -i "error" build.log | head -30
    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo ""
    echo "Log completo salvo em: build.log"
    echo ""
    echo "Para ver todos os erros:"
    echo "  cat build.log | grep -i error"
    exit 1
fi

echo ""
echo "════════════════════════════════════════════════════════════════════"
echo " 🎯 Próximos Passos:"
echo "════════════════════════════════════════════════════════════════════"
echo ""
echo "1. Executar localmente:"
echo "   docker run -p 8080:80 canadasoftware-build:temp"
echo ""
echo "2. Testar:"
echo "   curl http://localhost:8080/healthz"
echo ""
echo "3. Build imagem final:"
echo "   docker build -t canadasoftware-api:latest ."
echo ""
echo "════════════════════════════════════════════════════════════════════"

