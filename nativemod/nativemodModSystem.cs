using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Common;

namespace nativemod;

public class nativemodModSystem : ModSystem
{
    public override void StartServerSide(ICoreServerAPI sapi)
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        
        var outputPtr = Interop.CopyString("test string");
        string outString = Marshal.PtrToStringAnsi(outputPtr);
        Mod.Logger.Notification(outString);
        //Interop.free(outputPtr);
    }

    private IntPtr DllImportResolver(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        var suffix = RuntimeEnv.OS switch
        {
            OS.Windows => ".dll",
            OS.Mac => ".dylib",
            OS.Linux => ".so",
            _ => throw new ArgumentOutOfRangeException()
        };
        if (NativeLibrary.TryLoad($"{((ModContainer)Mod).FolderPath}/native/{libraryname}{suffix}", out var handle))
        {
            return handle;
        }
        return IntPtr.Zero;
    }
}

public class Interop
{
    // Declare the PInvoke function for CopyString
    [DllImport("libcopystring", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CopyString(string input);

    // Declare the PInvoke function for free
    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    public static extern void free(IntPtr ptr);
}