--- 此框架所有对象的基类
--- 主要用于封装一些通用属性与方法
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/18 17:43
---

--- @class Object
--- @field protected __className string 当前类名
--- @field protected __super Object 当前类的父类
local Object = Class.CreateClass("Object")

function Object:New()
end

--- ClassName 获得当前类名
--- @return string 当前类类名
function Object:ClassName()
    return self.__className
end

--- Super 获得父类
--- @return Object 父类
function Object:Super()
    return self.__super
end

return Object