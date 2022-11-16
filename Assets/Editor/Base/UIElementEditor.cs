// ================================================================================
//
//      作者  :   G Q
//      时间  :   2022年11月16日 12:22:42
//      类名  :   UIElementEditor
//      目的  :   使UIElement使用起来更加便携简单
//
// ================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// UIElement的编辑器代码
/// </summary>
[CustomEditor(typeof(UIElement))]
public class UIElementEditor : Editor
{
    /// <summary>
    /// 可视化排序列表
    /// </summary>
    private ReorderableList reorderableList;
    /// <summary>
    /// UIElement序列化属性
    /// </summary>
    private SerializedProperty uiElementProperty;

    /// <summary>
    /// 右键添加组件到UIElement上
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("CONTEXT/Component/AddDataElement", false, 10)]
    private static void AddComponent(MenuCommand menuCommand)
    {
        Object component = menuCommand.context;
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            return;
        }

        UIElement[] elementList = go.transform.GetComponentsInParent<UIElement>(true);
        UIElement uiElement = null;
        if (elementList != null && elementList.Length > 0)
        {
            for (int i = 0; i < elementList.Length; i++)
            {
                if (elementList[i] != go)
                {
                    // 查找当前选中的物体的父物体身上挂载的 UIElement
                    uiElement = elementList[i];
                    break;
                }
            }
        }

        if (uiElement == null)
        {
            Debug.LogError($"执行AddDataElement操作失败，添加{component}到UIElement失败，未找到父物体上挂载的UIElement组件，请检查!!");
            return;
        }

        if (uiElement.dataElementList == null)
        {
            uiElement.dataElementList = new List<DataElement>();
        }

        foreach (var item in uiElement.dataElementList)
        {
            if (item.obj == component)
            {
                item.name = component.name;
                SetDirty(uiElement);
            }
        }

        // 添加组件到UIElement
        uiElement.AddObject(component);
        SetDirty(uiElement);
    }

    /// <summary>
    /// 标脏
    /// </summary>
    /// <param name="target"></param>
    private static void SetDirty(Object target)
    {
        EditorUtility.SetDirty(target);
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }

    private void OnEnable()
    {
        // 获取 UIElement 序列化数据
        uiElementProperty = serializedObject.FindProperty("dataElementList");
        // 加载 ReorderableList
        reorderableList = new ReorderableList(serializedObject, uiElementProperty, true, false, false, false);
        // 设置 ReorderableList 每行高度
        reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 2;
        // 绘制 UIElement 数据内容
        reorderableList.drawElementCallback = OnDrawElementCallback;
        // 绘制 UIElement 标题
        reorderableList.drawHeaderCallback = OnDrawHeaderCallback;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        serializedObject.Update();
        // 设置UIElement拖动区域与 组件的拖动反馈相关
        DoReorderableListList();
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("清理所有"))
        {
            UIElement uiElement = target as UIElement;
            if (uiElement != null)
            {
                uiElement.dataElementList.Clear();
            }
        }
    }

    /// <summary>
    /// 绘制内容
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="selected"></param>
    /// <param name="focused"></param>
    private void OnDrawElementCallback(Rect rect, int index, bool selected, bool focused)
    {
        UIElement uiElement = target as UIElement;
        SerializedProperty element = uiElementProperty.GetArrayElementAtIndex(index);
        Rect rectInfo = new Rect(rect)
        {
            height = rect.height - 1,
            width = rect.width - 10,
            y = rect.y + 1
        };
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(rectInfo, element);
        if (EditorGUI.EndChangeCheck())
        {
            SetDirty(target);
        }

        var texture = new Texture2D(10, 2, TextureFormat.ARGB32, false, true);
        Rect pos = new Rect(rect)
        {
            x = rectInfo.x - 20 + rectInfo.width,
            y = rect.y - 8 + (rect.height - texture.height) * 0.5f,
            width = 32,
            height = 18
        };

        if (GUI.Button(pos, texture))
        {
            uiElement.RemoveAt(index);
            SetDirty(target);
        }
    }
    /// <summary>
    /// 绘制标题
    /// </summary>
    /// <param name="rect"></param>
    private void OnDrawHeaderCallback(Rect rect)
    {
        GUI.Label(rect, "UIElements");
    }

    /// <summary>
    /// 设置UIElement拖动区域与 组件的拖动反馈相关
    /// </summary>
    private void DoReorderableListList()
    {
        //reorderableList.DoLayoutList();
        float boxHeight = 17 + (reorderableList.count > 0 ? ((reorderableList.count + 1) * reorderableList.elementHeight) : 25);
        // 拖动区域
        Rect dragRect = GUILayoutUtility.GetRect(0f, boxHeight, GUILayout.ExpandWidth(true));
        GUI.Box(dragRect, "");
        reorderableList.DoList(dragRect);

        // 添加拖动事件
        AddDragEvent(dragRect);
    }

    /// <summary>
    /// 添加拖动反馈事件
    /// </summary>
    /// <param name="dragRect"></param>
    private void AddDragEvent(Rect dragRect)
    {
        switch (Event.current.type)
        {
            case EventType.DragPerform:
                //Debug.LogError("EventType.DragPerform");
                break;
            case EventType.DragExited:
                //Debug.LogError("拖动结束");
                OnDragEnd(Event.current, dragRect);
                break;
            case EventType.DragUpdated:
                //Debug.LogError("拖动中");
                break;
            case EventType.MouseDrag:
                //Debug.LogError("EventType.MouseDrag");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 组件拖动结束事件
    /// </summary>
    /// <param name="e"></param>
    /// <param name="dragRect"></param>
    private void OnDragEnd(Event e, Rect dragRect)
    {
        UIElement uiElement = target as UIElement;
        if (uiElement)
        {
            if (dragRect.Contains(e.mousePosition))
            {
                var info = DragAndDrop.objectReferences;
                var component = info[0];
                if (component)
                {
                    // 有东西拖入到UIElement
                    uiElement.AddObject(component);
                    SetDirty(uiElement);
                }
            }
        }
    }
}

/// <summary>
/// 绘制DataElement
/// </summary>
[CustomPropertyDrawer(typeof(DataElement))]
public class UIElementDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            float elementWidth = position.width;

            var nameRect = new Rect(position)
            {
                width = elementWidth * 0.4f,
            };
            var objRect = new Rect(position)
            {
                x = nameRect.x + nameRect.width,
                width = elementWidth * 0.55f,
            };

            // 获取序列化中的值
            var propName = property.FindPropertyRelative("name");
            var propObj = property.FindPropertyRelative("obj");

            // 开始绘制DataElement
            propName.stringValue = EditorGUI.TextField(nameRect, propName.stringValue);
            EditorGUI.BeginChangeCheck();
            propObj.objectReferenceValue = EditorGUI.ObjectField(objRect, propObj.objectReferenceValue, typeof(Object), true);
            EditorGUI.EndChangeCheck();
        }
    }
}