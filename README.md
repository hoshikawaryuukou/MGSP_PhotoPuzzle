# MGSP PhotoPuzzle

## 🎮 遊戲介紹

### 遊戲目標

將所有被打亂順序的拼圖碎片，透過**交換位置**的方式，在固定的盤面上還原成一幅完整的原始圖像。

### 🎯 遊戲特色

- **多種難度**：支援 3×3 到 7×7 的盤面大小選擇
- **隨機圖片**：整合 Lorem Picsum API，提供豐富的圖片資源
- **即時預覽**：隨時查看原圖提示，幫助完成挑戰

### 🕹️ 遊戲玩法

1. **設定挑戰**：選擇盤面大小，系統自動載入隨機圖片
2. **交換碎片**：點選第一塊拼圖碎片，再點選第二塊，兩者位置即會交換
3. **完成挑戰**：持續交換直到所有碎片歸位，還原完整圖像即獲勝！

## 專案架構設計
- [MGSU_DesignGuide_Fundamental_Client](https://github.com/hoshikawaryuukou/MGSU_DesignGuide_Fundamental_Client)

### 外部服務
- **[Lorem Picsum](https://picsum.photos/)**：隨機圖片 API 服務

## FAQ

### GameSetupModalPresenter 被其他 Presenter 調用這樣好嗎？

相對可以接受，因為 `GameSetupModalPresenter` 被設計成選擇型 Dialog，需要告知呼叫端結果，且依賴方向相對單純。

**設計考量**：
- **Modal 特性**：作為模態對話框，本身就是被動等待用戶選擇的元件
- **結果回傳**：需要將用戶的選擇（盤面大小）回傳給呼叫者
- **單純依賴**：只有單向的調用關係，不會形成循環依賴
- **職責明確**：專注於遊戲設定的 UI 邏輯，不包含複雜的業務邏輯

**替代方案**：
如果要更嚴格地遵循 MVP 原則，可以考慮：
- 將 Presenter 間的通訊提到更高層的 Flow Controller

但在輕量級專案中，直接調用是可接受的務實選擇。
