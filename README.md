# Unity Converters for Newtonsoft.Json

[![CircleCI](https://img.shields.io/circleci/build/gh/applejag/Newtonsoft.Json-for-Unity.Converters/master?logo=circleci&style=flat-square)](https://circleci.com/gh/applejag/Newtonsoft.Json-for-Unity.Converters)
[![Codacy grade](https://img.shields.io/codacy/grade/de7041b5f9f9415a8add975d1b8a9fcf?logo=codacy&style=flat-square)](https://www.codacy.com/manual/jilleJr/Newtonsoft.Json-for-Unity.Converters?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=jilleJr/Newtonsoft.Json-for-Unity.Converters&amp;utm_campaign=Badge_Grade)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg?style=flat-square)](/CODE_OF_CONDUCT.md)

[English](./README.md) | [中文](./README_CN.md)

This package contains converters to and from common Unity types. Types such as
**Vector2, Vector3, Matrix4x4, Quaternions, Color, even ScriptableObject,**
*and many, many more.*
(See the [full compatibility table of all +50 supported Unity types][doc-compatability-table])

> [!NOTE]
> **This is a fork** maintained by Goatherd0072. The original project by jilleJr/applejag is no longer maintained.
> This fork has been restructured to follow UPM standards and is tested/maintained for **Unity 2022+** only.

## Dependencies

### Newtonsoft.Json packages

This package requires the `Newtonsoft.Json.dll` file to be present in your
project. It **does not** have to be used with the `jillejr.newtonsoft.json-for-unity` package!

This package can be combined with any of the following:

- Unity's official Newtonsoft.Json package:
  [`com.unity.nuget.newtonsoft-json`](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html) 
  *(recommended)*

- The original jilleJr fork:
  [jilleJr/Newtonsoft.Json-for-Unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity)

- SaladLab's fork: [SaladLab/Json.Net.Unity3D](https://github.com/SaladLab/Json.Net.Unity3D)

- ParentElement's Assets Store package: <https://www.parentelement.com/assets/json_net_unity>

- *Any other source, as long as it declares the base `Newtonsoft.Json` types.*

### Newtonsoft.Json versions

There's no hard linking towards a specific version. The package has been tested
and works as-is with Newtonsoft.Json 10.0.3, 11.0.2, 12.0.3 and 13.0.1.

## Installation

### Install via Git URL (Recommended)

This is the recommended installation method for this pure UPM package.

1. Open Unity Package Manager (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git`
5. Click "Add"

You can also add a specific version, tag, or branch by appending it to the URL:
```
https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git#v2.0.0
```

### Install via manifest.json

Alternatively, you can add this package directly to your project's `Packages/manifest.json` file:

```json
{
  "dependencies": {
    "jillejr.newtonsoft.json-for-unity.converters": "https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters.git"
  }
}
```

## What does it solve

A lot of Unity types causes self-referencing loops, such as the Vector3 type.
While serializing the value `new Vector3(0,1,0)`, Newtonsoft.Json will start
writing:

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

*and so on, until "recursion" error..* But there are also some types such as the
`UnityEngine.RandomState` that has its state variables hidden.

The converters in this package takes care of these issues, as well as many more.

### Sample error without this package

Given this piece of code:

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

Then the following is shown:

```log
JsonSerializationException: Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'normalized'.
...
```

### Sample with this package

Same `NewBehaviour` script as above, but just by adding this package you instead
see:

```log
Position as JSON: {"x":201.0,"y":219.5,"z":0.0}
UnityEngine.Debug:Log(Object)
Sample:Start() (at Assets/Sample.cs:19)
```

## Configuring converters

This package automatically adds all its converters to the
[`JsonConvert.DefaultSettings`](https://www.newtonsoft.com/json/help/html/DefaultSettings.htm)
if that value has been left untouched.

If you want to only use a certain subset of the converters, or perhaps add in
some of your own custom converters, then you have some options that will be
explored just below here.

### Default settings

- Use the custom Unity contract resolver that looks for `[SerializeField]`
  attributes.

- Use only some of the Newtonsoft.Json converters, namely:

  - [StringEnumConverter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Converters_StringEnumConverter.htm)
  - [VersionConverter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Converters_VersionConverter.htm)

- Use all converters from this package.

- Use all other converters defined outside the `Newtonsoft.Json` namespace.

### Custom settings through code

You can override these defaults mentioned above in your code whenever you're
serializing/deserializing. For example:

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

### Custom settings through config file

You can configure and override these defaults.
To open the settings, click **"Tools"** in the top menu bar and select
**"Newtonsoft Json > Converters settings..."** (Note: menu location changed in v2.0.0).

![Configuring ScriptableObject config](Doc/images/configure-scriptableobject.png)

Within this settings page, you can enable or disable any of the converters you
wish to include or omit by default.

The settings page also includes configuration for JsonSerializerSettings properties.

## Package Structure

This package follows the Unity Package Manager (UPM) standard directory structure:

```
/
├── Runtime/              # Runtime code (converters, configuration, utilities)
├── Editor/               # Editor-only code (custom inspectors, menus)
├── Tests~/              # Unit tests (hidden, but importable via Package Manager Samples)
├── Documentation~/      # Documentation files (hidden from Unity project view)
├── package.json         # UPM package manifest
├── README.md
├── README_CN.md
├── LICENSE.md
└── CHANGELOG.md
```

## Contributing

Thankful that you're even reading this :)

If you want to contribute, here's what you can do:

- **Spread the word!** ❤ More users &rarr; more feedback &rarr; I get more
  will-power to work on this project. This is the best way to contribute!

- [Open an issue][issue-create]. Could be a feature request for a new converter,
  or maybe you've found a bug?

- Open a PR with some new feature or issue solved.

## Changelog

Please see the [CHANGELOG.md][changelog.md] file inside this package.

---

This package is licensed under The MIT License (MIT)

Copyright (c) 2019 Kalle Fagerberg (jilleJr)  
<https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters>

See full copyrights in [LICENSE.md][license.md] inside repository

[license.md]: /LICENSE.md
[changelog.md]: /CHANGELOG.md
[doc-compatability-table]: Documentation~/Compatibility-table.md
[issue-create]: https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters/issues/new/choose
[issue-list-unassigned]: https://github.com/Goatherd0072/Newtonsoft.Json-for-Unity.Converters/issues?q=is%3Aopen+is%3Aissue+no%3Aassignee