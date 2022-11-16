// ================================================================================
//
//      作者  :   G Q
//      时间  :   2022年11月11日 18:32:13
//      类名  :   UnityEngineEx
//      目的  :   Unity API扩展
//
// ================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity扩展
/// </summary>
public static class UnityEngineEx
{
    /// <summary>
    /// 设置 Sprite 图片
    /// </summary>
    /// <param name="image">Image组件</param>
    /// <param name="atlasPath">图集路径</param>
    /// <param name="spriteName">图片名字</param>
    /// <param name="bNativeSize">是否保持原图大小</param>
    public static void SetSprite(this Image image, string atlasPath, string spriteName, bool bNativeSize)
    {
        Sprite sprite = ResourceManager.LoadSprite(atlasPath, spriteName);
        if (ResourceManager.IsNull(sprite))
        {
            return;
        }

        image.sprite = sprite;
        if (bNativeSize)
        {
            image.SetNativeSize();
        }
    }

    /// <summary>
    /// 设置 Texture 图片
    /// </summary>
    /// <param name="rawImage">RawImage组件</param>
    /// <param name="texturePath">图片路径</param>
    /// <param name="bNativeSize">是否保持原图大小</param>
    public static void SetTexture(this RawImage rawImage, string texturePath, bool bNativeSize)
    {
        Texture texture = ResourceManager.LoadTexture(texturePath);
        if (ResourceManager.IsNull(texture))
        {
            return;
        }

        rawImage.texture = texture;
        if (bNativeSize)
        {
            rawImage.SetNativeSize();
        }
    }

    /// <summary>
    /// 获取或添加组件
    /// </summary>
    /// <typeparam name="T">Component组件</typeparam>
    /// <param name="go">该游戏物体</param>
    /// <returns>Component组件</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }
}