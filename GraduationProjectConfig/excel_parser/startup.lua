os.execute("chcp 65001");

-- pathinfo.appPath()是tlua.exe的路径

excel_dir = [[F:\UnityProject\GraduationProjectConfig\excel\]]
unpack_path = [[output\unpack_excel\]]
bat_path = [[F:\UnityProject\GraduationProjectConfig\excel_parser\]];
class_path = [[F:\UnityProject\GraduationProject\Assets\Configs\]]
resources_path = [[F:\UnityProject\GraduationProject\Assets\Resources\]]
remote_url = {
	-- [[http://172.16.20.100/index.php/2144/Async/FlushStaticData]],
	-- [[http://115.159.119.168/index.php/2144/Async/FlushStaticData]]
}

-- os.execute('svn update '..excel_dir)
-- os.execute('pause')

require('yl_excel_to_json');