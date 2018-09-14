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



    }


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr luaState);        


    class Program
    {

        static int TestFunc(IntPtr state)
        {

            return 0;
        }


        static void Main(string[] args)
        {

            IntPtr L = CppDll.LuaNewState();
            CppDll.LuaOpenLibs(L);

            {
                string lua = "local a = 1; local b = 2; print('a = '.. a);";
                byte[] bytes = System.Text.Encoding.Default.GetBytes(lua);
                int ret = CppDll.LuaDoString(L, bytes, bytes.Length);
            }

            LuaCSFunction f = new LuaCSFunction(TestFunc);
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(f);
            CppDll.RegisterCSFunc(L, "TestFunc", fn);

            {
                string lua = "if TestFunc then print('TestFunc');  else print('fail'); end";
                byte[] bytes = System.Text.Encoding.Default.GetBytes(lua);
                int ret = CppDll.LuaDoString(L, bytes, bytes.Length);
            }

            Console.WriteLine();
        }


    }
}
