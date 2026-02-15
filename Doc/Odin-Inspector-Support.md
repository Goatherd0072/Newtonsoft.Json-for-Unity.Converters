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

1. Install Odin Inspector in your project
2. Odin Inspector should automatically add the `ODIN_INSPECTOR` scripting define symbol
   - Verify in Project Settings > Player > Scripting Define Symbols
   - If not present, add it manually to enable Odin features
3. Open the Json.NET converters settings (Edit > Json.NET converters settings...)
4. The inspector will automatically use Odin's enhanced UI

## Without Odin Inspector

If Odin Inspector is not installed, the package works perfectly fine using Unity's default inspector. The custom `UnityConvertersConfigEditor` provides a good experience even without Odin.

## Technical Details

The implementation uses conditional compilation (`#if ODIN_INSPECTOR`) to add Odin attributes to the configuration class only when Odin is available. This ensures:

- No dependencies on Odin Inspector when it's not installed
- Zero performance impact when Odin is not present
- Automatic detection and usage when Odin is available
- No manual switching required
