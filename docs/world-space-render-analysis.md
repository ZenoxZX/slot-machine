# World Space Render Analizi

Bu doküman, slot makinesinin world space'e taşındıktan sonraki render davranışını ve ölçümlerini özetler. Daha önceki UI Canvas dönemi için bkz. [`draw-call-analysis.md`](draw-call-analysis.md).

> **TODO:** Bu dokümandaki ölçümler editor Play Mode'da alındı. Gerçek hedef davranışı yansıtması için tüm ölçümlerin **standalone build alınıp external Frame Debugger ile** tekrar yapılması ve sonuçların buraya eklenmesi gerekiyor. Editor overhead'i (scene view rendering, gizmos, profiler kendi yükü) çıktığında sayılar farklılaşacak.

## Sahne Yapısı

UI Canvas hiyerarşisi sökülüp yerine SpriteRenderer + Transform tabanlı bir world space rig kuruldu. Render edilen ana parçalar:

- **Background container** — slot çerçevesi ve dekoratif arka plan sprite'ları
- **Reel container** — 3 reel × 5 sembol (her sembol için Normal + Blur SpriteRenderer)
- **Coin VFX pool** — 30 coin SpriteRenderer (havuzlanmış)
- **HUD canvas** — sadece SPIN buton + TMP yazısı

Tüm sprite'lar `SA_Main` atlas'ından besleniyor — semboller, arka plan, çerçeve ve coin'ler aynı atlas'ı paylaşıyor.

## Stats Paneli (Worst Case)

Aşağıdaki ölçüm spin sırasında, coin burst aktifken alındı.

![Stats panel](../.github/src/world-space-analysis/stats-panel.png)

| Metrik          | Değer    |
|-----------------|----------|
| Batches         | **34**   |
| Saved by batching | **29** |
| SetPass calls   | **8**    |
| Triangles       | 852      |
| Vertices        | 996      |
| FPS             | 1029.7   |
| CPU main thread | 1.0 ms   |
| Render thread   | 0.5 ms   |

## Frame Debugger — SRP Batcher AÇIK

URP varsayılan ayarı SRP Batcher açık.

![Frame Debugger — SRP Batcher on](../.github/src/world-space-analysis/frame-debugger-srp-on.png)

Pipeline pass'leri:

```
DrawTransparentObjects (5 event)
├── DrawSRPBatcher → SRP Batch
├── RenderLoop.Draw (Draw Mesh + Draw Dynamic + Draw Mesh)
└── DrawSRPBatcher → SRP Batch
DrawScreenSpaceUI (3 event)
├── Draw GL
└── Canvas.RenderOverlays (Draw Mesh × 2)
```

Seçili `SRP Batch` event'inin detayında:
- Draw Calls: 15
- Vertices: 122, Indices: 276
- Shader: `Universal Render Pipeline/2D/Sprite-Unlit-Default`
- Batch cause: *SRP: First call from ScriptableRenderLoopJob*

Bu 15 draw call yalnızca reel sembollerinden değil; reel sembolleri + arka plan / çerçeve parçaları + ekran görüntüsünde aktif olan coin'ler tek SRP batch içinde toplanıyor (hepsi `SA_Main` atlas'ını paylaşıyor). Coin pool'u 30'a kadar büyüyebilir; aktif coin sayısı arttıkça aynı SRP batch'in draw call sayısı da büyür, ek bir batch break yaratmaz.

## Frame Debugger — SRP Batcher KAPALI (Karşılaştırma)

Aynı sahnede SRP Batcher devre dışı bırakıldığında pipeline klasik dynamic batching'e düşüyor.

![Frame Debugger — SRP Batcher off](../.github/src/world-space-analysis/frame-debugger-srp-off.png)

```
DrawTransparentObjects (5 event)
└── RenderLoop.Draw
    ├── Draw Dynamic
    ├── Draw Mesh
    ├── Draw Dynamic
    ├── Draw Mesh
    └── Draw Dynamic
DrawScreenSpaceUI (3 event)
└── ...
```

İki yapılandırmanın özeti:

| Metrik | SRP Batcher AÇIK | SRP Batcher KAPALI |
|--------|:-:|:-:|
| Frame Debugger event | 8 | 8 |
| `DrawTransparentObjects` event | 5 | 5 |
| Transparent pass GPU draw call | ~17 | ~5 |

Projede **SRP Batcher açık** bırakıldı (URP varsayılanı).

## Pipeline Pass'leri

Önceki temizlik adımlarından sonra (bkz. `draw-call-analysis.md` Optimizasyon #4) pipeline minimum durumda:

- ✗ SSAO
- ✗ Skybox
- ✗ CopyDepth
- ✗ CopyColor
- ✗ BlitFinalToBackBuffer
- ✓ DrawTransparentObjects
- ✓ DrawScreenSpaceUI

## Notlar

- **Tüm gameplay sprite'ları aynı SRP batch'de.** Reel sembolleri, arka plan, çerçeve ve coin'ler `SA_Main` atlas'ını paylaştığı için tek bir SRP batch içinde toplanıyor. Coin sayısı arttıkça aynı batch'in draw call sayısı büyür, ek batch break üretmez.
- **TMP "SPIN" yazısı** ayrı font atlası kullandığı için Canvas tarafında ayrı bir batch oluşturuyor; lokalizasyon ve dinamik metin desteği için bilinçli olarak ayrı bırakıldı (bkz. `draw-call-analysis.md` Optimizasyon #3).

## UI Canvas vs World Space — Ölçüm Karşılaştırması

| Metrik | UI Canvas | World Space |
|--------|:-:|:-:|
| Batches (worst case) | 4 | 34 |
| SetPass calls | 4 | 8 |
| CPU main thread | 0.8 ms | 1.0 ms |
| Render thread | 0.3 ms | 0.5 ms |
