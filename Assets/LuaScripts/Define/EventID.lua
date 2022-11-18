--- 事件ID表
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/17 15:09
---

--- @field EventIDStartID number 事件ID起始值
local EventIDStartID = 100000000

--- AddEventIDCount 事件ID自增函数
local AddEventIDCount = function()
    EventIDStartID = EventIDStartID + 1
    return EventIDStartID
end

local EventID = {
    TestEventId1 = AddEventIDCount(),
    TestEventId2 = AddEventIDCount(),
    TestEventId3 = AddEventIDCount(),
    TestEventId4 = AddEventIDCount(),
}

--- @class EventID
_G.EventID = EventID

return EventID