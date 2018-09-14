#define DLL_IMPLEMENT  

#pragma comment(lib, "ws2_32.lib")
#include "cpp-dll.h"  

#include <string>

using namespace std;


DLL_API lua_State * LuaNewState()
{
	return luaL_newstate();
}

DLL_API void LuaOpenLibs(lua_State *L)
{
	luaL_openlibs(L);
}


DLL_API int LuaDoString(lua_State * L ,const char *s, int l)
{
	return luaL_dostring(L,string(s,l).c_str());
}


DLL_API void LuaClose(lua_State *L)
{
	lua_close(L);
}


DLL_API void RegisterCSFunc(lua_State *L, const char *name, lua_CFunction fn)
{
	lua_pushcfunction(L, fn); //将函数放入栈中  
	lua_setglobal(L, name);
}
