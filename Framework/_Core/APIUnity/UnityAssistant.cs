using System.Collections.Generic;
using UnityEngine;

namespace XIFramework
{
    public class GameObjectKeyName
    {
        public static readonly string Level = "level";
        public static readonly string LevelLiving = "level_living";
        public static readonly string LevelDead = "level_dead";
        public static readonly string FadeOut = "FadeOut";
        public static readonly string LOD = "LOD_";
        public static readonly string LOD_HEIGHT = "LOD_H";
        public static readonly string LOD_LOW = "LOD_L";
        public static readonly string BODY_LOW = "BodyLow";
    }


    public enum UnityLayer
    {
        Default,
        TransparentFx,
        IgnoreRaycast,
        Empty3,
        Water,
        UI,
        Empty6,
        Empty7,
        PostProcessing,
        Entity,
        Scene,
        Effect,
        HUD,
        
        SceneCameraMask = (1 << Default) | (1 << TransparentFx) | (1 << Water) | (1 << Entity) | (1 << Scene) |
                          (1 << Effect),
        SceneLightMask = (1 << Default) | (1 << Entity) | (1 << Scene),
    }


    public static class UnityComponentList<T>
        where T : Component
    {
        public static readonly List<T> List = new List<T>();
        
        public static void Begin(GameObject obj)
        {
            Begin(obj, false);
        }
        
        public static void Begin(GameObject obj, bool includeInactive)
        {
            List.Clear();
            if (obj != null)
            {
                obj.GetComponentsInChildren<T>(includeInactive, List);
            }
        }
        
        public static void End()
        {
            List.Clear();
        }
    }

    public static class UnityAssistant
    {
        public static void Exit()
        {
            if (Application.isEditor)
            {
                //ViDebuger.Record("App.Exit");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public static void Pause()
        {
            if (Application.isEditor)
            {
                //ViDebuger.Record("App.Pause");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
#endif
            }
        }

        public static void SetActive(GameObject obj, bool active)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }


        public static void SetActiveEx(GameObject obj, bool value)
        {
            if (obj != null)
            {
                if (value)
                {
                    if (obj.activeInHierarchy)
                    {
                        return;
                    }
                }
                else
                {
                    if (!obj.activeInHierarchy)
                    {
                        return;
                    }
                }

                obj.SetActive(value);
            }
        }


        public static void UpdateUIScale(this GameObject obj, float value)
        {
            if (obj != null)
            {
                obj.transform.localScale = new Vector3(value, value, 1.0f);
            }
        }

        //
        public static void UpdateUIAlpha(this GameObject obj, float value)
        {
            if (obj != null)
            {
                CanvasGroup group = obj.GetComponentInChildren<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = value;
                }
            }
        }

        public static T Instantiate<T>(this T selfObj) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation, parent);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            return (T)UnityEngine.Object.Instantiate((UnityEngine.Object)selfObj, parent, worldPositionStays);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Component parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            return (T)UnityEngine.Object.Instantiate((UnityEngine.Object)selfObj, parent.transform, worldPositionStays);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Transform parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, parent, false);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Component parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, parent.transform, false);
        }


        public static T Name<T>(this T selfObj, string name) where T : UnityEngine.Object
        {
            selfObj.name = name;
            return selfObj;
        }
        
        public static void Destroy(ref UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

            //
            UnityEngine.GameObject gameObj = obj as UnityEngine.GameObject;
            if (gameObj != null)
            {
                gameObj.SetActive(false);
                gameObj.transform.parent = null;
            }

            UnityEngine.Object.Destroy(obj);
            obj = null;
        }
        
        public static void Destroy(ref UnityEngine.GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            obj.transform.SetParent(null, false);
            obj.SetActive(false);
            UnityEngine.Object.Destroy(obj);
            obj = null;
        }
        
        
        //
        public static void Destroy(List<UnityEngine.Object> objList)
        {
            if (objList == null)
            {
                return;
            }
            for (int iter = 0, end = objList.Count; iter < end; ++iter)
            {
                UnityEngine.Object iterObj = objList[iter];
                Destroy(ref iterObj);
            }
            objList.Clear();
        }


        public static void DestroySelf<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj);
            }

            return selfObj;
        }

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }


        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }
    }
}