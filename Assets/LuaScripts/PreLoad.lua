--- 预加载模块
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/9 10:43
---

require("UnityEngineDefine")
require("LuaEx")
require("Class")
require("UIManager")
require("ResourceManager")

-- UI父节点
_G.UIRoot = GameObject.Find("UICanvas").transform
_G.Destroy = UnityEngine.Object.Destroy