---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/9 15:38
---

--- @class TestWindow2 : BaseWindow
local TestWindow2 = Class.CreateClass("TestWindow2", "BaseWindow")

function TestWindow2:New()
    self.windowType = UIManager.WindowType.Window
    self.prefabPath = "Window/TestWindow"
end

function TestWindow2:OnInit()
    self:Super().OnInit(self)
end

function TestWindow2:InitEvent()
    self.btnOpen = self.gameObject.transform:Find("btnOpen"):GetComponent(typeof(Button))
    self.btnClose = self.gameObject.transform:Find("btnClose"):GetComponent(typeof(Button))

    self:AddListener(self.btnOpen.onClick, self.ClickBtnOpen)
    self:AddListener(self.btnClose.onClick, self.ClickBtnClose)

    self:AddListener(self.uiElements.btnAddEvent.onClick, function ()
        self:AddEvent(EventID.TestEventId1, self.XXX)

        --EventManager.AddEvent(EventID.TestEventId1, self.XXX, self)
    end)

    self:AddListener(self.uiElements.btnRemoveEvent.onClick, function ()
        self:RemoveEvent(EventID.TestEventId1, self.XXX)

        --EventManager.RemoveEvent(EventID.TestEventId1, self)
    end)
end

function TestWindow2:XXX(args)
    local a = self
    LogError(args)
end

function TestWindow2:OnShowing()
    self:InitEvent()
    self:Super().OnShowing(self)
end

function TestWindow2:ClickBtnOpen()
    UIManager.OpenUI("TestWindow3")
end

function TestWindow2:ClickBtnClose()
    --UIManager.CloseUI("TestWindow2")
    self:CloseSelf()
end

function TestWindow2:OnClose()
    self:Super().OnClose(self)
end

return TestWindow2