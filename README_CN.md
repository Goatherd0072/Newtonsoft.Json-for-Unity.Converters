# Unity Converters for Newtonsoft.Json

[![CircleCI](https://img.shields.io/circleci/build/gh/applejag/Newtonsoft.Json-for-Unity.Converters/master?logo=circleci&style=flat-square)](https://circleci.com/gh/applejag/Newtonsoft.Json-for-Unity.Converters)
[![Codacy grade](https://img.shields.io/codacy/grade/de7041b5f9f9415a8add975d1b8a9fcf?logo=codacy&style=flat-square)](https://www.codacy.com/manual/jilleJr/Newtonsoft.Json-for-Unity.Converters?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=jilleJr/Newtonsoft.Json-for-Unity.Converters&amp;utm_campaign=Badge_Grade)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg?style=flat-square)](/CODE_OF_CONDUCT.md)

[English](./README.md) | [中文](./README_CN.md)

该软件包包含常见 Unity 类型的转换器。例如 **Vector2, Vector3, Matrix4x4, Quaternions, Color, 甚至 ScriptableObject,** *还有更多。*
(请查看 [所有 50+ 种受支持的 Unity 类型的完整兼容性表][doc-compatability-table])

> [!NOTE]
> **这是一个由 Goatherd0072 维护的分支。** 原项目 (jilleJr/applejag) 已停止维护。
> 本分支进行了结构调整以符合 UPM 标准，且仅针对 **Unity 2022+** 版本进行测试和维护。

## 依赖项

### Newtonsoft.Json 包

此包需要项目中存在 `Newtonsoft.Json.dll` 文件。它**不需要**必须与 `jillejr.newtonsoft.json-for-unity` 包一起使用！Unity 生态系统中存在太多的 Newtonsoft.Json 分支和变体。

此包可以与以下任何项结合使用：

- Unity 官方提供的 Newtonsoft.Json 包：
  [`com.unity.nuget.newtonsoft-json`](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html) 
  *(推荐)*

- JamesNK 原始 Newtonsoft.Json 仓库的 jilleJr 分支：
  [jilleJr/Newtonsoft.Json-for-Unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity)

- SaladLab 的分支：[SaladLab/Json.Net.Unity3D](https://github.com/SaladLab/Json.Net.Unity3D)

- ParentElement 的 Asset Store 包：<https://www.parentelement.com/assets/json_net_unity>

- *任何其他来源，只要在 Assets 文件夹中有 `Newtonsoft.Json.dll` 文件，并且声明了基础的 `Newtonsoft.Json` 类型即可。*

### Newtonsoft.Json 版本

没有硬性链接到特定版本。该软件包已通过测试，可以按原样与 Newtonsoft.Json 10.0.3, 11.0.2, 12.0.3 和 13.0.1 一起使用。

此包未在早于 v10.0.3 的 Newtonsoft.Json 版本上进行测试。

## 安装

### 通过 Git URL 安装 (推荐)

这是此纯 UPM 包的推荐安装方法。

1. 打开 Unity Package Manager (Window > Package Manager)
2. 点击左上角的 "+" 按钮
3. 选择 "Add package from git URL..."
4. 输入：`https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git`
5. 点击 "Add"

您也可以通过在 URL 后附加内容来添加特定版本、标签或分支：
```
https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git#v2.0.0
```

### 通过 manifest.json 安装

或者，您可以直接将此包添加到项目的 `Packages/manifest.json` 文件中：

```json
{
  "dependencies": {
    "jillejr.newtonsoft.json-for-unity.converters": "https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git"
  }
}
```

## 它解决了什么问题

很多 Unity 类型会导致自引用循环，例如 Vector3 类型。
在序列化值 `new Vector3(0,1,0)` 时，Newtonsoft.Json 会开始写入：

```json
{
  "x": 0,
  "y": 1,
  "z": 0,
  "normalized": {
    "x": 0,
    "y": 1,
    "z": 0,
    "normalized": {
      "x": 0,
      "y": 1,
      "z": 0,
      "normalized": {
          ...
      }
    }
  }
}
```

*以此类推，直到出现 "recursion" 错误..* 此外，还有一些类型（如 `UnityEngine.RandomState`）隐藏了其状态变量。

此包中的转换器解决了这些问题，以及更多其他问题。

### 示例：不使用此包时的错误

给定以下代码：

```csharp
using UnityEngine;
using Newtonsoft.Json;

public class NewBehaviour : MonoBehaviour {
    void Start() {
        var json = JsonConvert.SerializeObject(transform.position);
        Debug.Log("Position as JSON: " + json);
    }
}
```

将显示以下错误：

```log
JsonSerializationException: Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'normalized'.
...
```

### 示例：使用此包

与上面相同的 `NewBehaviour` 脚本，只需添加此包，您将看到：

```log
Position as JSON: {"x":201.0,"y":219.5,"z":0.0}
UnityEngine.Debug:Log(Object)
Sample:Start() (at Assets/Sample.cs:19)
```

## 配置转换器

如果未更改 `JsonConvert.DefaultSettings`，此包会自动将其所有转换器添加到其中。

如果您只想使用部分转换器，或者添加一些自定义转换器，可以通过以下选项进行配置。

### 默认设置

- 使用自定义 Unity 契约解析器，该解析器查找 `[SerializeField]` 属性。
- 使用 Newtonsoft.Json 的部分转换器：
  - StringEnumConverter
  - VersionConverter
- 使用此包中的所有转换器。
- 使用定义在 `Newtonsoft.Json` 命名空间之外的所有其他转换器。

### 通过代码自定义设置

您可以在代码中覆盖这些默认值。例如：

```cs
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.Math;

var settings = new JsonSerializerSettings {
    Converters = new [] {
        new Vector3Converter(),
        new StringEnumConverter(),
    },
    ContractResolver = new UnityTypeContractResolver(),
};

var myObjectINeedToSerialize = new Vector3(1, 2, 3);
var json = JsonConvert.SerializeObject(myObjectINeedToSerialize, settings);
```

### 通过配置文件自定义设置

您可以配置和覆盖默认值。
要打开设置，请点击顶部菜单栏中的 **"Tools"** 并选择 **"Newtonsoft Json > Converters settings..."** (注意：菜单位置已在 v2.0.0 中更改)。

在此设置页面中，您可以启用或禁用默认包含的任何转换器。
设置页面还包括 JsonSerializerSettings 属性的配置，例如：
- Type Name Handling
- Null Value Handling
- Default Value Handling
- Reference Loop Handling
- Formatting (compact or indented)
- ...

## 包结构

此包遵循 Unity Package Manager (UPM) 标准目录结构：

```
/
├── Runtime/              # 运行时代码
├── Editor/               # 编辑器代码
├── Tests~/               # 单元测试 (隐藏)
├── Documentation~/       # 文档 (隐藏)
├── package.json          # UPM 包清单
├── README.md
├── README_CN.md
├── LICENSE.md
└── CHANGELOG.md
```

## 贡献

如果您想做出贡献：

- **宣传！** ❤ 更多用户 &rarr; 更多反馈 &rarr; 更有动力。
- [提交 Issue][issue-create]。
- 提交 PR。

## 更新日志

请参阅此包内的 [CHANGELOG.md][changelog.md] 文件。

---

此包根据 MIT 许可证 (MIT) 授权

Copyright (c) 2019 Kalle Fagerberg (jilleJr)  
<https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters>

在仓库内的 [LICENSE.md][license.md] 中查看完整的版权信息。

[license.md]: /LICENSE.md
[changelog.md]: /CHANGELOG.md
[doc-compatability-table]: Documentation~/Compatibility-table.md
[issue-create]: https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters/issues/new/choose
