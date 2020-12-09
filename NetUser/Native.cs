using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetUser
{
    class Native
    {
        const UInt32 USE_NOFORCE = 0;
        const UInt32 USE_FORCE = 1;
        const UInt32 USE_LOTS_OF_FORCE = 2;

        [DllImport("Netapi32.dll", SetLastError = true, EntryPoint = "NetUserDel", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Error)]
        public static extern UInt32 NetUserDel(
                IntPtr servername,
                string username
            );

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetUserAdd(
        [MarshalAs(UnmanagedType.LPWStr)] 
            string servername,
            UInt32 level,
            ref USER_INFO_1 userInfo,
            string parm_err
            );

        [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 NetLocalGroupAddMembers(
            string servername, //server name
            string groupname, //group name
            UInt32 level, //info level
            ref LOCALGROUP_MEMBERS_INFO_3 buf, //Group info structure
            UInt32 totalentries //number of entries
            );

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern UInt32 NetUserSetInfo(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
            string username,
            UInt32 level,
            ref USER_INFO_1008 buf,
            string parm_err
            );

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern UInt32 NetUserSetInfo(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
            string username,
            UInt32 level,
            ref USER_INFO_1003 buf,
            string parm_err
            );


        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();
        public static bool IsCurrentProcessAdmin()
        {
            return IsUserAnAdmin();
        }

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetLastError", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern int GetLastError();
        public static Int32 GetLastWin32Error()
        {
            return GetLastError();
        }

        [DllImport("Netapi32.dll")]
        public extern static int NetUserEnum([MarshalAs(UnmanagedType.LPWStr)]
                string servername,
                int level,
                int filter,
                out IntPtr bufptr,
                int prefmaxlen,
                out int entriesread,
                out int totalentries,
                out int resume_handle
            );


    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct  USER_INFO_1
    {
        public string name;
        public string password;
        public int password_age;
        public int priv;
        public string home_dir;
        public string comment;
        public int flags;
        public string script_path;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct USER_INFO_0
    {
        public String Username;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct USER_INFO_1003
    {
        public String sPassword;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct USER_INFO_1008
    {
        public UInt32 usri1008_flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LOCALGROUP_MEMBERS_INFO_3
    {
        public string domainandname;
    }


    public enum UserLevel
    { 
        User_PRIV_GUEST,
        USER_PRIV_USER,
        USER_PRIV_ADMIN
    }
}
