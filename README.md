# MGSP_PhotoPuzzle

本遊戲是一個拼圖遊戲 
玩家會先在 Gallery 中選擇圖片，並到 Game 中遊玩

使用 unity
插件包括 
1. unitask
2. messagepipe
3. unirx
4. vcontainer

分成三個模組
1. App: 初始化與其他模組之間的路由
2. Gallery: 從圖片api 隨機拉取圖片，讓玩家選擇圖片
3. Game: 將選擇的圖片作為拼圖遊玩

分成四個上下文
1. Root
2. App (parent is Root)
3. Gallery (parent is App)
4. Game (parent is App)

我想要一些狀態管理與路由的思路