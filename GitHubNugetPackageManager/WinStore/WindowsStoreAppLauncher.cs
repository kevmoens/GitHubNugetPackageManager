
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GitHubNugetPackageManager.WinStore
{


    public static class WindowsStoreAppLauncher
    {
        [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationActivationManager
        {
            IntPtr ActivateApplication([In] String appUserModelId,
                                       [In] String arguments,
                                       [In] ActivateOptions options,
                                       [Out] out UInt32 processId);

            IntPtr ActivateForFile([In] String appUserModelId,
                          [In] IntPtr /*IShellItemArray* */ itemArray,
                          [In] String verb,
                          [Out] out UInt32 processId);

            IntPtr ActivateForProtocol([In] String appUserModelId,
                          [In] IntPtr /* IShellItemArray* */itemArray,
                          [Out] out UInt32 processId);
        }

        public static Process Launch(string appUserModelId, string? arguments = null)
        {
            var launcher = new ApplicationActivationManager();
            int hr = launcher.ActivateApplication(appUserModelId, arguments, ActivateOptions.None, out uint processId).ToInt32();
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            if (processId > 0)
            {
                return Process.GetProcessById((int)processId);
            }

            throw new Exception($"Could not launch Store App '{appUserModelId}'");
        }

        [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
        private class ApplicationActivationManager : IApplicationActivationManager
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                                MethodCodeType.Runtime)/*, PreserveSig*/]
            public extern IntPtr ActivateApplication(
                                       [In] String appUserModelId,
                                       [In] String arguments,
                                       [In] ActivateOptions options,
                                       [Out] out UInt32 processId);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                                                 MethodCodeType.Runtime)]
            public extern IntPtr ActivateForFile(
                                  [In] String appUserModelId,
                                  [In] IntPtr /*IShellItemArray* */itemArray,
                                  [In] String verb,
                                  [Out] out UInt32 processId);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                                                  MethodCodeType.Runtime)]
            public extern IntPtr ActivateForProtocol(
                                 [In] String appUserModelId,
                                 [In] IntPtr /* IShellItemArray* */itemArray,
                                 [Out] out UInt32 processId);
        }

    }
}
