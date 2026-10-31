# madake1：地图冒险模组原型

这是 madake 的 Terraria / tModLoader 个人项目，目标是制作类似红云物语的固定地图冒险：玩家依靠武器和道具战斗，不能随意挖掘地图；击败怪物获得原石，用于抽卡获取装备。目前处于原型和整理阶段，尚不是可发布的完整地图模组。

## 当前状态

| 功能 | 当前代码行为 |
| --- | --- |
| 原石掉落 | 非友好、最大生命值大于 5 的非 Boss 掉落 1～5 个；Boss 掉落 100～300 个 |
| 抽卡消耗 | 160 原石在工作台合成 1 金球；单抽消耗 1 金球，十连消耗 10 金球 |
| 卡池 | JSON 配置 3 台机器的地图 ID、稳定机器 ID、地点名称、位置、半径和四档奖励；有定轨、保底计数和玩家存档 |
| 界面 | 单抽、十连、大奖选择、拖动面板、离开机器后关闭 |
| 地图保护 | 仅带 `madake-adventure` 地图标记的世界启用地形锁；功能饰品栏装备作者身份证可在单人模式编辑 |
| 测试内容 | 几把武器、原石、金球、扭蛋机；商人仍出售金球，武器仍有测试配方 |
| 地图资源 | `Maps/Madake_Adventure.wld` 是可单文件分发的大世界底图；出生点已有三处扭蛋机站点，机器由模组在加载时放置 |
| 联机 | 保留旧原型协议，客户端决定抽卡结果，尚不具备正式联机所需的服务器校验 |

`build.txt` 中的 `version = 0.1` 是模组自身版本，不是 Terraria 或 tModLoader 版本。内部标识为 `madake1`，显示名暂为 `mymod1`；后续改名需同时考虑资源路径、本地化键和旧存档。

## 冒险地图与作者身份证

- 只需将 `Maps/Madake_Adventure.wld` 放入 tModLoader 的 `Worlds` 目录。当前底图为 8400 格宽的大世界；模组根据 `.wld` 中的世界 GUID 和世界 ID 识别它为 `madake-adventure` v1，不依赖种子或 `.twld`。普通世界不会锁地形。玩家保存后，tModLoader 仍可能在本地生成 `.twld`；这不影响单文件分发。
- 冒险地图中，普通玩家不能挖、放、替换方块，不能拆、放墙，也不能用爆炸破坏地形。机关、门、箱子等原版交互仍需逐项验证；液体和特殊地形改造道具尚未完整封锁。
- 作者身份证没有配方、掉落、商店出售或初始赠送。用 HEROs Mod 的物品工具取出，装备在**功能饰品栏**后，可在单人模式编辑地图；放回背包、饰品外观栏或取下后立即恢复地形锁。当前图标暂用原版工具腰带贴图。其他作弊工具也可能生成无配方物品，身份证不是多人服务器的管理员认证。
- 新建其他大世界时，作者戴着身份证在单人模式输入 `/madakemap mark` 才会将其标记为冒险地图；`/madakemap status` 可查看当前状态。服务器控制台也可执行 `madakemap mark`。其他地图仍通过 `.twld` 保存标记，分发时需同时保留 `.wld`、`.twld`。联机服务器上的作者编辑目前关闭。
- 底图在出生点 `(4201, 451)` 格附近包含原版木地面、上层平台和踏板；左、右站点距出生点水平各 96 像素。`.wld` 本身不含模组方块，原版游戏只显示地形；tModLoader 加载内置地图时，按 `Data/Gacha/GachaConfig.json` 的 `placeOnWorldLoad` 和坐标生成三台扭蛋机。玩家保存后，本地可能生成 `.twld`。
- 三台机器目前命名为“出生点·西侧／东侧／上层扭蛋机”。调整位置或 `displayName` 时保留 `machineId`，玩家保底记录才会延续。已经保存过模组方块的玩家世界若移动机器，还需清理旧位置的方块；新发布的干净 `.wld` 不受影响。

## 版本选择

2026-10-07 核对：官方最新稳定发行页列出 **v2026.08.3.0，基于 Terraria 1.4.4**；1.4.5 仍处于开发迁移阶段。因此现阶段建议先以最新 1.4.4 stable 恢复构建和玩法测试，不必为了 Terraria 本体已经发布 1.4.5 就马上迁移。

旁边的 `../tModLoader` 实际是 `1.4.5` 分支，提交 `a9fd75022c`（2026-10-07），它的 ExampleMod 也面向开发中的 API。它不是本项目已经适配 1.4.5 的证明。本文未确认电脑上 Steam 安装的 tModLoader 运行版本。

官方来源：[稳定发行记录](https://github.com/tModLoader/tModLoader/releases)、[1.4.5 进度与 FAQ](https://github.com/tModLoader/tModLoader/issues/5070)、[迁移指南草稿](https://github.com/tModLoader/tModLoader/blob/1.4.5/MigrationGuide_1.4.5.md)。详细记录见 [版本与构建](docs/VERSION_AND_BUILD.md)。

## 目录导航

```text
mytmod/
├── madake1.cs                 # Mod 入口，仅分发网络包
├── madake1.csproj / .sln      # 构建和 IDE 入口
├── Common/
│   ├── Gacha/                # 抽卡流程、算法、配置、玩家存档、奖励效果
│   ├── Networking/           # 协议编号、发送适配、接收处理
│   ├── WorldRules/           # 世界保护钩子
│   ├── UI/                   # 打开界面时的输入/方块限制
│   ├── Loot/                 # 全局怪物掉落
│   └── Shops/                # 原版 NPC 商店扩展
├── Content/
│   ├── Items/                # 物品、武器和贴图
│   │   └── Tiles/            # 现有扭蛋机方块/放置物品及贴图
│   └── UI/                   # 抽卡界面、控件和界面贴图
├── Data/Gacha/GachaConfig.json
├── Maps/                    # 可单文件分发的大世界底图
├── Localization/             # 中英文本地化
├── Properties/               # 调试配置
└── docs/                     # 架构、版本、后续开发和验收计划
```

父目录只是包含三个项目的工作区；`mytmod` 是独立 Git 仓库。`../tsorcRevamp`、`../tModLoader` 是只读学习参考，没有作为本模组的运行依赖，也不应把它们的代码整体复制进来。

## 恢复开发

1. 使用 Steam 的 tModLoader stable，按[官方入门指南](https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide)安装 .NET 8 SDK。
2. 通过游戏的 Workshop → Develop Mods → Open Sources 找到实际的 `ModSources`。
3. 推荐把本项目内容作为 `ModSources/madake1` 源码目录使用，保留 `madake1.csproj` 和内部标识。不要把外层三个项目一同复制进去；也不要以 `mytmod` 作为发布时的新模组标识。
4. 标准布局使用游戏生成的上级 `tModLoader.targets`。留在当前工作区使用 IDE 时，可复制 `Build.Local.props.example` 为 `Build.Local.props`，把路径改成已安装 stable 的 `tMLMod.targets`。这是编译引用入口配置，不能代替游戏的源码发现和正确命名目录。
5. 在正确的源码目录打开 `madake1.sln`，构建后在游戏中 Build + Reload，用专门的测试角色和测试地图验证。

2026-10-31 已在本机 tModLoader v2026.08.3.0 和 .NET 8 SDK 8.0.427 上编译通过，并用独立服务器生成、标记、重载大世界。旁边源码仓库里的 `tModLoader.targets` 是 tModLoader 自身开发专用入口，不用于本模组构建。

## 接下来做什么

优先恢复 stable 构建，再处理卡池校验和抽卡事务；随后完善地图标识/保护，最后才扩展联机、关卡和武器数量。具体顺序与验收条件见 [开发计划](docs/ROADMAP.md)。

增加功能前先看 [架构约定](docs/ARCHITECTURE.md)。这次完成了目录整理和第一步职责拆分；旧的联机信任、概率计算、配置容错仍有明确技术债，不能把“换了文件夹”理解为这些问题已经修好。

## 参考与素材

红云物语用于研究地图冒险的组织方式，ExampleMod 用于学习 tModLoader API。代码和素材若来自第三方，应逐项确认许可、来源与署名要求；本仓库尚未声明统一开源许可，不自动沿用参考项目的许可。
