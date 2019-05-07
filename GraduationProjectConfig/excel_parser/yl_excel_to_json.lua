

local input_table = {}
lfs.dumpdir(excel_dir, ".xlsx", input_table, false)--查找文件

-- 解压文件
-----------------------------------
-- 生成bat文件
local batstr = "cd %~dp0\nrd /s /q "..unpack_path.."\nmkdir "..unpack_path.."\ncd "..unpack_path.."\n";
for i,path in ipairs(input_table) do
	if not string.find(path,'~$') then
		batstr = batstr .. "mkdir %~dp0\\"..unpack_path..path.."\n";
		batstr = batstr .. "cd %~dp0\\"..unpack_path..path.."\n";
		batstr = batstr .. "%~dp0\\7z.exe x "..excel_dir..path..".xlsx\n";
	end
end
local file = io.open(bat_path.."run.bat","w");
file:write(batstr)
file:close();
------------------------------------
-- xlsx解压缩
os.execute(bat_path.."run.bat");
------------------------------------
-- -- 转换成txtTable
local all_sharedStrings = {}
local all_texts = {}
for i,path in ipairs(input_table) do
	if not string.find(path,'~$') then
		local xlspath = bat_path..unpack_path..path.."\\xl\\sharedStrings.xml"
		print('read sharedStrings',path)
		local xls = io.open(xlspath,"r");
		assert(xls);
		local data = xls:read("*a");
		xls:close();
	    local str = {};
		for it in string.gmatch(data, "<si>(.-)</si>") do
			local text=''
			for ot in string.gmatch(it,'<t>(.-)</t>') do
				text=text..ot
			end
	        table.insert(str,text)
		end
	    all_sharedStrings[path] = str;
	    local sheetpath = bat_path..unpack_path..path..'\\xl\\worksheets\\sheet1.xml'
	    print('read sheet',path)
	    local xlsx = io.open(sheetpath,"r");
	    assert(xls);
	    local data = xlsx:read("*a")
	    xlsx:close();
	    local tab = xml.eval(data)

	    local texts = {}
	    local maxcol=0
	    local maxrow=0
	    for i,v in ipairs(tab) do
	    	if v[0] == 'sheetData' then
	    		-- 表格数据
	    		for i2,rowvalue in ipairs(v) do
	    			for i3,cellvalue in ipairs(rowvalue) do
	    				local key = cellvalue.r
	    				local isstr = cellvalue.t == 's'
	    				local index;
	    				for i3,valuetype in ipairs(cellvalue) do
	    					if valuetype[0] == 'v' then
	    						index = isstr and str[tonumber(valuetype[1])+1] or valuetype[1]
	    					end
	    				end
	    				if index ~= nil then
					    	local st,ed,col,row = string.find(key,'(%a+)(%d+)')
					    	row = tonumber(row)
					    	local colnum
					    	if #col > 1 then
					    		local c1 = string.byte(string.sub(col,1,1))
					    		local c2 = string.byte(string.sub(col,2,2))
					    		c1 = c1 - 65
					    		c2 = c2 - 65
					    		colnum = c1*26+c2 + 1
					    	else
						    	colnum = string.byte(col) - 64
						    end
						    colnum = tonumber(colnum)
						    if colnum > maxcol then maxcol = colnum end
						    if row > maxrow then maxrow = row end
						    if not texts[colnum] then
						    	texts[colnum] = {}
						    end
					    	texts[colnum][row] = index
	    				end
	    			end
	    		end
	    	end
	    end
	    all_texts[path] = {
	    	texts = texts,
	    	maxcol = maxcol,
	    	maxrow = maxrow
	    }
	end
end

------------------------------------
-- 配置json格式
local syntax = {
	explore = {
		extra_key = 'map_id'
	},
	map_event = {
		extra_key = 'explore_id'
	},
	role_eup = {
		extra_keys = {'role_id','star'},
		dump1 = true,
	},
	trump_skill_sup = {
		extra_keys = {'rarity','level_id'},
	},
	game_config = {
		switch_key = 'key',
	},
	gacha_pool = {
		extra_keys = {'pool_id','rarity'},
		to_list = true,
	},
	battle = {
		extra_keys = {'battle_id','wave_id'},
		dump1 = true,
	},
	extra_file_name = {
		-- battle_buff_group = true,
		-- game_config = true,
		-- item_base = true,
		role_eup = true,
		-- skill_base = true,
		-- trump_evolution_upgrade = true,
		trump_skill_sup = true,
	},
	types = {
		string = {
			type='string',
			func = function ( v,t )
				if v == nil then
					return ''
				end
				v = tostring(v)
				v = string.gsub(v,'&quot;','"')
				v = string.gsub(v,'&#13;','\r')
				v = string.gsub(v,'&#10;','\n')
				v = string.gsub(v,"&lt;","<")
				v = string.gsub(v,"&gt;",">")
				return v
			end
		},
		int = {
			type='int',
			func = function ( v,t )
				if v == nil then
					return 0
				end
				return tonumber(v)
			end
		},
		json = {
			type='JsonData',
			func = function ( v,t )
				if v == nil or v=='' then
					return {}
				end
				v = string.gsub(v,'&quot;','"')
				v = string.gsub(v,'&#13;','\r')
				v = string.gsub(v,'&#10;','\n')
				v = string.gsub(v,"&lt;","<")
				v = string.gsub(v,"&gt;",">")
				return json.decode(v)
			end
		},
		reward = {
			type='Reward',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = '..t..'.LoadFromJson('..v..');\n'
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					if vars[3] == nil then
						print(v)
					end
					local d = {
						type=vars[1],
						drop_type=vars[2],
						prob=vars[3]*1000,
						id=vars[4],
						num=vars[5],
						iscost=false,
					}
					table.insert(ret,d)
				end
				return ret
			end
		},
		cost = {
			type='Cost',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = '..t..'.LoadFromJson('..v..');\n'
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					local d = {
						type=vars[1],
						drop_type=vars[2],
						prob=vars[3]*1000,
						id=vars[4],
						num=vars[5],
						iscost=true,
					}
					table.insert(ret,d)
				end
				return ret
			end
		},
		comp = {
			type='Operation',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = '..t..'.LoadFromJson('..v..');\n'
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					local d = {
						name=vars[1],
						value=vars[2]
					}
					if (vars[3]) then
						error('comp error');
					end
					table.insert(ret,d)
				end
				return ret
			end
		},
		map = {
			type='Dictionary<string,string>',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = new '..t..'();\n'..
				'\t\tvar _'..v..' = '..v..'.ToObject();\n'..
				'\t\tif(_build_effect!=null){\n\t\t\tforeach(var pair in _'..v..') { '..fix_filename_to_class(v)..'[pair.Key] = pair.Value.ToString(); }\n\t\t}\n'
			end,
			func = function( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					ret[vars[1]] = vars[2];
				end
				return ret;
			end
		},
		time = {
			type='int',
			func = function ( v,t )
				if v == nil then
					return 0
				end
				local year, mon, mday, hour, min, sec = string.match(v, "(%d+)%-(%d+)%-(%d+) (%d+):(%d+):(%d+)");
				local time = os.time({year=year,month=mon,day=mday,hour=hour,min=min,sec=sec})
				return time;
			end
		},
		cond = {
			type='Condition',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = '..t..'.LoadFromJson('..v..');\n'
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					table.insert(ret,vars)
				end
				return ret
			end
		},
		attribute = {
			type='CommonAttribute',
			convert = function ( v,t )
				return '\t\t'..fix_filename_to_class(v)..' = '..t..'.LoadFromJson('..v..');\n'
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,';')
				local ret = {}
				for i,v in ipairs(list) do
					local vars = string.split(v,':')
					local d = {
						name=vars[1],
						value=vars[2]
					}
					table.insert(ret,d)
				end
				return ret
			end
		},
		array = {
			type='List<int>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\t{\n'..
				'\t\t\tif('..fftc..'==null) '..fftc..'=new List<int>();\n'..
				'\t\t\tIList<JsonData> list = '..v..'.ToArray();\n'..
				'\t\t\tforeach(var json in list)\n'..
				'\t\t\t{\n'..
				'\t\t\t\t'..fftc..'.Add(json.ToInt());\n'..
				'\t\t\t}\n'..
				'\t\t}\n';
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,',')
				return list
			end,
		},
		list_int = {
			type='List<int>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\tif('..fftc..'==null) '..fftc..'=new List<int>();\n'..
				'\t\t'..fftc..'.LoadFromJson('..v..');\n'
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				return json.decode(v)
			end,
		},
		list_float = {
			type='List<float>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\tif('..fftc..'==null) '..fftc..'=new List<float>();\n'..
				'\t\t'..fftc..'.LoadFromJson('..v..');\n'
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				return json.decode(v)
			end,
		},
		list2_int = {
			type='List<List<int>>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\tif('..fftc..'==null) '..fftc..'=new List<List<int>>();\n'..
				'\t\t'..fftc..'.LoadFromJson('..v..');\n'
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				return json.decode(v)
			end,
		},
		list2_float = {
			type='List<List<float>>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\tif('..fftc..'==null) '..fftc..'=new List<List<float>>();\n'..
				'\t\t'..fftc..'.LoadFromJson('..v..');\n'
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				return json.decode(v)
			end,
		},
		strarray = {
			type='List<string>',
			convert = function ( v,t )
				local fftc = fix_filename_to_class(v)
				local code = '\t\t{\n'..
				'\t\t\tif('..fftc..'==null) '..fftc..'=new List<string>();\n'..
				'\t\t\tIList<JsonData> list = '..v..'.ToArray();\n'..
				'\t\t\tforeach(var json in list)\n'..
				'\t\t\t{\n'..
				'\t\t\t\t'..fftc..'.Add(json.ToString());\n'..
				'\t\t\t}\n'..
				'\t\t}\n';
				return code
			end,
			func = function ( v,t )
				if v == nil then
					return {}
				end
				local list = string.split(v,',')
				return list
			end,
		},
		config = {
			type='JsonData',
			func = function ( v,t )
				local num = tonumber(v)
				if num == nil then
					v = string.gsub(v,'&quot;','"')
					v = string.gsub(v,'&#13;','\r')
					v = string.gsub(v,'&#10;','\n')
					return json.decode(v)
				else
					return num
				end
			end
		},
	}
}


local all_json1 = {}
local all_json2 = {}

local class = {}

function fix_filename_to_class( filename )
	local cf = string.split(filename,'_')
	local clsname = ''
	for i,v2 in ipairs(cf) do
		local f = string.upperFirst(v2)
		clsname = clsname .. f
	end
	return clsname
end


for path,v in pairs(all_texts) do
	print('convert json ',path)
	local texts = v.texts
	local maxcol = v.maxcol
	local maxrow = v.maxrow

	local first_json = {}
	local remarks = {}
	local fields = {}
	local types = {}
	class[path] = {}
	-- first_json['__remarks'] = remarks
	-- first_json['__fields'] = fields
	-- first_json['__types'] = types
	for i=1,maxcol do
		if texts[i] ~= nil then
			remarks[i] = texts[i][1]
			fields[i] = texts[i][2]
			types[i] = texts[i][3]
			if fields[i] ~= nil then
				table.insert(class[path],{
						remark = remarks[i],
						field = fields[i],
						type = types[i],
					})
				for k=4,maxrow do
					if texts[1][k] == nil  then
						break
					end
					if i == 1 then
						first_json[texts[1][k]] = {}
					end
					if syntax.types[types[i]] then
						if path == 'game_config' and fields[i] == 'value' then
							first_json[texts[1][k]][fields[i]] = syntax.types[texts[3][k]].func(texts[i][k],texts[3][k])
						else
							first_json[texts[1][k]][fields[i]] = syntax.types[types[i]].func(texts[i][k],types[i])
						end
					else
						error('unknow type:'..path..'['..i..']'..tostring(types[i]))
					end
				end
			end
		end
	end
	all_json1[path] = first_json

	if syntax[path] then
		if syntax[path] == 'ignore' then

		else
			local key = syntax[path].extra_key
			if key then
				for id,obj in pairs(first_json) do
					if not all_json2[path..':'..obj[key]] then
						all_json2[path..':'..obj[key]] = {}
					end
					all_json2[path..':'..obj[key]][id] = obj
				end
			end
			key = syntax[path].switch_key
			if key then
				for id,obj in pairs(first_json) do
					all_json2[path..':'..obj[key]] = obj
				end
			end
			key = syntax[path].extra_keys
			if key then
				for id,obj in pairs(first_json) do
					local ckey = ''
					if syntax[path].dump1 then
						local okey = path..':'..obj[key[1]]
						if not all_json2[okey] then
							all_json2[okey] = {}
						end
						all_json2[okey][tostring(obj[key[2]])] = obj
					else
						for i2,v2 in ipairs(key) do
							ckey = ckey..':'..obj[v2]
						end
						if not all_json2[path..ckey] then
							all_json2[path..ckey] = {}
						end
						if syntax[path].to_list then
							table.insert(all_json2[path..ckey],obj)
						else
							all_json2[path..ckey] = obj
						end
					end
				end
			end
		end
	end
	for id,obj in pairs(first_json) do
		all_json2[path..':'..id] = obj
	end
	all_json2[path..':all'] = first_json
	-- dump(first_json)
end

function string.upperFirst( str )
	local f = string.upper(string.sub(str,1,1))
	return string.replace(str,1,1,f)
end

local config_loaders = {}

for path,v in pairs(class) do
	print('convert cs code:',path)
	local cf = string.split(path,'_')
	if cf[#cf] == 'config' then
		cf[#cf]=nil
	end
	local filename = ''
	for i,v2 in ipairs(cf) do
		local f = string.upperFirst(v2)
		filename = filename .. f
	end
	filename = filename..'Config'
	local fixname = filename
	if syntax.extra_file_name[path] then
		fixname = path..'_config'
	end
	table.insert(config_loaders,{filename,path});

	local output = 
'using System;\n'..
'using System.Collections;\n'..
'using System.Collections.Generic;\n'..
'using UnityEngine;\n'..
'using LitJson;\n'..
'\n'
	if fixname=="GameConfig" then
output=output..'public partial class '..fixname..'\n'..
'{\n'
		local loadcode = ''
		local jsontabs = {}
		for cfgid,obj in pairs(all_json1[path]) do
			table.insert(jsontabs,obj)
		end
		table.sort(jsontabs,function( a,b )
			return string.compare(a.key,b.key) > 0
		end)
		for cfgid,obj in pairs(jsontabs) do
			local t = obj.type
			local line = ''
			if syntax.types[t] ~= nil then
				if t == 'string' then
					line = '\tstatic public '+t+' '..obj.key..';\n'
					loadcode = loadcode..'\t\t'..obj.key..' = json["'..obj.key..'"].ToString();\n\n'
				elseif t == 'int' then
					line = '\tstatic public '+t+' '..obj.key..';\n'
					loadcode = loadcode..'\t\t'..obj.key..' = json["'..obj.key..'"].ToInt();\n\n'
				elseif t == 'float' then
					line = '\tstatic public '+t+' '..obj.key..';\n'
					loadcode = loadcode..'\t\t'..obj.key..' = json["'..obj.key..'"].ToFloat();\n\n'
				else
					line = '\tstatic public JsonData '..obj.key..';\n'
					line = line..
							'\tstatic public '..syntax.types[t].type..' '..fix_filename_to_class(obj.key)..';/* '..obj.remark..' */\n\n'
					if syntax.types[t].convert then
						loadcode = loadcode..
							'\t\t'..obj.key..' = json["'..obj.key..'"];\n'..
							syntax.types[t].convert(obj.key,syntax.types[t].type)..'\n'
					end
				end
			else
				error('game_config unknow type '.. t);
			end
			output = output..line;
		end
		output = output..
		'\tstatic public void LoadFromJson(JsonData json)\n'..
		'\t{\n'..
		loadcode..'\t}\n}\n\n';
		local tjson = all_json1[path]
		all_json1[path] = {}
		for k,v in pairs(tjson) do
			all_json1[path][v.key] = v.value
		end
	else
output=output..'public partial class '..fixname..' : BaseConfig<'..filename..'>\n'..
'{\n'
		local ts = {}
		local extra_keys = {}
		for i2,v3 in ipairs(v) do
			local key = v3.field
			local line;
			if syntax.types[v3.type].convert then
				line = '\tpublic '..'JsonData'..' '..key..';/*'..tostring(v3.remark)..'*/\n'..
				'\tpublic '..syntax.types[v3.type].type..' '..fix_filename_to_class(key)..';/*'..tostring(v3.remark)..'*/\n'
				table.insert(extra_keys,{key,syntax.types[v3.type].type,syntax.types[v3.type].convert})
			else
				line = '\tpublic '..syntax.types[v3.type].type..' '..key..';/*'..tostring(v3.remark)..'*/\n'
			end
			table.insert(ts,line)
		end
		if #extra_keys > 0 then
			output = output..'\tpublic override void OnLoadJsonEnded()\n';
			output = output..'\t{\n\t\tbase.OnLoadJsonEnded();\n';
			for idx,vs in ipairs(extra_keys) do
				output = output..vs[3](vs[1],vs[2])
			end
			output = output..'\t}\n';
		end
		output = output..table.concat(ts)..'}\n\n'
	end

	local file = io.open(class_path..fixname..'.cs','w')
	file:write(output)
	file:close()

end

local file = io.open(resources_path..'all_config.json','w')
file:write(json.encode(all_json1))
file:close()

local loader_code = 
'using System;\n'..
'using System.Collections;\n'..
'using System.Collections.Generic;\n'..
'using UnityEngine;\n'..
'using LitJson;\n'..
'\n'..
'public class ConfigLoader\n'..
'{\n'..
'\tstatic public void LoadFromJson(JsonData json)\n'..
'\t{\n';
table.sort(config_loaders,function( a,b )
	return string.compare(a[1],b[1]) > 0
end)
for i,v in ipairs(config_loaders) do
	loader_code=loader_code..
	'\t\t'..v[1]..'.LoadFromJson(json["'..v[2]..'"]);\n';
end
loader_code=loader_code..'\t}\n}\n'
local file = io.open(class_path..'ConfigLoader.cs','w')
file:write(loader_code)
file:close()

local jsn = json.encode(all_json2)
require'ltn12'
for i,url in ipairs(remote_url) do
	print(url)
	local response_body = {}
	local http = require('socket.http')
	local ret = http.request({
			url = url,
			method = 'POST',
			headers = {  
	            ["Content-Type"] = "application/x-www-form-urlencoded";  
	            ["Content-Length"] = #jsn;  
	        },
			source = ltn12.source.string(jsn),
			sink = ltn12.sink.table(response_body)
		});
	dump(response_body)
end

-- dump(all_texts)
-- local batstr = "cd %~dp0\nrd /s /q "..unpack_path;
-- local file = io.open(bat_path.."run.bat","w");
-- file:write(batstr)
-- file:close();
-- os.execute(bat_path.."run.bat");
-- return all_tabs;
