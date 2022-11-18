// ================================================================================
//
//      作 者  :   G Q
//      时 间  :   2022年11月09日 15:44:47
//      类 名  :   ResourceManager
//      目 的  :   CS 端的资源管理器，管理整个框架的资源加载卸载等
//
// ================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CS资源管理器
/// </summary>
public class ResourceManager
{
    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="prefabPath">预制体路径</param>
    /// <param name="callback">加载完成回调</param>
    public static void LoadPrefab(Transform parent, string prefabPath, Action<string, GameObject> callback)
    {
        ResourceRequest rq = Resources.LoadAsync<GameObject>(prefabPath);
        rq.completed += delegate(AsyncOperation obj)
        {
            GameObject go = (obj as ResourceRequest).asset as GameObject;
            if (go != null)
            {
                var prefab = UnityEngine.Object.Instantiate(go, parent);
                callback?.Invoke(prefabPath, prefab);
            }
        };
    }

    /// <summary>
    /// 同步加载预制体
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="prefabPath">预制体路径</param>
    /// <param name="callback">加载完成回调</param>
    /// <returns>预制体</returns>
    public static GameObject LoadPrefabSync(Transform parent, string prefabPath, Action<string, GameObject> callback)
    {
        GameObject go = Resources.Load<GameObject>(prefabPath);
        var prefab = UnityEngine.Object.Instantiate(go, parent);
        callback?.Invoke(prefabPath, prefab);
        return prefab;
    }

    /// <summary>
    /// 加载Sprite图片
    /// </summary>
    /// <param name="atlasPath">图集路径</param>
    /// <param name="spriteName">图片名字</param>
    /// <returns>Sprite 图片</returns>
    public static Sprite LoadSprite(string atlasPath, string spriteName)
    {
        Sprite sprite = SpritesManager.LoadAtlas<Sprite>(atlasPath, spriteName);
        return sprite;
    }

    /// <summary>
    /// 加载Texture图片
    /// </summary>
    /// <param name="textureName">图片名字</param>
    /// <returns>Texture 图片</returns>
    public static Texture LoadTexture(string textureName)
    {
        Texture texture = Resources.Load<Texture>(textureName);
        return texture;
    }

    /// <summary>
    /// 加载音频
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <returns>音频</returns>
    public static AudioClip LoadAudioClip(string path)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        return audioClip;
    }

    /// <summary>
    /// 判断一个物体是否为空
    /// </summary>
    /// <param name="obj">物体</param>
    /// <returns>是否为空？ true ：是</returns>
    public static bool IsNull(System.Object obj)
    {
        return obj?.Equals(null) ?? true;
    }
}