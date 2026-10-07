# madake1：地图冒险模组原型

这是 madake 的 Terraria / tModLoader 个人项目，目标是制作类似红云物语的固定地图冒险：玩家依靠武器和道具战斗，不能随意挖掘地图；击败怪物获得原石，用于抽卡获取装备。目前处于原型和整理阶段，尚不是可发布的完整地图模组。

## 当前状态

| 功能 | 当前代码行为 |
| --- | --- |
| 原石掉落 | 非友好、最大生命值大于 5 的非 Boss 掉落 1～5 个；Boss 掉落 100～300 个 |
| 抽卡消耗 | 160 原石在工作台合成 1 金球；单抽消耗 1 金球，十连消耗 10 金球 |
| 卡池 | JSON 配置 3 台机器的位置、半径和四档奖励；有定轨、保底计数和玩家存档 |
| 界面 | 单抽、十连、大奖选择、拖动面板、离开机器后关闭 |
| 地图保护 | 仅种子 `20260111` 的世界启用已有方块/墙体限制；不等于完整的地图防破坏方案 |
| 测试内容 | 几把武器、原石、金球、扭蛋机；商人仍出售金球，武器仍有测试配方 |
| 地图资源 | 仓库没有 `.wld` / `.twld` 地图文件；种子本身不会生成设计好的冒险地图 |
| 联机 | 保留旧原型协议，客户端决定抽卡结果，尚不具备正式联机所需的服务器校验 |

`build.txt` 中的 `version = 0.1` 是模组自身版本，不是 Terraria 或 tModLoader 版本。内部标识为 `madake1`，显示名暂为 `mymod1`；后续改名需同时考虑资源路径、本地化键和旧存档。

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

当前工作区缺少上级 `tModLoader.targets`，执行环境也没有找到 `dotnet`，本轮没有完成编译或游戏运行测试。不要把旁边源码仓库里的 `tModLoader.targets` 直接接过来：它是 tModLoader 自身开发专用入口。

## 接下来做什么

优先恢复 stable 构建，再处理卡池校验和抽卡事务；随后完善地图标识/保护，最后才扩展联机、关卡和武器数量。具体顺序与验收条件见 [开发计划](docs/ROADMAP.md)。

增加功能前先看 [架构约定](docs/ARCHITECTURE.md)。这次完成了目录整理和第一步职责拆分；旧的联机信任、概率计算、配置容错仍有明确技术债，不能把“换了文件夹”理解为这些问题已经修好。

## 参考与素材

红云物语用于研究地图冒险的组织方式，ExampleMod 用于学习 tModLoader API。代码和素材若来自第三方，应逐项确认许可、来源与署名要求；本仓库尚未声明统一开源许可，不自动沿用参考项目的许可。
