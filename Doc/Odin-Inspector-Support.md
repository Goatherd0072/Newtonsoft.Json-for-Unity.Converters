# Odin Inspector Support

This package includes optional support for Odin Inspector. When Odin Inspector is installed in your project, the configuration inspector will automatically use Odin's enhanced UI features.

## Features with Odin Inspector

When Odin Inspector is detected (via the `ODIN_INSPECTOR` scripting define symbol), the following enhancements are automatically enabled:

- **Foldout Groups**: Settings are organized into collapsible groups for better organization
- **Info Boxes**: Helpful descriptions appear directly in the inspector
- **Enum Toggle Buttons**: JsonSerializerSettings enums are displayed as toggle buttons for easier selection
- **Conditional Visibility**: Lists only show when "use all" is disabled
- **Better List Display**: Converter lists use Odin's enhanced list drawer

## How to Enable

1. Install Odin Inspector in your project via UPM or Asset Store
2. The package will automatically detect Odin Inspector and enable enhanced features
   - Detection is automatic via `versionDefines` in the assembly definition files
   - The `ODIN_INSPECTOR` scripting define symbol is set automatically when `com.sirenix.odininspector` package is detected
   - No manual configuration needed!
3. Open the Json.NET converters settings (Edit > Json.NET converters settings...)
4. The inspector will automatically use Odin's enhanced UI

**Note**: If you're using Odin Inspector from the Asset Store (not UPM), you may need to manually add the `ODIN_INSPECTOR` scripting define symbol via Project Settings > Player > Scripting Define Symbols.

## Without Odin Inspector

If Odin Inspector is not installed, the package works perfectly fine using Unity's default inspector. The custom `UnityConvertersConfigEditor` provides a good experience even without Odin.

## Technical Details

The implementation uses conditional compilation (`#if ODIN_INSPECTOR`) to add Odin attributes to the configuration class only when Odin is available. This ensures:

- No dependencies on Odin Inspector when it's not installed
- Zero performance impact when Odin is not present
- Automatic detection via `versionDefines` in assembly definition files
- No manual switching required

### Automatic Detection

The package automatically detects Odin Inspector using Unity's `versionDefines` feature in the assembly definition files:

```json
"versionDefines": [
    {
        "name": "com.sirenix.odininspector",
        "expression": "",
        "define": "ODIN_INSPECTOR"
    }
]
```

When the `com.sirenix.odininspector` package is installed (via UPM), Unity automatically defines the `ODIN_INSPECTOR` symbol, enabling all Odin-specific code.

### Asset Store Installation

If you're using Odin Inspector from the Unity Asset Store (not UPM), the package name may not be `com.sirenix.odininspector`. In this case:
1. Check if the `ODIN_INSPECTOR` symbol is already defined by Odin's own setup
2. If not, manually add `ODIN_INSPECTOR` to Project Settings > Player > Scripting Define Symbols
