// ================================================================================
//
//      作者  :   G Q
//      时间  :   2022年11月16日 11:47:33
//      类名  :   UIElement
//      目的  :   用于可以在Lua端直接 .出组件
//                  避免在Lua端出现大量 GameObject.Find, transform.Find 等查找组件代码引起的性能消耗
//
// ================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using Object = UnityEngine.Object;

/// <summary>
/// 目的：用于可以在Lua端直接 .出组件
/// 原因：避免Lua端出现大量 GameObject.Find, transform.Find 等大量查找组件代码引起性能消耗
/// </summary>
[DisallowMultipleComponent]
[LuaCallCSharp]
[Serializable]
public class UIElement : MonoBehaviour
{
    /// <summary>
    /// UIElement 列表
    /// </summary>
    [SerializeField]
    public List<DataElement> dataElementList;

    /// <summary>
    /// 检查某个组件是否已存在？
    /// </summary>
    /// <param name="obj">组件</param>
    /// <returns>是否已存在</returns>
    public bool CheckIsExist(Object obj)
    {
        if (dataElementList != null && obj != null)
        {
            foreach (var item in dataElementList)
            {
                if (obj == item.obj)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 往UIElement上添加组件
    /// </summary>
    /// <param name="data"></param>
    private void Add(DataElement data)
    {
        if (dataElementList == null)
        {
            dataElementList = new List<DataElement>();
        }
        dataElementList.Add(data);
    }

    /// <summary>
    /// 往UIElement上添加组件
    /// </summary>
    /// <param name="obj">添加的组件</param>
    public void AddObject(Object obj)
    {
        if (obj == null)
        {
            Debug.LogError($"添加失败，要添加的对象：{obj}为空!!");
            return;
        }

        if (CheckIsExist(obj))
        {
            Debug.LogError($"添加失败，要添加的对象：{obj}已存在!!");
            return;
        }

        Add(new DataElement(obj));
    }

    /// <summary>
    /// 通过名字移除UIElement上已添加的组件
    /// </summary>
    /// <param name="name">名字</param>
    public void Remove(string name)
    {
        for (int i = 0; i < dataElementList.Count; i++)
        {
            var item = dataElementList[i];
            if (item.name == name)
            {
                dataElementList.Remove(item);
                break;
            }
        }
    }

    /// <summary>
    /// 通过索引移除UIElement上已添加的组件
    /// </summary>
    /// <param name="index">索引</param>
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < dataElementList.Count)
        {
            dataElementList.RemoveAt(index);
        }
    }

    /// <summary>
    /// 绑定
    /// </summary>
    /// <param name="t">luaTable</param>
    private void Bind(LuaTable t)
    {
        for (int i = 0; i < dataElementList.Count; i++)
        {
            DataElement dataElement = dataElementList[i];
            if (dataElement.obj is UIElement)
            {
                LuaTable tChild = t.GetOrAddTable(dataElement.name);
                UIElement dataChild = dataElement.obj as UIElement;
                dataChild.Bind(tChild);
            }
            else
            {
                t.Set(dataElement.name, dataElement.obj);
            }
        }
    }

    /// <summary>
    /// 绑定（Lua端绑定CS端的UIElement，便于在Lua端直接可以通过名字获取到组件）
    /// </summary>
    /// <param name="obj">挂载UIElement的游戏物体</param>
    /// <param name="t">luaTable</param>
    public static void TryBind(Object obj, LuaTable t)
    {
        GameObject gameObject = obj as GameObject;
        if (gameObject != null)
        {
            UIElement dataElement = gameObject.GetComponent<UIElement>();
            if (dataElement != null)
            {
                dataElement.Bind(t);
            }
            return;
        }

        //Debug.LogError($"UIElement绑定失败，{gameObject}为空!!");
    }
}

/// <summary>
/// UIElement 单个元素结构
/// </summary>
[LuaCallCSharp]
[Serializable]
public class DataElement
{
    /// <summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// 游戏物体组件
    /// </summary>
    public Object obj;

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="obj">组件</param>
    /// <param name="name">名字</param>
    public DataElement(Object obj, string name = null)
    {
        this.obj = obj;
        this.name = string.IsNullOrEmpty(name) ? obj.name : name;
    }
}