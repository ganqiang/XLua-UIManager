--- UI管理器
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/8 15:38
---

--- @class UIManager
local UIManager = {}
_G.UIManager = UIManager

local UILayerManager = require("UILayerManager")

--- @field WindowType Enum UI类型
local WindowType = {
    Error = -1, -- 当出现界面错误时，窗口类型为它
    Main = 1, -- 主界面常显（会一直显示，不会隐藏或销毁）
    Window = 2, -- 全屏类型窗口
    Pop = 3, -- 弹窗类型窗口
    Other = 99, -- 暂定其他类型窗口
}
UIManager.WindowType = WindowType

--- @field curShowUIName string 当前显示的UI名字
local curShowUIName = nil

--- @field uiWindowList table 存放所有UI Window名字的队列信息（UIManager显示队列）  格式为：{ { xxx }, { xxx }, ... }
local uiWindowList = {}

--- @class uiClassList 存放所有UI类的队列信息
--- @field UIName string UI名字
--- @field UIClass BaseWindow UI类
local uiClassList = {}

--- AddUIWindow 存储UI名字队列
--- @param uiName string UI名字
local AddUIWindow = function(uiName)
    for index = #uiWindowList, 1, -1 do
        local info = uiWindowList[index]
        if (info) then
            local windowName = info[1]
            if (windowName == uiName) then
                -- 如果已有，就不往里添了
                return
            end
        end
    end

    curShowUIName = uiName
    uiWindowList[#uiWindowList + 1] = { uiName }
end

--- RemoveUIWindow 从UI名字队列移除UI
--- @param uiName string UI名字
local RemoveUIWindow = function(uiName)
    for index = #uiWindowList, 1, -1 do
        local uiWindowInfo = uiWindowList[index]
        if (uiWindowInfo) then
            local windowName = uiWindowInfo[1]
            if (windowName == uiName) then
                local uiClass = UIManager.GetUIClassByName(windowName)
                local windowType = uiClass:GetWindowType()
                if (windowType ~= WindowType.Main) then
                    -- 当要关闭的UI类型不为Main常显类型的窗口，那么才移除
                    --uiWindowList[index] = nil
                    table.remove(uiWindowList, index)
                    if (not HUtils.IsEmpty(uiWindowList)) then
                        curShowUIName = uiWindowList[#uiWindowList][1]
                    else
                        curShowUIName = nil
                    end
                end
                break
            end
        end
    end
end

--- LoadPrefab 加载预制体
--- @param parent Transform 父物体
--- @param prefabPath string 预制体路径
--- @param callback function 加载完成回调
function UIManager.LoadPrefab(parent, prefabPath, callback)
    ResourceManager.LoadPrefab(parent or UIRoot, prefabPath, callback)
    --ResourceManager.LoadPrefabSync(parent or UIRoot, prefabPath, callback)
end

--- OpenUI 打开UI
--- @param uiName string UI名字
--- @param args table 打开界面携带传入的参数
--- @param callback function 成功打开界面后的回调
function UIManager.OpenUI(uiName, args, callback)
    --- @type BaseWindow
    local lastUIClass = UIManager.GetUIClassByName(curShowUIName)

    --- @type BaseWindow
    local uiClass = UIManager.GetUIClassOrCreate(uiName, true)
    if (uiClass) then
        UILayerManager.AddUILayer(uiClass)
        uiClass:Show(args, function ()
            if (callback) then
                callback()
            end

            -- 当界面打开后，需要检查上一个界面是否存在，需要对上一个界面进行隐藏或销毁等处理
            if (lastUIClass) then
                local windowType = uiClass:GetWindowType()
                if (windowType == UIManager.WindowType.Main) then
                    -- 如果当前界面类型是主界面常显类型，那么就不用管，因为该窗口类型是常显的
                elseif (windowType == UIManager.WindowType.Window) then
                    -- 如果当前界面是全屏的，那么就需要把上一个全屏界面隐藏
                    -- 隐藏上一个窗口时，需要判断上一个窗口是否是常显类型窗口（常显类型窗口是不会隐藏的）
                    local lastWindowType = lastUIClass:GetWindowType()
                    if (lastWindowType ~= UIManager.WindowType.Main) then
                        -- 如果不是常显类型窗口，那么就需要执行隐藏操作
                        lastUIClass:Hide()
                    end
                elseif (windowType == UIManager.WindowType.Pop) then
                    -- 如果当前界面是弹窗类型的，那么也不用管，因为弹窗的时候需要显示上一个界面
                end
            end
        end)
    end

    -- 这里把要显示的UI添加进UI名字队列
    AddUIWindow(uiName)
end

--- CloseUI 关闭UI
--- @param uiName string UI名字
function UIManager.CloseUI(uiName)
    RemoveUIWindow(uiName)

    if (curShowUIName) then
        --- @type BaseWindow
        local lastUIClass = UIManager.GetUIClassByName(curShowUIName)
        -- 当关闭一个界面之前，需要先把UI队列里的上一个界面显示出来（防止出现黑屏闪屏效果）
        if (lastUIClass) then
            lastUIClass:Show()
        end
    end

    --- @type BaseWindow
    local uiClass = UIManager.GetUIClassByName(uiName)
    if (uiClass) then
        local windowType = uiClass:GetWindowType()
        -- 关闭窗口前，先判断要关闭的窗口类型是否为Main常显类型窗口，只有不属于常显类型窗口才关闭
        if (windowType ~= WindowType.Main) then
            -- 执行关闭当前UI与重计算UI层级操作
            uiClass:Close()
            UILayerManager.RemoveUILayer(uiClass)
        else
            LogError(string.format("想要关闭%s窗口，该窗口为常显类型的窗口，若确实需要关闭，请改变%s的窗口类型（self.windowType）!!", uiName, uiName))
        end
    end

    -- 关闭UI后需要把队列中存储的相关UI一并移除
    UIManager.RemoveUIClassByName(uiName)
end

--- GetUIClassOrCreate 获得或创建UI类
--- @param uiName string UI名字
--- @param isCreate boolean 当不存在的时候，是否创建？ true ：是
--- @return BaseWindow UI类
function UIManager.GetUIClassOrCreate(uiName, isCreate)
    --- @type uiClassList
    local uiClass = UIManager.GetUIClassByName(uiName)
    if (not uiClass and isCreate) then
        -- 还没有类，开始实例化
        uiClass = Class.New(uiName)
        uiClassList[#uiClassList + 1] = { UIName = uiName, UIClass = uiClass }
    end

    return uiClass
end

--- GetUIClassInfoByName 根据UI名字获取UI队列信息
--- @param uiName string UI名字
--- @return boolean 该UI是否存在于UI队列中？ true ：是
--- @return number 该UI存在于UI队列中的位置（索引）
--- @return uiClassList 该UI存储在UI队列中的信息  格式为：{ UIName = xxx, UIClass = xxx }
function UIManager.GetUIClassInfoByName(uiName)
    for index = 1, #uiClassList do
        local uiClassInfo = uiClassList[index]
        if (uiClassInfo.UIName == uiName) then
            return true, index, uiClassInfo
        end
    end

    return false, -1, {}
end

--- GetUIClassByName 根据UI名字获取UI类
--- @param uiName string UI名字
--- @return BaseWindow UI类
function UIManager.GetUIClassByName(uiName)
    --- @type uiClassList
    local _, _, uiClassInfo = UIManager.GetUIClassInfoByName(uiName)
    return uiClassInfo.UIClass
end

--- RemoveUIClassByName 根据UI名字移除UI队列中的UI类
--- @param uiName string UI名字
function UIManager.RemoveUIClassByName(uiName)
    local isExist, uiIndex, uiClassInfo = UIManager.GetUIClassInfoByName(uiName)
    if (isExist) then
        --- @type BaseWindow
        local uiClass = uiClassInfo.UIClass
        local windowType = uiClass:GetWindowType()
        -- 只有不属于常显类型窗口才把该UI移除UI队列
        if (windowType ~= WindowType.Main) then
            uiClassList[uiIndex] = nil
        end
    end
end

--- UIShowed UI已展示完成
--- @param uiClass BaseWindow 已成功打开的UI
function UIManager.UIShowed(uiClass)
    -- 目的：处理界面A --> B --> C 然后在界面C又想打开A的情况，此时就直接关闭界面B与C，直接回到界面A   类似：X --> A --> B --> C 此时再打开 A   就会变成： X --> A
    local uiName = uiClass:ClassName()
    local isExist, uiIndex = UIManager.GetUIClassInfoByName(uiName)
    if (isExist) then
        for index = #uiClassList, uiIndex + 1, -1 do
            --- @type uiClassList
            local uiClassInfo = uiClassList[index]
            local existUIClass = uiClassInfo.UIClass
            if (existUIClass) then
                -- 从UI队列末尾开始向前依次移除与关闭销毁界面
                RemoveUIWindow(existUIClass:ClassName())
                UIManager.RemoveUIClassByName(existUIClass:ClassName())
                existUIClass:Close()
            end
        end
    end
end


--- GetCurShowUILayerValue 获得当前UI的层级数值
--- @return number 当前UI层级数值
function UIManager.GetCurShowUILayerValue()
    local layerValue = UILayerManager.GetCurUILayerValue()
    return layerValue
end

--- GetUIWindowList 获得UI队列信息
--- @return table 存储的当前已打开的所有UI名字队列信息
function UIManager.GetUIWindowList()
    return uiWindowList
end

--- Clear 退出游戏时，清理UIManager所有数据
function UIManager.Clear()
    -- 清理当前UI
    curShowUIName = nil

    -- 清理保存的UI名字队列
    uiWindowList = {}

    -- 清理所有未被关闭的UI类
    for index = #uiClassList, 1, -1 do
        --- @type uiClassList
        local uiClassInfo = uiClassList[index]
        local uiClass = uiClassInfo.UIClass
        if (uiClass) then
            uiClass:Close()
        end
    end
    uiClassList = {}

    -- 清理UI层级数据
    UILayerManager.Clear()
end

return UIManager