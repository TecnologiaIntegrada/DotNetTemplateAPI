#!/bin/bash

# Script para fazer push para o repositório GitHub
# Repository: https://github.com/TecnologiaIntegrada/DotNetTemplateAPI

echo "════════════════════════════════════════════════════"
echo "  Push para GitHub TecnologiaIntegrada/DotNetTemplateAPI"
echo "════════════════════════════════════════════════════"
echo ""

# Configurar remote
echo "[1/3] Configurando remote do GitHub..."
git remote remove origin 2>/dev/null || true
git remote add origin https://github.com/TecnologiaIntegrada/DotNetTemplateAPI.git

echo "✅ Remote configurado"
echo ""

# Verificar branch
echo "[2/3] Verificando branch..."
CURRENT_BRANCH=$(git branch --show-current)
echo "Branch atual: $CURRENT_BRANCH"

if [ "$CURRENT_BRANCH" != "main" ] && [ "$CURRENT_BRANCH" != "master" ]; then
    echo "Renomeando branch para main..."
    git branch -M main
fi

echo "✅ Branch configurado"
echo ""

# Push
echo "[3/3] Fazendo push para GitHub..."
echo ""
echo "⚠️  ATENÇÃO: Isto irá SUBSTITUIR todo o código do repositório!"
echo ""
echo "Para prosseguir, execute manualmente:"
echo ""
echo "  git push -u origin main --force"
echo ""
echo "NOTA: Use --force apenas se tiver certeza!"
echo "      Isso irá substituir o código existente no GitHub"
echo ""

