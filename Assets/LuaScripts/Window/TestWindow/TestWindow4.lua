---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/10 17:14
---

--- @class TestWindow4 : BaseWindow
local TestWindow4 = Class.CreateClass("TestWindow4", "BaseWindow")

function TestWindow4:New()
    self.windowType = UIManager.WindowType.Window
    self.prefabPath = "Window/TestWindow"
end

function TestWindow4:OnInit()
    self:Super().OnInit(self)
end

function TestWindow4:InitEvent()
    self.btnOpen = self.gameObject.transform:Find("btnOpen"):GetComponent(typeof(Button))
    self.btnClose = self.gameObject.transform:Find("btnClose"):GetComponent(typeof(Button))

    self:AddListener(self.btnOpen.onClick, self.ClickBtnOpen)
    self:AddListener(self.btnClose.onClick, self.ClickBtnClose)
end

function TestWindow4:OnShowing()
    self:InitEvent()
    self:Super().OnShowing(self)
end

function TestWindow4:ClickBtnOpen()
    UIManager.OpenUI("TestWindow2")
end

function TestWindow4:ClickBtnClose()
    UIManager.CloseUI("TestWindow4")
end

function TestWindow4:OnClose()
    self:Super().OnClose(self)
end

return TestWindow4