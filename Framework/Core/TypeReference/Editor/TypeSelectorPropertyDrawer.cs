namespace XIFramework.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [CustomPropertyDrawer(typeof(TypeReference))]
    [CustomPropertyDrawer(typeof(TypeConstraintAttribute))]
    public class TypeSelectorPropertyDrawer : PropertyDrawer
    {
        private struct TypeInfo
        {
            public string Name;
            public string AssemblyQualifiedName;
            public string AssemblyName;
        }

        private static Dictionary<string, List<TypeInfo>> _typeCache =
            new Dictionary<string, List<TypeInfo>>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 获取类型约束属性
            TypeConstraintAttribute constraint = null;

            // 从字段获取约束
            var fieldConstraints = fieldInfo.GetCustomAttributes(typeof(TypeConstraintAttribute), true);
            if (fieldConstraints != null && fieldConstraints.Length > 0)
            {
                constraint = fieldConstraints[0] as TypeConstraintAttribute;
            }

            // 从属性绘制器获取约束
            if (constraint == null && attribute is TypeConstraintAttribute)
            {
                constraint = attribute as TypeConstraintAttribute;
            }

            // 如果还是没有约束，显示默认字段
            if (constraint == null || constraint.BaseType == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // 获取缓存键
            string cacheKey =
                $"{constraint.BaseType.FullName}_{constraint.AllowAbstract}_{constraint.IncludeEditorAssemblies}";

            // 获取类型缓存
            if (!_typeCache.TryGetValue(cacheKey, out var types))
            {
                types = GetValidTypes(constraint.BaseType, constraint.AllowAbstract,
                    constraint.IncludeEditorAssemblies);
                _typeCache[cacheKey] = types;

                // 打印调试信息
                Debug.Log($"Found {types.Count} types for {constraint.BaseType.Name} " +
                          $"(EditorAssemblies: {constraint.IncludeEditorAssemblies})");

                foreach (var type in types)
                {
                    Debug.Log($"- {type.Name} ({type.AssemblyName})");
                }
            }

            // 获取当前值
            string currentValue = "";

            if (fieldInfo.FieldType == typeof(TypeReference))
            {
                SerializedProperty typeRefProp = property.FindPropertyRelative("_assemblyQualifiedName");
                if (typeRefProp != null)
                {
                    currentValue = typeRefProp.stringValue;
                }
            }
            else
            {
                currentValue = property.stringValue;
            }

            // 查找当前选择的索引
            int selectedIndex = -1;
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].AssemblyQualifiedName == currentValue)
                {
                    selectedIndex = i;
                    break;
                }
            }

            // 创建显示名称列表
            string[] displayNames = types.Select(t => $"{t.Name} ({t.AssemblyName})").ToArray();

            // 绘制下拉菜单
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayNames);

            if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < types.Count)
            {
                string newValue = types[newIndex].AssemblyQualifiedName;

                if (fieldInfo.FieldType == typeof(TypeReference))
                {
                    SerializedProperty typeRefProp = property.FindPropertyRelative("_assemblyQualifiedName");
                    if (typeRefProp != null)
                    {
                        typeRefProp.stringValue = newValue;
                    }
                }
                else
                {
                    property.stringValue = newValue;
                }
            }

            // 如果没有选择且列表不为空，设置默认值
            if (selectedIndex == -1 && types.Count > 0)
            {
                string defaultValue = types[0].AssemblyQualifiedName;

                if (fieldInfo.FieldType == typeof(TypeReference))
                {
                    SerializedProperty typeRefProp = property.FindPropertyRelative("_assemblyQualifiedName");
                    if (typeRefProp != null)
                    {
                        typeRefProp.stringValue = defaultValue;
                    }
                }
                else
                {
                    property.stringValue = defaultValue;
                }
            }
        }

        private List<TypeInfo> GetValidTypes(System.Type baseType, bool allowAbstract, bool includeEditorAssemblies)
        {
            var validTypes = new List<TypeInfo>();

            // 添加"None"选项
            validTypes.Add(new TypeInfo
            {
                Name = "None",
                AssemblyQualifiedName = "",
                AssemblyName = ""
            });

            // 遍历所有程序集
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().Name;

                // 跳过系统程序集
                if (ShouldSkipAssembly(assemblyName, includeEditorAssemblies))
                {
                    continue;
                }

                System.Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load types from assembly {assemblyName}: {e.Message}");
                    continue;
                }

                foreach (var type in types)
                {
                    if (type == null) continue;

                    try
                    {
                        // 检查类型是否有效
                        if (IsValidType(type, baseType, allowAbstract))
                        {
                            validTypes.Add(new TypeInfo
                            {
                                Name = type.Name,
                                AssemblyQualifiedName = type.AssemblyQualifiedName,
                                AssemblyName = assemblyName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Error processing type {type.Name} in {assemblyName}: {e.Message}");
                    }
                }
            }

            // 按名称排序
            validTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return validTypes;
        }

        private bool ShouldSkipAssembly(string assemblyName, bool includeEditorAssemblies)
        {
            // 总是跳过系统程序集
            if (assemblyName.StartsWith("System.") ||
                assemblyName == "mscorlib" ||
                assemblyName.StartsWith("netstandard") ||
                assemblyName.StartsWith("Microsoft.") ||
                assemblyName.StartsWith("Mono.") ||
                assemblyName.StartsWith("Unity.") ||
                assemblyName.StartsWith("UnityEngine.") ||
                assemblyName.StartsWith("UnityEditor."))
            {
                return true;
            }

            // 根据设置决定是否跳过Editor程序集
            if (!includeEditorAssemblies &&
                (assemblyName.EndsWith(".Editor") || assemblyName.Contains(".Editor.")))
            {
                return true;
            }

            return false;
        }

        private bool IsValidType(System.Type type, System.Type baseType, bool allowAbstract)
        {
            // 跳过泛型类型
            if (type.IsGenericTypeDefinition)
                return false;

            // 检查是否继承自基类
            if (!baseType.IsAssignableFrom(type))
                return false;

            // 跳过抽象类（除非允许）
            if (type.IsAbstract && !allowAbstract)
                return false;

            // 跳过UnityEngine.Object（Component除外）
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) &&
                !typeof(UnityEngine.Component).IsAssignableFrom(type))
            {
                return false;
            }

            // 跳过编译器生成的类型
            if (type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0)
                return false;

            return true;
        }

        // 重置缓存
        public static void ResetCache()
        {
            _typeCache.Clear();
        }
    }
#endif
}