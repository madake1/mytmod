# 架构约定

采用一个模组项目，按功能分目录。目前规模不需要多程序集、依赖注入容器或通用框架。

## 当前边界

| 模块 | 负责 | 新增代码不应承担 |
| --- | --- | --- |
| `Content/UI` | 渲染、交互、结果展示 | 直接扣货币、生成掉落、拼接网络包 |
| `Common/Gacha/GachaWorkflow` | 一次抽卡的流程入口 | 控件绘制、网络包字段解析 |
| `GachaService` | 抽卡品质、奖池选择、更新计数 | UI、网络发送 |
| `GachaSystem` | 加载地图机器配置、位置查询 | 概率判定 |
| `GachaPlayer` / `GachaMachineData` | 玩家状态与存档适配 / 每台机器数据 | UI 和掉落生成 |
| `Common/Networking` | 协议、发送、接收适配 | 把客户端声明当作服务器事实 |
| `Common/WorldRules` | 世界限制钩子 | 依赖抽卡 UI 才启用冒险保护 |
| `Common/UI` | 界面打开时的暂时交互限制 | 作为世界保护的唯一实现 |
| `Common/Loot` / `Shops` | 怪物掉落 / 商店规则 | 承担抽卡与存档逻辑 |
| `Content/Items` | 物品属性、使用行为和贴图 | 承担完整游戏流程 |
| `Data` | 地图坐标与卡池数据 | C# 业务逻辑 |

已拆开 Mod 入口和包处理、界面和扣费流程、协议枚举和发送器、算法和结果模型、玩家存档适配和数据、扭蛋机方块和放置物品。已移除 UI 中写入 `.cursor/debug.log` 的临时诊断代码。

现有调用链是 UI → GachaWorkflow → GachaService → GachaRequest → 奖励效果；这是保留原玩法的过渡结构。GachaService 仍依赖 Terraria.Player、Main.rand；GachaEffects 仍同时处理生成物品和出金通知。后续应在真正需要单元测试/联机时，把随机源、奖励发放、显示通知逐步抽成明确边界，避免一次造出大量空接口。

## 兼容性约束

- 内部模组名 `madake1`、内容类名、本地化键和存档字段保持不变。移动 C# 类时同步检查自动加载资源路径。
- 现有物品/方块贴图保留在原路径。`Content/Items/Tiles` 是有意保留的兼容路径；将来迁移到 `Content/Tiles` 应同步迁移贴图或显式覆盖 Texture。
- JSON 已从 `Sets/GachaConfig.json` 移到 `Data/Gacha/GachaConfig.json`，加载入口同步更新。
- 机器用配置中的稳定 `machineId` 作为存档键，`mapId` 限定所属地图，`displayName` 是可修改的地点名称。`placeOnWorldLoad` 的内置地图机器在 `PostWorldLoad` 放置，因此交付 `.wld` 只含原版地形。移动机器或改名时保留 `machineId`；已游玩世界的旧机器方块需另行清理。
- 网络编号保留 0/1/2；未新增服务器权威抽卡协议，不声称兼容性或联机安全已经经过游戏验证。

## 增加功能的方法

新增武器放入对应 `Content/Items/Weapons` 分类，贴图同目录，名称/说明进入 Localization。新增卡池首先修改 Data；目前自定义奖励短名仍需登记到 ModItemMap。新增全局规则放入 Common 下对应功能目录，不再恢复 Sets、utils、tools 这种混合职责目录。

地图文件未来放 `Maps/`，配套地图 ID、格式版本、发行说明和恢复方式；关卡触发规则未来按需要增加 `Common/Progression`。这两个目录尚未创建，避免用空目录伪装已实现功能。

JSON 在加载阶段应最终校验物品存在、数量合法、蓝池非空、坐标/半径合法、机器 ID 唯一。当前加载器尚未达到该要求，详见开发计划。
