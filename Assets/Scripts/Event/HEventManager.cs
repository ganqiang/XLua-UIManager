// ================================================================================
//
//      作 者  :   G Q
//      类 名  :   HEventManager
//      时 间  :   2022年11月17日 11:15:27
//      目 的  :   用于驱动此框架的事件管理系统
//
// ================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件管理器（目前只支持不带参，带一个参的事件）
/// </summary>
public class HEventManager
{
    /// <summary>
    /// 存放所有事件的列表
    /// </summary>
    private static Dictionary<HEventType, Delegate> allEventDic = new Dictionary<HEventType, Delegate>();

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType">事件码</param>
    /// <param name="callback">回调</param>
    public static void AddEvent(HEventType eventType, Action callback)
    {
        if (!allEventDic.ContainsKey(eventType))
        {
            // 如果不存在，就添加
            allEventDic.Add(eventType, null);
        }

        Delegate handle = allEventDic[eventType];
        if (handle != null && handle.GetType() != callback.GetType())
        {
            throw new Exception($"想要添加不同类型的委托，当前委托类型是：{handle.GetType()}，想要添加的委托类型为：{callback.GetType()}");
        }

        // 添加委托
        allEventDic[eventType] = (Action)allEventDic[eventType] + callback;
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType">事件码</param>
    /// <param name="callback">回调</param>
    public static void RemoveEvent(HEventType eventType, Action callback)
    {
        if (!allEventDic.ContainsKey(eventType))
        {
            // 当前事件列表里没有对应的事件码的事件
            return;
        }

        Delegate handle = allEventDic[eventType];
        if (handle == null)
        {
            // 当前事件列表里没有事件码所对应的委托
            return;
        }

        if (handle.GetType() != callback.GetType())
        {
            throw new Exception($"想要移除不同类型的委托，当前委托类型是：{handle.GetType()}，想要移除的委托类型为：{callback.GetType()}");
        }

        // 移除委托
        allEventDic[eventType] = (Action)allEventDic[eventType] - callback;
    }

    /// <summary>
    /// 派遣事件
    /// </summary>
    /// <param name="eventType">事件码</param>
    public static void DispatchEvent(HEventType eventType)
    {
        if (allEventDic.TryGetValue(eventType, out Delegate handle))
        {
            Action callback = handle as Action;
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 注册包涵一个参数的事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventType">事件码</param>
    /// <param name="callback">回调</param>
    public static void AddEvent<T>(HEventType eventType, Action<T> callback)
    {
        if (!allEventDic.ContainsKey(eventType))
        {
            // 如果不存在，就添加
            allEventDic.Add(eventType, null);
        }

        Delegate handle = allEventDic[eventType];
        if (handle != null && handle.GetType() != callback.GetType())
        {
            throw new Exception($"想要添加不同类型的委托，当前委托类型是：{handle.GetType()}，想要添加的委托类型为：{callback.GetType()}");
        }

        // 添加委托
        allEventDic[eventType] = (Action<T>)allEventDic[eventType] + callback;
    }

    /// <summary>
    /// 移除包涵一个参数的事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventType">事件码</param>
    /// <param name="callback">回调</param>
    public static void RemoveEvent<T>(HEventType eventType, Action<T> callback)
    {
        if (!allEventDic.ContainsKey(eventType))
        {
            // 当前事件列表里没有对应的事件码的事件
            return;
        }

        Delegate handle = allEventDic[eventType];
        if (handle == null)
        {
            // 当前事件列表里没有事件码所对应的委托
            return;
        }

        if (handle.GetType() != callback.GetType())
        {
            throw new Exception($"想要移除不同类型的委托，当前委托类型是：{handle.GetType()}，想要移除的委托类型为：{callback.GetType()}");
        }

        // 移除委托
        allEventDic[eventType] = (Action<T>)allEventDic[eventType] - callback;
    }

    /// <summary>
    /// 派遣包涵一个参数的事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventType">事件码</param>
    /// <param name="args">回调</param>
    public static void DispatchEvent<T>(HEventType eventType, T args)
    {
        if (allEventDic.TryGetValue(eventType, out Delegate handle))
        {
            Action<T> callback = handle as Action<T>;
            callback?.Invoke(args);
        }
    }
}