#ifdef DLL_IMPLEMENT  
#define DLL_API __declspec(dllexport)  
#else  
#define DLL_API __declspec(dllimport)  
#endif  

extern "C" {  
#include "lua.h"  
#include "lualib.h"  
#include "lauxlib.h"  
}

extern "C" DLL_API lua_State * LuaNewState();

extern "C" DLL_API void	LuaOpenLibs(lua_State *L);

extern "C" DLL_API int LuaDoString(lua_State * L ,const char *s, int l);

extern "C" DLL_API void LuaClose(lua_State *L);

extern "C" DLL_API void RegisterCSFunc(lua_State *L, const char *name, lua_CFunction fn);

