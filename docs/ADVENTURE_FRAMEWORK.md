# 传送水晶与地图进度

## 作者放置

传送水晶不再从 JSON 坐标生成。单人模式装备作者身份证后输入 `/crystalitem`，领取道具并放在三格连续承重地面上。放好后会产生 `AdventurePylonEntity`，进入原版晶塔地图层。

旧版从 JSON 自动生成的晶塔会在世界首次加载新版模组时清理一次。之后不会再自动清理或生成；需要作者亲自摆放。旧版的 `capital` / `west` 解锁记录不会对应新晶塔，玩家需重新右键解锁。

## 玩家使用

- 未解锁的晶塔显示灰色，不绘制地图图标。右键后只为当前角色解锁，打开全屏地图；已解锁的晶塔在该玩家画面中显示深蓝色，地图上有可点击图标。
- 点击地图图标时，客户端发送晶塔实际图块坐标。服务端核对晶塔仍存在、玩家已解锁、玩家靠近另一座已解锁晶塔，再从目标晶塔附近寻找安全落点。传送不使用任何 JSON 位置。
- 水晶记录按 `角色 × 地图副本 × 晶塔左上角坐标` 保存。移动或重新放置晶塔会形成新站点，需要重新解锁；旧地点不会继续作为传送落点。
- 世界进度范围由 `AdventureWorldSystem.ProgressScope` 区分，同一角色进入两个地图副本时互不共享。

## 主要文件

- `Common/Adventure/AdventurePylonItem.cs`：可手动放置的道具。
- `Common/Adventure/CrystalItemCommand.cs`：作者领取命令。
- `Common/Adventure/AdventurePylonTile.cs`：3×4 建筑、灰蓝贴图切换、右键和地图图标。
- `Common/Adventure/CrystalWorldSystem.cs`：实际晶塔查找、安全落点与一次性旧晶塔迁移。
- `Common/Adventure/AdventurePackets.cs`：解锁、传送的服务端验证和联机同步。
- `Common/Adventure/AdventurePlayer.cs`：按地图副本保存个人解锁状态；任务进度字段保留作后续设计。

贴图由一张原创水晶图转换为符合 Terraria 图块尺寸的像素素材；先前复制的 ExampleMod 晶体动画素材已移出模组。首次进游戏需检查两种颜色、道具放置、地图图标和落点。
