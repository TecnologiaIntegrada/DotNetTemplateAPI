#!/bin/bash

echo "╔════════════════════════════════════════════════════════════════════╗"
echo "║                                                                    ║"
echo "║         🔍 ANÁLISE DE ERROS - CanadaSoftware.ApiDotNet            ║"
echo "║                                                                    ║"
echo "╚════════════════════════════════════════════════════════════════════╝"
echo ""

if [ ! -f "build.log" ]; then
    echo "❌ Arquivo build.log não encontrado!"
    echo ""
    echo "Execute primeiro: ./COMPILAR.sh"
    exit 1
fi

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 📊 RESUMO DOS ERROS"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Contar erros
TOTAL_ERRORS=$(grep -c "error CS" build.log)
TOTAL_WARNINGS=$(grep -c "warning CS" build.log)

echo "Total de ERROS: $TOTAL_ERRORS"
echo "Total de WARNINGS: $TOTAL_WARNINGS"
echo ""

if [ $TOTAL_ERRORS -eq 0 ]; then
    echo "✅ Nenhum erro encontrado!"
    exit 0
fi

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 🔴 ERROS POR TIPO"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Agrupar erros por código
grep "error CS" build.log | sed 's/.*error \(CS[0-9]*\).*/\1/' | sort | uniq -c | sort -rn

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 📝 DETALHES DOS ERROS (primeiros 50)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

grep "error CS" build.log | head -50

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo " 💡 SALVAR ANÁLISE COMPLETA"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Criar relatório detalhado
{
    echo "RELATÓRIO DE ERROS - $(date)"
    echo "================================"
    echo ""
    echo "RESUMO:"
    echo "  Erros: $TOTAL_ERRORS"
    echo "  Warnings: $TOTAL_WARNINGS"
    echo ""
    echo "ERROS POR TIPO:"
    grep "error CS" build.log | sed 's/.*error \(CS[0-9]*\).*/\1/' | sort | uniq -c | sort -rn
    echo ""
    echo "LISTA COMPLETA DE ERROS:"
    echo "================================"
    grep "error CS" build.log
} > erros-detalhados.txt

echo "✅ Relatório salvo em: erros-detalhados.txt"
echo ""
echo "Para ver o relatório:"
echo "  cat erros-detalhados.txt"

