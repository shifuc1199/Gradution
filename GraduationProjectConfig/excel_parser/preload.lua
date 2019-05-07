function __G__TRACKBACK__( msg )
	print(msg..'\n')
	print(debug.traceback("", 2))
end

----------------------------------------
-------Function
----------------------------------------
local __method = {
	new = function(m, cx)
		local func = {
			context = cx,
			method = m
		}
		setmetatable(func,__method)
		return func
	end,
	__call = function( my, ... )
		return my.method(my.context, ...)
	end
}

----------------------------------------
-------void
----------------------------------------
void = {}
setmetatable(void,{
	__index = function ( t, k )
		Exception.VOID_GET_VALUE(key)
		return nil
	end,
	__newindex = function ( t, k, v )
		Exception.VOID_SET_VALUE(k)
	end,
	__unm = function ( t )
		return true
	end,
	__add = function ( a,b )
		return b
	end,
	__tostring = function ( t )
		return 'void';
	end
})

----------------------------------------
-------Math
----------------------------------------
Math = math
Math.LN2 = math.log(2)
Math.LN10 = math.log(10)
Math.E = math.exp(1)
Math.PI = math.pi
Math.SQRT2 = math.sqrt(2)
Math.SQRT1_2 = 1/Math.SQRT2

----------------------------------------
-------Array
----------------------------------------
Array = {
	new = function ( ... )
		local tab = {...}
		local c0 = tab[1]
		table.remove(tab,1)
		rawset(tab,0,c0)
		setmetatable(tab, {
				__index = function ( t, i )
					if i == 'count' then
						return #t+1
					elseif Array[i] then
						return __method.new(Array[i],t)
					else
						return rawget(t,i)
					end
				end,
				__newindex = function ( t, k, v )
					local n = tonumber(k)
					if n then
						rawset(t,0,void)
						if n>#t then
							for c=#t+1,n do
								rawset(t,c,void)
							end
						end
						rawset(t,n,v)
					elseif k == 'count' then
						if v-1>#t then
							for c=#t+1,v-1 do
								rawset(t,c,void)
							end
						else
							for i=#t,v,-1 do
								rawset(t,i,nil)
							end
						end
					else
						rawset(t,k,v)
					end
				end
			})
		return tab;
	end
}

----------------------------------------
-------String
----------------------------------------
String = getmetatable('String');
String.__index = function ( t,i )
	return string.sub(t,i+1,i+1);
end
String.__add = function ( a,b )
	if a == nil or a == void then a = '' end
	if b == nil or b == void then b = '' end
	return a .. b
end

function string.compare( a, b )
	if a == b then return 0 end
	local ta = {string.byte(a,1,string.len(a))}
	local tb = {string.byte(b,1,string.len(b))}
	for i,v in ipairs(ta) do
		if tb[i] == nil then
			return 1
		end
		if v > tb[i] then
			return 1
		elseif v < tb[i] then
			return -1
		end
	end
	return -1
end


function incontextof(m, cx )
	local comtype = type(m)
	if comtype == 'function' then
		return __method.new(m,cx)
	elseif comtype == 'table' and getmetatable(m) == __method then
		return __method.new(m.method,cx)
	else
		return Exception.INCONTEXTOF_ERROR_TYPE(comtype)
	end
end


function typeof( obj )
	if obj == nil then
		return 'undefined'
	elseif obj == void then
		return 'void';
	else
		local t = type(obj)
		if t == 'number' then
			if math.floor(obj) < obj then
				return 'Real'
			else
				return 'Integer'
			end
		elseif t == 'string' then
			return 'String'
		else
			return 'Object'
		end
	end
end

function instanceof( obj, clsname )
	if type(obj) == clsname then
		return true
	else
		if type(obj) == 'table' then
			local cls = rawget(myobj,'class')
			if cls then
				return getter.instanceof(cls,clsname)
			else
				-- TODO 这里还要判断Array或其他
				return clsname == 'Dictionary'
			end
		end
	end
end

function string.split(str, delimiter)
    if str==nil or str=='' or delimiter==nil then
        return nil
    end
    local result = {}
    local st,ed,led
    led = 1
    while true do
        st,ed = string.find(str,delimiter,led)
        if st then
            table.insert(result,string.sub(str,led,st-1))
            led = ed+1
        else
            break
        end
    end
    if led <= #str then
        table.insert(result,string.sub(str,led,st))
    end
    return result
end

function string.replace( str, startpos, endpos, repstr )
    local leftstr = ''
    local rightstr = ''
    if startpos > 1 then
        leftstr = string.sub(str,1,startpos-1)
    end
    if endpos < #str then
        rightstr = string.sub(str,endpos+1,#str)
    end
    return leftstr..repstr..rightstr
end

function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function os.pause()
    os.execute('pause')
end

function dump(tbl, key , level)
	local msg = ""
	level = level or 0
	local indent_str = ""
	for i = 1, level do
		indent_str = indent_str.."    "
	end
	if key then
		key = key..' = ';
	else
		key = ''
	end
	if type(tbl) == 'table' then
		print(indent_str .. key .. "{")
		for k,v in pairs(tbl) do
			dump(v, k, level + 1)
			if type(v) == "table" then
			end
		end
		print(indent_str .. "}")
	else
		local item_str = string.format("%s%s%s", indent_str,key,tostring(tbl))
		print(item_str)
	end
end


getter = {}
setter = {}

function getter.super(obj, cls, key)
	if cls == nil then return nil end;
	return getter.member(obj,cls.super,key)
end

function getter.instanceof( cls, name )
	if cls == nil then return false end;
	return cls.typeof == name or getter.instanceof(cls.super,name)
end

function getter.member(obj, cls, key )
	if cls == nil then return nil end
	local func = cls.getter[key]
	if func then
		return incontextof(func,obj)
	else
		func = cls.method[key]
		if func then
			return incontextof(func,obj)
		else
			func = cls.setter[key]
			if func then
				Exception.PROPERTY_WRITEONLY(key)
			else
				return getter.member(obj,cls.super,key)
			end
		end
	end
end

function getter.this( this, name )
	if this == nil then
		return _G[name]
	else
		return this[name]
	end
end


function setter.super( obj, key, value )
	local cls = rawget(obj,'class');
end

local function get_or_super(cls,comtype,key)
	local super = cls
	while super do
		local func = super[comtype][key]
		if func then
			return func
		else
			super = super.super
		end
	end
end

__def__index = function( myobj, key )
	if key == 'class' then
		Exception.UNMODIFIABLE_VALUES(key)
		return nil;
	end
	local func = rawget(myobj,key)
	if func then
		return func
	else
		return getter.member(myobj,rawget(myobj,'class'), key)
	end
end;

__def__newindex = function( myobj, key, value )
	if key == 'class' then
		Exception.UNMODIFIABLE_VALUES(key)
		return;
	end
	local func = get_or_super(rawget(myobj,'class'),'setter',key)
	if func then
		func(myobj,value)
	else
		func = get_or_super(rawget(myobj,'class'),'getter',key)
		if func then
			Exception.PROPERTY_READONLY(key)
		else
			rawset(myobj,key,value)
		end
	end
end;

__def__new = function ( cls, ... )
	local obj = cls.create(...)
	obj.class = cls
	local obj_metatable = {
		__index = __def__index,
		__newindex = __def__newindex,
		__metatable = 'you could not change metatables.',
		__gc = cls.finalize,
	}
	cls.initialize(obj);
	setmetatable(obj,obj_metatable);
	cls.constructor(obj,...);
	return obj;
end

function class( name, ... )
	local arg = {...}
	local class_table = {}
	class_table.setter = {}
	class_table.getter = {}
	if #arg == 1 then
		class_table.super = arg[1]
	elseif #arg > 1 then
		-- 多重继承的时候，排在前面的方法优先索引，即与tjs相反
		table.remove(arg,1)
		class_table.super = class('', table.unpack(arg))
	end
	class_table.typeof = name
	class_table.method = {}
	class_table.initialize = function( self )
		if class_table.super then
			class_table.super.initialize()
		end
	end
	class_table.create = function( )
		return {}
	end
	class_table.constructor = function( self )
		if class_table.super then
			class_table.super.constructor()
		end
	end
	class_table.finalize = function( self )
		if class_table.super then
			class_table.super.finalize()
		end
	end
	class_table.new = __def__new
	return class_table;
end

function static_class( name, ... )
	return class(name,...):new()
end

----------------------------------------
-------Exception
----------------------------------------
local Exception = class("Exception");
-- return void.xxx
function Exception.VOID_GET_VALUE( key )
	error('Cannot get the member "'..key..'" in Void');
end
-- void.xxx = xxx
function Exception.VOID_SET_VALUE( key )
	error('Cannot set the member "'..key..'" to Void');
end
function Exception.PROPERTY_READONLY( key )
	error('Property "'..key..'" is ReadOnly');
end
function Exception.PROPERTY_WRITEONLY( key )
	error('Property "'..key..'" is WriteOnly');
end
function Exception.INCONTEXTOF_ERROR_TYPE( key )
	error('Could not incontextof type out "'..key..'"');
end
function Exception.VALUE_IS_NOT_FUNCTION( key )
	error('Value is not a function: "'..key..'"');
end
function Exception.UNMODIFIABLE_VALUES( key )
	error('Unmodifiable values: "'..key..'"');
end

-- 遍历获取目录
-- path = 目录
-- wefind = 后缀过滤
-- r_table = 返回值存储的table
-- intofolder = 是否递归
function lfs.dumpdir(path, wefind, r_table, intofolder)
    for file in lfs.dir(path) do
        if file ~= "." and file ~= ".." then
            local f = path..file
            if wefind ~= nil then
                if string.find(f, wefind) ~= nil then
                    local outpath = string.sub(file,1,#file-#wefind)
                    table.insert(r_table, outpath)
                end
            else
                table.insert(r_table, file)
            end
            if intofolder then
                local attr = lfs.attributes (f)
                assert (type(attr) == "table")
                if attr.mode == "directory" then
                    lfs.dumpdir(f, wefind, r_table, intofolder)
                end
            end
        end
    end
end

Exception = Exception
_G.__method = __method
-- Test = class('Test');

-- function Test.initialize(this)
-- 	if Test.super then
-- 		Test.super.initialize(this)
-- 	end
-- 	this.a = 1
-- 	this.b = 2
-- end

-- function Test.method.add( this, value)
-- 	print(this.b*value+this.a)
-- end

-- Test2 = class('Test2',Test)

-- function Test2.initialize( this )
-- 	if Test2.super then
-- 		Test2.super.initialize(this)
-- 	end
-- end

-- function Test2.method.add( this, value)
-- 	this.b = 4
-- 	this.a=3
-- 	getter.super(this,this.class,'add')(value+5)
-- end


-- local test = Test2:new()
-- test.add(3);