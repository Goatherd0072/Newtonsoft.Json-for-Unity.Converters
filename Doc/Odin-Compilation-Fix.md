# Odin Inspector Compilation Fix

## Problem

When using this package as a local package without Odin Inspector installed, Unity throws a compilation error:

```
C:\Project\Newtonsoft.Json-for-Unity.Converters\Configuration\UnityConvertersConfig.cs(7,7): 
error CS0246: The type or namespace name 'Sirenix' could not be found 
(are you missing a using directive or an assembly reference?)
```

## Root Cause

The code uses conditional compilation (`#if ODIN_INSPECTOR`) to optionally include Odin Inspector features:

```csharp
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
```

However, the `ODIN_INSPECTOR` scripting define symbol was not being set automatically, causing Unity to try to compile the Odin Inspector code even when Odin was not installed.

## Solution

We added **automatic detection** of Odin Inspector using Unity's `versionDefines` feature in the assembly definition files.

### Changes Made

#### 1. Newtonsoft.Json.UnityConverters.asmdef

Added version define to automatically set `ODIN_INSPECTOR` when Odin Inspector is detected:

```json
"versionDefines": [
    {
        "name": "com.sirenix.odininspector",
        "expression": "",
        "define": "ODIN_INSPECTOR"
    }
]
```

#### 2. Newtonsoft.Json.UnityConverters.Editor.asmdef

Added the same version define to the editor assembly:

```json
"versionDefines": [
    {
        "name": "com.sirenix.odininspector",
        "expression": "",
        "define": "ODIN_INSPECTOR"
    }
]
```

## How It Works

1. **Without Odin Inspector:**
   - The `com.sirenix.odininspector` package is not present
   - Unity does NOT define `ODIN_INSPECTOR`
   - All code inside `#if ODIN_INSPECTOR` blocks is excluded from compilation
   - The package compiles successfully without any Odin dependencies

2. **With Odin Inspector (via UPM):**
   - The `com.sirenix.odininspector` package is installed
   - Unity automatically defines `ODIN_INSPECTOR` via `versionDefines`
   - All Odin-specific code is included
   - Enhanced UI features are enabled

3. **With Odin Inspector (via Asset Store):**
   - Odin Inspector typically sets its own define symbols
   - If not, users can manually add `ODIN_INSPECTOR` to Project Settings > Player > Scripting Define Symbols

## Testing

To verify the fix:

1. **Without Odin Inspector:**
   - Clone the repository
   - Add it as a local package to a Unity project
   - Ensure Odin Inspector is NOT installed
   - The package should compile without errors

2. **With Odin Inspector:**
   - Install Odin Inspector via UPM as `com.sirenix.odininspector`
   - The package should detect it automatically
   - Open Edit > Json.NET converters settings...
   - The inspector should show enhanced Odin UI (foldout groups, info boxes, etc.)

## Benefits

- ✅ **No manual configuration required** - Detection is fully automatic
- ✅ **Works with or without Odin** - Package is truly optional dependency
- ✅ **No compilation errors** - Clean compilation regardless of Odin presence
- ✅ **Zero performance impact** - No Odin overhead when not installed
- ✅ **Enhanced UX when available** - Better inspector when Odin is present

## Related Documentation

- [Odin Inspector Support Documentation](Odin-Inspector-Support.md)
- [Unity versionDefines Documentation](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html#define-symbols)
