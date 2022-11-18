// ================================================================================
//
//      作 者  :   G Q
//      时 间  :   2022年10月11日 17:46:22
//      类 名  :   SpritesManager
//      目 的  :   图集管理器，管理项目中的图片资源加载
//                  避免使用过多未打图集的图导致DrawCall过高
//
// ================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图集管理器
/// </summary>
public class SpritesManager
{
    /// <summary>
    /// 所有图集列表 key : 图集路径  value : 该图集下所有小图
    /// </summary>
    private static Dictionary<string, Object[]> atlasDic = new Dictionary<string, Object[]>();

    /// <summary>
    /// 加载图集图片
    /// </summary>
    /// <typeparam name="T">类型：Sprite Texture</typeparam>
    /// <param name="atlasPath">图集路径</param>
    /// <param name="name">图片名字</param>
    /// <returns>图片</returns>
    public static T LoadAtlas<T>(string atlasPath, string name) where T : Object
    {
        T atlas = null;

        if (atlasDic.ContainsKey(atlasPath))
        {
            atlas = FindSprite<T>(atlasPath, name);
        }
        else
        {
            Object[] spriteList = Resources.LoadAll(atlasPath);
            atlasDic.Add(atlasPath, spriteList);
            atlas = FindSprite<T>(atlasPath, name);
        }

        return atlas;
    }

    /// <summary>
    /// 查找图集内的小图 Sprite图片
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="atlasPath">图集路径</param>
    /// <param name="spriteName">图片名字</param>
    /// <returns>Sprite图片</returns>
    private static T FindSprite<T>(string atlasPath, string spriteName) where T : Object
    {
        Object[] spriteList = atlasDic[atlasPath];
        for (int i = 0; i < spriteList.Length; i++)
        {
            Object sp = spriteList[i];
            if (sp.name == spriteName)
            {
                return sp as T;
            }
        }

        Debug.LogError($"没有在{atlasPath}图集里找到名为{spriteName}的图片，请检查!!");
        return null;
    }
}