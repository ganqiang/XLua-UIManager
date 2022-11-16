// ================================================================================
//
//      作者  :   G Q
//      时间  :   2022年10月09日 14:47:31
//      类名  :   SpritesManager
//      目的  :   XLua启动器，从这里开始进行与Lua端的交互的入口
//
// ================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

/// <summary>
/// XLua CS端启动器
/// </summary>
public class XLuaLauncher : MonoBehaviour
{
    [Tooltip("是否打印杀掉程序后有哪些未释放的委托")]
    public bool isPrintNotReleseDelegate = false;

    private LuaEnv luaEnv;

    private Dictionary<string, string> luaScriptsDic;

    private void Awake()
    {
        if (Application.isEditor)
        {
            InitLuaScripts();
        }

        luaEnv = new LuaEnv();

        if (isPrintNotReleseDelegate)
        {
            luaEnv.DoString("util =  require('xlua.util')");
        }

        luaEnv.AddLoader((ref string fileName) =>
        {
            if (fileName.EndsWith(".lua"))
            {
                fileName = fileName.Replace(".lua", "");
            }

            byte[] value = null;

            string luaPath = "";
            if (!luaScriptsDic.TryGetValue(fileName, out luaPath))
            {
                return null;
            }

            if (!File.Exists(luaPath))
            {
                throw new ArgumentNullException($"没有找到{luaPath}该文件!!");
            }
            else
            {
                value = File.ReadAllBytes(luaPath);
                if (value.Length == 0)
                {
                    Debug.LogError($"{luaPath}加载失败，内容为空!!");
                }
            }

            return value;
        });

        // 此处开始进入Lua端
        luaEnv.DoString("require('Main')");
        luaEnv.DoString("OnAwake()");
    }

    private void InitLuaScripts()
    {
        if (luaScriptsDic == null)
        {
            luaScriptsDic = new Dictionary<string, string>();

            // TODO GQ 后续在此需要做平台判断，来决定如何加载Lua文件
            string luaFilePath = Path.Combine(Application.dataPath, "LuaScripts");
            string[] luaPathList = Directory.GetFiles(luaFilePath, "*.lua", SearchOption.AllDirectories);
            for (int i = 0; i < luaPathList.Length; i++)
            {
                string luaPath = luaPathList[i];
                string fileName = Path.GetFileName(luaPath);
                fileName = fileName.Replace(".lua", "");
                if (luaScriptsDic.ContainsKey(fileName))
                {
                    Debug.LogError($"重复添加{fileName}文件!!");
                }
                else
                {
                    luaScriptsDic[fileName] = luaPath.Replace("\\", "/");
                }
            }
        }
    }

    private void Start()
    {
        luaEnv.DoString("OnStart()");
    }

    private void Update()
    {
        luaEnv.DoString("OnUpdate()");
    }

    private void FixedUpdate()
    {
        luaEnv.DoString("OnFixedUpdate()");
    }

    private void LateUpdate()
    {
        luaEnv.DoString("OnLateUpdate()");
    }

    private void OnDestroy()
    {
        luaEnv.DoString("OnDestroy()");

        if (isPrintNotReleseDelegate)
        {
            luaEnv.DoString("util.print_func_ref_by_csharp()");
        }

        luaEnv.Dispose();
    }
}