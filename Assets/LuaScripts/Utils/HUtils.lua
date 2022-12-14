--- 工具类
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by gan.qiang.
--- DateTime: 2022/11/8 15:21
---

local HUtils = {}
_G.HUtils = HUtils

local function byte2string(bytes)
    local list = {}
    local count = #bytes
    local isByte = false
    for i = 1, count do
        local byte = string.byte(bytes, i)
        list[#list + 1] = byte
        if byte == 0 then
            isByte = true
        end
    end
    if not isByte then
        return string.format('"%s"', bytes)
    end
    return string.format('<%s>', table.concat(list, ','))
end

local dumpTableList = {}
---@type function            @类似dumpTable，严格遍历顺序，保证同一个表每次输出相同
---@param data table        @表示要输出的数据
---@param dup_table table    @复用表格
---@param lastCount number    @用于格式控制，用户请勿使用该变量
---@return string            @表格序列化字符串
local function realDumpTableEx(data, dup_table, lastCount)
    local function tab(count)
        local tabs = {}
        for i = 1, count do
            tabs[i] = "\t"
        end
        return table.concat(tabs)
    end
    local pt = {}
    if type(data) ~= "table" then
        if type(data) == "string" then
            pt[#pt + 1] = byte2string(data)
        else
            pt[#pt + 1] = tostring(data)
        end
    elseif dup_table and dup_table[data] then
        pt[#pt + 1] = dup_table[data]
    elseif not dumpTableList[data] then
        dumpTableList[data] = true
        local count = lastCount or 0
        count = count + 1
        --行前空格数量
        local tabs = tab(count)
        pt[#pt + 1] = "{\n"
        --数组部分
        local len = #data
        for i = 1, len do
            pt[#pt + 1] = tabs
            pt[#pt + 1] = realDumpTableEx(data[i], dup_table, count)
            pt[#pt + 1] = ",\n"
        end
        --hash部分也分数字键和string键
        local keys_num = {}
        local keys_str = {}
        for key, _ in pairs(data) do
            local tkey = type(key)
            if tkey == "number" then
                if key > len or key <= 0 then
                    keys_num[#keys_num + 1] = key
                end
            else
                keys_str[#keys_str + 1] = tostring(key)
            end
        end
        table.sort(keys_num)
        table.sort(keys_str)
        for i = 1, #keys_num do
            pt[#pt + 1] = tabs
            pt[#pt + 1] = "[" .. keys_num[i] .. "] = "
            pt[#pt + 1] = realDumpTableEx(data[keys_num[i]], dup_table, count)
            pt[#pt + 1] = ",\n"
        end
        for i = 1, #keys_str do
            pt[#pt + 1] = tabs
            pt[#pt + 1] = "[\"" .. keys_str[i] .. "\"] = "
            pt[#pt + 1] = realDumpTableEx(data[keys_str[i]], dup_table, count)
            pt[#pt + 1] = ",\n"
        end

        pt[#pt + 1] = tab(count - 1)
        pt[#pt + 1] = "}"
    end
    --Format
    if not lastCount then
        pt[#pt + 1] = ("\n")
    end

    return table.concat(pt)
end

---@type function dumptable 便于打印
---@param data table        @表示要输出的数据
---@param dup_table table    @复用表格
---@param lastCount number    @用于格式控制，用户请勿使用该变量
function dumpTableEx(data, dup_table, lastCount)
    dumpTableList = {}
    local ret = realDumpTableEx(data, dup_table, lastCount)
    dumpTableList = {}
    return ret
end

--- Clone 克隆表
function HUtils.Clone(object)
    local lookup_table = {}
    local function copyObj(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end

        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[copyObj(key)] = copyObj(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return copyObj(object)
end

--- IsEmpty 判断表是否为空
function HUtils.IsEmpty(tab)
    if (tab == nil) or type(tab) ~= "table" then
        return true
    end

    local isNull = true
    for _, _ in pairs(tab) do
        isNull = false
        break
    end

    return isNull
end

return HUtils