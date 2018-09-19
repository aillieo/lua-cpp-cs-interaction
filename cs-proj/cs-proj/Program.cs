using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace cs_proj
{

    public class CppDll
    {
        const string DLL = "cpp-dll.dll";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaNewState")]
        public static extern IntPtr LuaNewState();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaOpenLibs")]
        public static extern void LuaOpenLibs(IntPtr L);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaDoString")]
        public static extern int LuaDoString(IntPtr L, byte[] str, int len);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaClose")]
        public static extern void LuaClose(IntPtr L);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "RegisterCSFunc")]
        public static extern void RegisterCSFunc(IntPtr L, string name, IntPtr func);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaPushNumber")]
        public static extern void LuaPushNumber(IntPtr L, double number);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaGetTop")]
        public static extern int LuaGetTop(IntPtr L);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaToNumber")]
        public static extern double LuaToNumber(IntPtr L, int level);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaGetGlobal")]
        public static extern void LuaGetGlobal(IntPtr L, string name);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LuaPCall")]
        public static extern IntPtr LuaPCall(IntPtr L, int nargs, int nrets, int errfunc);


    }


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr luaState);


    class Program
    {

        static int CSFunc(int a, int b)
        {
            Console.WriteLine("[CS] this is CSFunc with args a = " + a + " and b = " + b);
            int ret = 10 * a + b;
            Console.WriteLine("[CS] ret will be " + ret);
            return ret;
        }

        static int CSFuncWrap(IntPtr L)
        {
            CppDll.LuaGetTop(L);
            int arg0 = (int)CppDll.LuaToNumber(L, 1);
            int arg1 = (int)CppDll.LuaToNumber(L, 2);
            int o = CSFunc(arg0, arg1);
            CppDll.LuaPushNumber(L, o);
            return 1;
        }


        static void Main(string[] args)
        {

            // init lua state

            IntPtr L = CppDll.LuaNewState();
            CppDll.LuaOpenLibs(L);


            // lua scripts to run
            string lua_1 = @"
                print('[lua] ha! it is lua here');
                LuaFunc = function(x,y) 
                    print('[lua] this is LuaFunc with args x = ' .. x .. ' and y = ' .. y); 
                    local ret = x * 10 + y; 
                    print('[lua] ret will be ' .. ret);
                    return ret;    
                end
            ";

            string lua_2 = @"
                print('[lua] will call CSFunc(1,2) ...');
                local ret = CSFunc(1,2);
                print('[lua] we got '.. ret); 
            ";


            byte[] bytes = null;


            // load lua_1
            bytes = System.Text.Encoding.Default.GetBytes(lua_1);
            CppDll.LuaDoString(L, bytes, bytes.Length);


            // register cs func
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((LuaCSFunction)(CSFuncWrap));
            CppDll.RegisterCSFunc(L, "CSFunc", fn);


            // load lua_2
            bytes = System.Text.Encoding.Default.GetBytes(lua_2);
            CppDll.LuaDoString(L, bytes, bytes.Length);


            // call lua function
            Console.WriteLine("[CS] will call LuaFunc(3,5) ...");
            CppDll.LuaGetGlobal(L, "LuaFunc");
            CppDll.LuaPushNumber(L, 3);
            CppDll.LuaPushNumber(L, 5);
            CppDll.LuaPCall(L, 2, 1, 0);
            int ret = (int)CppDll.LuaToNumber(L, 1);
            Console.WriteLine("[CS] we got " + ret);

            CppDll.LuaClose(L);


            Console.ReadKey();
        }


    }
}
