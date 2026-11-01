# 传送水晶与地图进度

## 作者放置

传送水晶不再从 JSON 坐标生成。作者可用物品工具取得传送水晶道具，在单人模式装备作者身份证后，将水晶放在三格连续承重地面上。放好后会产生 `AdventurePylonEntity`，进入原版晶塔地图层。

旧版从 JSON 自动生成的晶塔会在世界首次加载新版模组时清理一次。之后不会再自动清理或生成；需要作者亲自摆放。旧版的 `capital` / `west` 解锁记录不会对应新晶塔，玩家需重新右键解锁。

## 玩家使用

- 未解锁的晶塔显示灰色，不绘制地图图标。第一次右键只为当前角色解锁，晶塔变为蓝色；第二次右键已解锁的晶塔才打开全屏地图。解锁态水晶外侧有 12 帧循环的淡蓝光晕，中央水晶上下浮动一像素，地图上有可点击图标。
- 点击地图图标时，客户端发送晶塔实际图块坐标。服务端核对晶塔仍存在、玩家已解锁、玩家靠近另一座已解锁晶塔，再从目标晶塔附近寻找安全落点。传送不使用任何 JSON 位置。
- 水晶记录按 `角色 × 地图副本 × 晶塔左上角坐标 × 本次放置的随机 ID` 保存。挖下晶塔时立即删除在线角色的解锁记录；离线角色下次进入地图时清理失效记录。同一位置重新放置也会得到新 ID，需要重新解锁。旧版只有坐标的记录会在首次进入新版时清理。
- 世界进度范围由 `AdventureWorldSystem.ProgressScope` 区分，同一角色进入两个地图副本时互不共享。

## 主要文件

- `Common/Adventure/AdventurePylonItem.cs`：可手动放置的道具。
- `Common/Adventure/AdventurePylonTile.cs`：3×4 建筑、灰蓝贴图切换、右键和地图图标。
- `Common/Adventure/CrystalWorldSystem.cs`：实际晶塔查找、安全落点与一次性旧晶塔迁移。
- `Common/Adventure/AdventurePackets.cs`：解锁、传送的服务端验证和联机同步。
- `Common/Adventure/AdventurePlayer.cs`：按地图副本保存个人解锁状态；任务进度字段保留作后续设计。

晶塔以作者恢复的上一版贴图为原稿，备份在 `tools/pylon_source/`。`tools/BuildAdventurePylonArt.ps1` 只加厚外缘硬勾边，并把解锁态拆为静止的底座和环、可上下移动的中央水晶、位于水晶外围的渐隐淡蓝光晕。光晕 PNG 的 RGB 已按透明度预乘，适配 Terraria 图块绘制的混合方式，避免低透明像素显示成亮青色硬边。这些图层全部在 `PreDraw` 中绘制。大地图继续使用深蓝色简约水晶图标；鼠标悬停时未解锁显示同形灰色图标，已解锁显示地图同款蓝色图标。放置道具本身使用灰色图标。

光效有两处可调：`tools/BuildAdventurePylonArt.ps1` 的 `GlowStrength` 和椭圆宽高决定可见光晕；修改后要重新运行脚本并打包模组。`Common/Adventure/AdventurePylonTile.cs` 的 `ModifyLight` 中 RGB 基础值和 `pulse` 系数决定晶塔对周围图块的实际照明，修改后只需重新编译模组。游戏已运行时需要重新加载模组才能看到新贴图和代码。
