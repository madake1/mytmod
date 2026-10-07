# 版本检查与升级路线

核对日期：2026-10-07。以下是检查时的快照，恢复开发时应重新查看官方发行页。

## 检查结果

| 对象 | 实际观察 | 意义 |
| --- | --- | --- |
| 本模组 | `build.txt` 为 0.1；csproj 未固定 Terraria 版本 | 不能从 0.1 推导游戏版本 |
| 构建导入 | 原路径 `../tModLoader.targets` 不存在 | 当前布局无法直接解析原构建入口 |
| 本地参考源码 | `tModLoader` 的 `1.4.5` 分支，`a9fd75022c` | 很新，但与本模组的 1.4.4 基线不一致 |
| 官方 stable | v2026.08.3.0，发行页标注 1.4.4 | 当前推荐开发基线 |
| 官方 preview | v2026.09.2.1，发行页仍标注 1.4.4 | preview 不等于 1.4.5 |
| 本地开发工具 | PATH 找不到 dotnet，常规 Program Files 路径也未找到 | 尚不能执行编译；不等于已扫描所有自定义安装位置 |
| Steam 运行环境 | 本轮未检查 | 不能把旁边源码的版本当成游戏安装版本 |

来源：[官方发行记录](https://github.com/tModLoader/tModLoader/releases)、[开发进度](https://github.com/tModLoader/tModLoader/issues/5070)。

## 现在应调整什么

- 以 stable 版的 tMLMod.targets、程序集和 ExampleMod 为一套匹配的开发依赖；当前旁边的 ExampleMod 只能作为概念参考。
- 安装 .NET 8 SDK，恢复标准 ModSources 布局或配置本机 `Build.Local.props`；框架版本由匹配的官方 targets 提供，不在项目里盲目写死。
- 保留 `madake1` 标识，先记录一份成功运行的 tModLoader 版本和 Git 提交。此次未升级 build.txt 的模组版本，也未切换任何参考仓库分支。
- 拆分“目录整理”和“API 迁移”的后续提交，便于出问题时独立定位。

本地路径配置示例在根目录 `Build.Local.props.example`。复制后的 `Build.Local.props` 已加入忽略列表，也可以通过 MSBuild 的 `-p:TmlTargetsPath=...` 参数提供路径。不要把本机 Steam 路径提交给其他开发者。

## 未来迁移到 1.4.5

等官方迁移指引和发布阶段适合模组开发时，在独立开发分支进行：

1. 备份角色/地图，并保留能运行的 1.4.4 版本。
2. 根据官方指南准备 .NET 10 SDK；Visual Studio 用户需要 VS 2026，VS 2022 不再适用。
3. 切换到匹配版本的运行时、targets 和示例，先运行 tModPorter，再检查无法自动迁移的部分。
4. 优先检查 `Common/Gacha/GachaEffects.cs` 中的 `Item.NewItem`：1.4.5 调整了地面物品模型 `WorldItem` 及生成 API。检查所有 ModItem / GlobalTile / GlobalWall / GlobalNPC 钩子签名。
5. 编译通过后，依次验证资源、本地化、旧玩家保底存档、掉落、界面、地图保护与联机。

工具和 API 变化来源：[官方迁移指南草稿](https://github.com/tModLoader/tModLoader/blob/1.4.5/MigrationGuide_1.4.5.md)。它仍会改变，迁移不能只把版本号或 TargetFramework 改成新值。
