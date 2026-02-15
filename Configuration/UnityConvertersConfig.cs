using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Newtonsoft.Json.UnityConverters.Configuration
{

#pragma warning disable CA2235 // Mark all non-serializable fields
    [Serializable]
    public sealed class UnityConvertersConfig : ScriptableObject
    {
        internal const string PATH = "Assets/Resources/Newtonsoft.Json-for-Unity.Converters.asset";
        internal const string PATH_FOR_RESOURCES_LOAD = "Newtonsoft.Json-for-Unity.Converters";

#if ODIN_INSPECTOR
        [FoldoutGroup("Contract Resolver")]
        [InfoBox("Custom IContractResolver that properly handles SerializeField attributes and creates ScriptableObjects correctly.")]
#endif
        public bool useUnityContractResolver = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("Outside Converters")]
        [InfoBox("Registers all classes outside of the 'Newtonsoft.Json.*' namespace that derive from JsonConverter.")]
#endif
        public bool useAllOutsideConverters = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("Outside Converters")]
        [ShowIf("@!useAllOutsideConverters")]
        [ListDrawerSettings(ShowIndexLabels = false, ListElementLabelName = "converterName")]
#endif
        public List<ConverterConfig> outsideConverters = new List<ConverterConfig>();

#if ODIN_INSPECTOR
        [FoldoutGroup("Unity Converters")]
        [InfoBox("Registers all classes inside of the 'Newtonsoft.Json.UnityConverters.*' namespace.")]
#endif
        public bool useAllUnityConverters = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("Unity Converters")]
        [ShowIf("@!useAllUnityConverters")]
        [ListDrawerSettings(ShowIndexLabels = false, ListElementLabelName = "converterName")]
#endif
        public List<ConverterConfig> unityConverters = new List<ConverterConfig>();

#if ODIN_INSPECTOR
        [FoldoutGroup("Json.NET Converters")]
        [InfoBox("Registers converters from the Newtonsoft.Json library itself.")]
#endif
        public bool useAllJsonNetConverters;

#if ODIN_INSPECTOR
        [FoldoutGroup("Json.NET Converters")]
        [ShowIf("@!useAllJsonNetConverters")]
        [ListDrawerSettings(ShowIndexLabels = false, ListElementLabelName = "converterName")]
#endif
        public List<ConverterConfig> jsonNetConverters = new List<ConverterConfig> {
            new ConverterConfig { converterName = typeof(StringEnumConverter).FullName, enabled = true },
            new ConverterConfig { converterName = typeof(VersionConverter).FullName, enabled = true },
        };

#if ODIN_INSPECTOR
        [FoldoutGroup("Auto Sync")]
        [InfoBox("Automatic synchronization of JsonConverter types. Disable this on large projects to reduce load spikes on assembly reload.")]
#endif
        public bool autoSyncConverters = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings", expanded: true)]
        [Title("Serialization Settings")]
        [InfoBox("Configure the default JsonSerializerSettings properties applied by this package.")]
        [EnumToggleButtons]
#endif
        public TypeNameHandling typeNameHandling = TypeNameHandling.None;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public NullValueHandling nullValueHandling = NullValueHandling.Include;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Error;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public Formatting formatting = Formatting.None;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public DateFormatHandling dateFormatHandling = DateFormatHandling.IsoDateFormat;

#if ODIN_INSPECTOR
        [FoldoutGroup("JsonSerializerSettings")]
        [EnumToggleButtons]
#endif
        public MissingMemberHandling missingMemberHandling = MissingMemberHandling.Ignore;
    }
#pragma warning restore CA2235 // Mark all non-serializable fields
}
