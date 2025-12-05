#!/bin/bash
echo "=== HIZLI DURUM KONTROLÜ ==="
echo ""

for level in 01-Beginner 02-Intermediate 03-Advanced 04-Expert 05-RealWorld 06-CuttingEdge 07-CloudNative 08-Capstone 98-RealWorld-Problems 99-Exercises; do
    if [ -d "samples/$level" ]; then
        count=$(find "samples/$level" -maxdepth 1 -type d ! -name ".*" ! -name "$level" | wc -l)
        echo "$level: $count proje"
        echo "  İlk 3 örnek:"
        find "samples/$level" -maxdepth 1 -type d ! -name ".*" ! -name "$level" | head -3 | while read dir; do
            echo "    - $(basename "$dir")"
        done
        echo ""
    else
        echo "$level: KLASÖR YOK!"
        echo ""
    fi
done

# Toplam kontrol
total=$(find samples -mindepth 2 -maxdepth 2 -type d | wc -l)
echo "================================================"
echo "Toplam sample proje klasörü: $total"
echo "================================================"
