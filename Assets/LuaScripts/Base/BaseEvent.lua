--- 整个框架的事件基类
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/10 10:29
---

--- @class BaseEvent : Object
local BaseEvent = Class.CreateClass("BaseEvent", "Object")

function BaseEvent:New()
    -- 存储的所有Unity事件（按钮等）注册列表
    self.unityEventList = {}
end

--- AddListener 监听按钮事件
--- @param event Event 按钮onClick事件
--- @param callback function 按钮点击事件回调
function BaseEvent:AddListener(event, callback)
    if (not self.unityEventList[event]) then
        self.unityEventList[event]= {}
    end

    event:RemoveAllListeners()
    event:AddListener(function ()
        if (callback) then
            callback(self)
        end
    end)
end

--- AddEvent 注册事件
--- @param eventID EventID 事件ID
--- @param callback function 回调（写法尽量以 self:AddEvent(eventId, self.XXX) 的方式，如果用匿名函数，那么匿名函数的第一个参数为self自身）
function BaseEvent:AddEvent(eventID, callback)
    EventManager.AddEvent(eventID, callback, self)
end

--- RemoveEvent 移除事件
--- @param eventID EventID 事件ID
--- @param callback function 回调（写法尽量以 self:RemoveEvent(eventId, self.XXX) 的方式，如果用匿名函数，那么匿名函数的第一个参数为self自身）
function BaseEvent:RemoveEvent(eventID, callback)
    EventManager.RemoveEvent(eventID, callback, self)
end

--- ClearEvent 清理事件
function BaseEvent:ClearUnityEvent()
    for event, _ in pairs(self.unityEventList) do
        event:RemoveAllListeners()
        event:Invoke()
    end
    self.unityEventList = {}
end

function BaseEvent:ClearEvent()
    -- TODO GQ 这里需要把通过 self:AddEvent 注册的事件记录下来，当界面关闭（生命周期结束）的时候，需要清理事件
end

return BaseEvent