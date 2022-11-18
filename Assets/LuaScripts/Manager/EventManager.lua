--- 用于驱动此框架的初版Lua端事件管理器，后续可能会进行修改完善
--- 该Lua事件管理器基于一对多（一个总事件列表对应多个事件ID列表，其里面的单个事件ID对应多个类）的方式开发完善的
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/17 14:23
---

--- @class EventManager
local EventManager = {}
_G.EventManager = EventManager

--[[
AllEventList =
{
    eventId1 =
    {
        class1 = { uiClass = owner, handle = callback },
        class2 = { uiClass = owner, handle = callback },
        ...
    },
    eventId2 =
    {
        class1 = { uiClass = owner, handle = callback },
        class2 = { uiClass = owner, handle = callback },
        ...
    },
    ...
}
--]]
--- @class AllEventList 存放所有事件的列表，格式如上
local AllEventList = {}

--- AddEvent 监听事件
--- @param eventID EventID 事件ID
--- @param callback function 回调（写法尽量以 self:AddEvent(eventId, self.XXX) 的方式，如果用匿名函数，那么匿名函数的第一个参数为self自身）
--- @param owner Object 此事件所属类（如果该类继承自BaseEvent，那么该值不用传）
function EventManager.AddEvent(eventID, callback, owner)
    local eventInfoList = AllEventList[eventID]
    if (not eventInfoList) then
        -- 还未在总事件表里注册此ID的事件，故新建
        eventInfoList = {}
        AllEventList[eventID] = eventInfoList
    end

    if (not eventInfoList[owner:ClassName()]) then
        -- 此ID还未在当前类（owner）里注册，故新建
        eventInfoList[owner:ClassName()] = { uiClass = owner, handle = callback }
    else
        -- TODO GQ 此处或许需要判断同一EventID重复注册不同回调的问题
        LogError(string.format("在%s类重复注册事件ID为%s的事件", owner:ClassName(), eventID))
    end
end

--- RemoveEvent 移除事件
--- @param eventID EventID 事件ID
--- @param callback function 回调（写法尽量以 self:AddEvent(eventId, self.XXX) 的方式，如果用匿名函数，那么匿名函数的第一个参数为self自身）
--- @param owner Class 此事件所属类（如果该类继承自BaseEvent，那么该值不用传）
function EventManager.RemoveEvent(eventID, callback, owner)
    local eventInfoList = AllEventList[eventID]
    if (HUtils.IsEmpty(eventInfoList)) then
        return
    end

    if (not owner) then
        eventInfoList = nil
        return
    end

    if (not eventInfoList[owner:ClassName()]) then
        return
    end

    eventInfoList[owner:ClassName()] = nil
end

--- DispatchEvent 派遣事件
--- @param eventID EventID 事件ID
--- @param args any 派遣事件时所携带的参数（可不传）
function EventManager.DispatchEvent(eventID, args)
    local eventInfoList = AllEventList[eventID]
    if (HUtils.IsEmpty(eventInfoList)) then
        return
    end

    for className, eventInfo in pairs(eventInfoList) do
        local uiClass = eventInfo.uiClass
        local handle = eventInfo.handle
        if (uiClass) then
            handle(uiClass, args)
        end
    end
end

--- ClearEvent 清理事件数据
function EventManager.ClearEvent()
    AllEventList = {}
end

return EventManager