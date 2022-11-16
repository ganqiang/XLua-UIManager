--- 类构造器
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/8 15:18
---

--- @class Class
local Class = {}
_G.Class = Class

local requireClassList = {}

function Class.New(className)
    local class
    if (not requireClassList[className]) then
        class = require(className)
        requireClassList[className] = class
    else
        class = requireClassList[className]
    end

    return class:Ctor()
end

function Class.CreateClass(className, superName)
    local class, super
    super = Class.GetClassByName(superName)
    class = { __cname = className, __super = super }
    if (super) then
        setmetatable(class, { __index = Class.GetFromSuper })
    end
    function class:Ctor(...)
        local instance = {}
        setmetatable(instance, { __index = requireClassList[className], __tostring = class.ToString })
        do
            local create
            create = function(instance, c, ...)
                if c.__super then
                    create(instance, c.__super, ...)
                end
                if rawget(c, "New") then
                    rawget(c, "New")(instance, ...)
                end
            end
            create(instance, requireClassList[className], ...)
        end
        return instance
    end

    requireClassList[className] = class
    return class
end

function Class.GetClassByName(className)
    local uiClass
    if (className) then
        if (not requireClassList[className]) then
            uiClass = require(className)
            requireClassList[className] = uiClass
        else
            uiClass = requireClassList[className]
        end
    end

    return uiClass
end

function Class.GetFromSuper(t, k)
    if t.__super ~= nil then
        -- 拷贝到当前table
        local v = t.__super[k]
        if v then
            rawset(t, k, v)
        end
        return v
    else
        -- 拷贝到当前table
        LogError(string.format("[%s]: 访问一个空值(%s)."), t:ClassName(), k)
        rawset(t, k, false)
        return false
    end
end

return Class