#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

declare -A TARGET_COUNTS=(
    ["01-Beginner"]=10
    ["02-Intermediate"]=8
    ["03-Advanced"]=12
    ["04-Expert"]=6
    ["05-RealWorld"]=8
    ["06-CuttingEdge"]=5
    ["07-CloudNative"]=4
    ["08-Capstone"]=1
    ["98-RealWorld-Problems"]=10
    ["99-Exercises"]=15
)

TOTAL_PROJECTS=0
COMPLETE_PROJECTS=0
PARTIAL_PROJECTS=0
MISSING_PROJECTS=0

echo "================================================"
echo "📊 DETAYLI PROJE KONTROL RAPORU"
echo "================================================"
echo ""

for LEVEL in 01-Beginner 02-Intermediate 03-Advanced 04-Expert 05-RealWorld 06-CuttingEdge 07-CloudNative 08-Capstone 98-RealWorld-Problems 99-Exercises; do
    TARGET=${TARGET_COUNTS[$LEVEL]}
    
    if [ ! -d "samples/$LEVEL" ]; then
        echo -e "${RED}❌ $LEVEL: KLASÖR YOK!${NC}"
        continue
    fi
    
    ACTUAL_COUNT=$(find "samples/$LEVEL" -maxdepth 1 -type d ! -name ".*" ! -name "$LEVEL" | wc -l)
    
    echo "================================================"
    echo -e "${YELLOW}📁 $LEVEL${NC}"
    echo -e "   Hedef: $TARGET proje | Bulunan: $ACTUAL_COUNT proje"
    
    if [ $ACTUAL_COUNT -eq $TARGET ]; then
        echo -e "   ${GREEN}✅ Proje sayısı TAMAM!${NC}"
    elif [ $ACTUAL_COUNT -gt $TARGET ]; then
        echo -e "   ${GREEN}📈 Hedeften fazla: +$((ACTUAL_COUNT - TARGET))${NC}"
    else
        echo -e "   ${RED}❌ Eksik: $((TARGET - ACTUAL_COUNT))${NC}"
    fi
    
    LEVEL_COMPLETE=0
    LEVEL_PARTIAL=0
    LEVEL_MISSING=0
    
    for PROJECT_DIR in samples/$LEVEL/*/; do
        if [ -d "$PROJECT_DIR" ]; then
            PROJECT_NAME=$(basename "$PROJECT_DIR")
            TOTAL_PROJECTS=$((TOTAL_PROJECTS + 1))
            
            # Dosya kontrolü
            HAS_README=0
            HAS_CSPROJ=0
            HAS_PROGRAM=0
            
            [ -f "$PROJECT_DIR/README.md" ] && HAS_README=1
            [ $(find "$PROJECT_DIR" -maxdepth 1 -name "*.csproj" | wc -l) -gt 0 ] && HAS_CSPROJ=1
            [ -f "$PROJECT_DIR/Program.cs" ] && HAS_PROGRAM=1
            
            MISSING_COUNT=$((3 - HAS_README - HAS_CSPROJ - HAS_PROGRAM))
            
            if [ $MISSING_COUNT -eq 0 ]; then
                COMPLETE_PROJECTS=$((COMPLETE_PROJECTS + 1))
                LEVEL_COMPLETE=$((LEVEL_COMPLETE + 1))
            elif [ $MISSING_COUNT -eq 1 ]; then
                PARTIAL_PROJECTS=$((PARTIAL_PROJECTS + 1))
                LEVEL_PARTIAL=$((LEVEL_PARTIAL + 1))
            else
                MISSING_PROJECTS=$((MISSING_PROJECTS + 1))
                LEVEL_MISSING=$((LEVEL_MISSING + 1))
            fi
        fi
    done
    
    echo -e "   📊 Durum: ${GREEN}Tam:$LEVEL_COMPLETE${NC} | ${YELLOW}Kısmi:$LEVEL_PARTIAL${NC} | ${RED}Eksik:$LEVEL_MISSING${NC}"
    echo ""
done

echo "================================================"
echo "📈 ÖZET RAPORU"
echo "================================================"

TOTAL_TARGET=0
for count in "${TARGET_COUNTS[@]}"; do
    TOTAL_TARGET=$((TOTAL_TARGET + count))
done

COMPLETION_PCT=$((TOTAL_PROJECTS * 100 / TOTAL_TARGET))
COMPLETE_PCT=$((COMPLETE_PROJECTS * 100 / TOTAL_PROJECTS))

echo "📊 GENEL DURUM:"
echo "   Toplam hedef: $TOTAL_TARGET proje"
echo "   Bulunan: $TOTAL_PROJECTS proje (%$COMPLETION_PCT)"
echo ""
echo "🏗️ KALİTE:"
echo -e "   ${GREEN}✅ Tam: $COMPLETE_PROJECTS (%$COMPLETE_PCT)${NC}"
echo -e "   ${YELLOW}⚠️  Kısmi: $PARTIAL_PROJECTS${NC}"
echo -e "   ${RED}❌ Eksik: $MISSING_PROJECTS${NC}"
echo ""

if [ $TOTAL_PROJECTS -ge $TOTAL_TARGET ] && [ $COMPLETE_PROJECTS -gt $((TOTAL_PROJECTS * 80 / 100)) ]; then
    echo -e "${GREEN}🎉 TEBRIKLER! Projeler %80+ tamamlanmış!${NC}"
else
    echo -e "${YELLOW}⚠️  Bazı projeler eksik veya tamamlanmamış.${NC}"
fi

echo "================================================"
