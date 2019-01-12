using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace Sidebar.Module.Shortcut.FileSystem
{
    [SecuritySafeCritical]
    static class ImageHelper
    {
        public static Icon GetFileIcon(string path, IconSizeType sizeType, Size itemSize)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr retVal = SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), (int)(SHGFI_SYSICONINDEX | SHGFI_ICON));
            int iconIndex = shinfo.iIcon;
            IImageList iImageList = (IImageList)GetSystemImageListHandle(sizeType);
            IntPtr hIcon = IntPtr.Zero;
            if (iImageList != null)
                iImageList.GetIcon(iconIndex, (int)ILD_TRANSPARENT, ref hIcon);
            Icon icon = null;
            if (hIcon != IntPtr.Zero)
            {
                icon = Icon.FromHandle(hIcon).Clone() as Icon;
                DestroyIcon(shinfo.hIcon);
            }
            return icon;
        }        

        #region Constants

        const uint ILD_TRANSPARENT = 0x00000001;
        const uint SHGFI_ICON = 0x000000100;
        const uint SHGFI_SYSICONINDEX = 0x000004000;

        #endregion

        #region Internal

        static IImageList GetSystemImageListHandle(IconSizeType sizeType)
        {
            IImageList iImageList = null;
            Guid imageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            int ret = SHGetImageList((int)sizeType, ref imageListGuid, ref iImageList);
            return iImageList;
        }

        #endregion

        #region Native

        [ComImport(), Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IImageList
        {
            [PreserveSig]
            int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
            [PreserveSig]
            int ReplaceIcon(int i, IntPtr hicon, ref int pi);
            [PreserveSig]
            int SetOverlayImage(int iImage, int iOverlay);
            [PreserveSig]
            int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
            [PreserveSig]
            int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
            [PreserveSig]
            int Draw(ref IMAGELISTDRAWPARAMS pimldp);
            [PreserveSig]
            int Remove(int i);
            [PreserveSig]
            int GetIcon(int i, int flags, ref IntPtr picon);
            [PreserveSig]
            int GetImageInfo(int i, ref IMAGEINFO pImageInfo);
            [PreserveSig]
            int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
            [PreserveSig]
            int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);
            [PreserveSig]
            int Clone(ref Guid riid, ref IntPtr ppv);
            [PreserveSig]
            int GetIconSize(ref int cx, ref int cy);
            [PreserveSig]
            int SetIconSize(int cx, int cy);
            [PreserveSig]
            int GetImageCount(ref int pi);
            [PreserveSig]
            int SetImageCount(int uNewCount);
            [PreserveSig]
            int SetBkColor(int clrBk, ref int pclr);
            [PreserveSig]
            int GetBkColor(ref int pclr);
            [PreserveSig]
            int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
            [PreserveSig]
            int EndDrag();
            [PreserveSig]
            int DragEnter(IntPtr hwndLock, int x, int y);
            [PreserveSig]
            int DragLeave(IntPtr hwndLock);
            [PreserveSig]
            int DragMove(int x, int y);
            [PreserveSig]
            int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
            [PreserveSig]
            int DragShowNolock(int fShow);
            [PreserveSig]
            int GetItemFlags(int i, ref int dwFlags);
            [PreserveSig]
            int GetOverlayImage(int iOverlay, ref int piIndex);
        };

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", EntryPoint = "#727")]
        static extern int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

        #endregion
    }

    public enum IconSizeType
    {
        Medium = 0x0,
        Small = 0x1,
        Large = 0x2,
        ExtraLarge = 0x4
    }

    #region Internal

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SHELLEXECUTEINF
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;
        public int yBitmap;
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
    }

    #endregion
}
