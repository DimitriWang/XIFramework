using System.IO;
using UnityEditor;
using UnityEngine;

namespace XIFramework.SkillEditor
{
    public static class SkillEditorAssistant
    {
        /// <summary>
        /// 通过传入的ScriptableObject实例查找对应的UXML文件路径
        /// </summary>
        /// <param name="scriptableObject">调用者的ScriptableObject实例</param>
        /// <param name="fileName">要查找的文件名（可选，默认使用脚本名称）</param>
        /// <returns>UXML文件的AssetDatabase路径，如果未找到则返回空字符串</returns>
        public static string FindUXMLPath(ScriptableObject scriptableObject, string fileName = null)
        {
            if (scriptableObject == null)
            {
                UnityEngine.Debug.LogError("传入的scriptableObject实例为空");
                return string.Empty;
            }

            // 获取脚本对象
            MonoScript script = MonoScript.FromScriptableObject(scriptableObject);
            if (script == null)
            {
                UnityEngine.Debug.LogError("无法获取脚本对象");
                return string.Empty;
            }

            // 获取脚本路径
            string scriptPath = AssetDatabase.GetAssetPath(script);
            if (string.IsNullOrEmpty(scriptPath))
            {
                UnityEngine.Debug.LogError("无法获取脚本路径");
                return string.Empty;
            }

            // 如果没有指定文件名，则使用脚本名称
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = scriptableObject.GetType().Name;
            }

            return FindUXMLPathByScriptPath(scriptPath, fileName);
        }

        /// <summary>
        /// 通过传入的MonoBehaviour实例查找对应的UXML文件路径
        /// </summary>
        /// <param name="behaviour">调用者的MonoBehaviour实例</param>
        /// <param name="fileName">要查找的文件名（可选，默认使用脚本名称）</param>
        /// <returns>UXML文件的AssetDatabase路径，如果未找到则返回空字符串</returns>
        public static string FindUXMLPath(MonoBehaviour behaviour, string fileName = null)
        {
            if (behaviour == null)
            {
                UnityEngine.Debug.LogError("传入的behaviour实例为空");
                return string.Empty;
            }

            // 获取脚本对象
            MonoScript script = MonoScript.FromMonoBehaviour(behaviour);
            if (script == null)
            {
                UnityEngine.Debug.LogError("无法获取脚本对象");
                return string.Empty;
            }

            // 获取脚本路径
            string scriptPath = AssetDatabase.GetAssetPath(script);
            if (string.IsNullOrEmpty(scriptPath))
            {
                UnityEngine.Debug.LogError("无法获取脚本路径");
                return string.Empty;
            }

            // 如果没有指定文件名，则使用脚本名称
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = behaviour.GetType().Name;
            }

            return FindUXMLPathByScriptPath(scriptPath, fileName);
        }

        /// <summary>
        /// 根据脚本路径查找对应的UXML文件路径
        /// </summary>
        /// <param name="scriptPath">脚本的完整路径</param>
        /// <param name="fileName">要查找的文件名（不包含扩展名）</param>
        /// <returns>UXML文件的AssetDatabase路径，如果未找到则返回空字符串</returns>
        public static string FindUXMLPathByScriptPath(string scriptPath, string fileName = null)
        {
            if (string.IsNullOrEmpty(scriptPath))
            {
                UnityEngine.Debug.LogError("脚本路径为空");
                return string.Empty;
            }

            // 如果没有指定文件名，则使用脚本名称
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileNameWithoutExtension(scriptPath);
            }


            // 获取脚本所在目录
            string scriptDirectory = Path.GetDirectoryName(scriptPath);

            // 向上查找直到找到Asset目录
            string currentDirectory = scriptDirectory;
            int level = 0;
            while (!string.IsNullOrEmpty(currentDirectory) && level < 10) // 限制搜索层数防止死循环
            {

                string assetDir = Path.Combine(currentDirectory, "Asset");
                if (Directory.Exists(assetDir))
                {

                    // 在Asset目录下查找VisualTree子目录
                    string visualTreeDir = Path.Combine(assetDir, "VisualTree");
                    if (Directory.Exists(visualTreeDir))
                    {

                        // 查找对应的uxml文件
                        string uxmlFilePath = Path.Combine(visualTreeDir, $"{fileName}.uxml");

                        if (File.Exists(uxmlFilePath))
                        {
                            // 转换为AssetDatabase路径格式
                            string assetPath = uxmlFilePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            return assetPath;
                        }
                        else
                        {
                            UnityEngine.Debug.Log($"文件不存在: {uxmlFilePath}");
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"VisualTree目录不存在: {visualTreeDir}");
                    }
                }
                else
                {
                }

                // 向上一级目录
                currentDirectory = Path.GetDirectoryName(currentDirectory);
                level++;

                // 防止无限循环
                if (currentDirectory == Application.dataPath || string.IsNullOrEmpty(currentDirectory))
                    break;
            }

            UnityEngine.Debug.LogWarning($"未找到 {fileName}.uxml 文件");
            return string.Empty;
        }

        /// <summary>
        /// 根据指定的脚本类型查找UXML路径
        /// </summary>
        /// <typeparam name="T">脚本类型</typeparam>
        /// <param name="fileName">要查找的文件名（可选）</param>
        /// <returns>UXML文件的AssetDatabase路径</returns>
        public static string FindUXMLPath<T>(string fileName = null) where T : Object
        {
            // 通过AssetDatabase查找脚本
            string[] guids = AssetDatabase.FindAssets($"t:Script {typeof(T).Name}");
            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                UnityEngine.Debug.Log($"通过类型查找脚本路径: {scriptPath}");

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = typeof(T).Name;
                }

                return FindUXMLPathByScriptPath(scriptPath, fileName);
            }

            UnityEngine.Debug.LogError($"找不到脚本类型: {typeof(T).Name}");
            return string.Empty;
        }
    }
}