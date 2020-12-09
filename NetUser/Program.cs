using System;
using static System.Console;
using System.Security.Principal;
using System.Text;
using static System.Environment;
using System.Collections;

using System.Runtime.InteropServices;

using PInvoke = NetUser;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace NetUser
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[0])
                {
                    case "/add":
                        {
                            UserAdd(args[1], args[2]);
                            break;
                        }
                    case "/del":
                        {
                            DelUser(args[1]);
                            break;
                        }
                    case "/active":
                        {
                            ActiveUser(args[1],args[2]);
                            break;
                        }
                    case "/change":
                        {
                            ChangeUserPass(args[1], args[2]);
                            break;
                        }
                    case "-h":
                        {
                            Help();
                            Environment.Exit(0);
                            break;

                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                Help();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public static void DelUser(string user)
        {
            bool IsAdmin = Native.IsCurrentProcessAdmin();
            if (!IsAdmin)
            {
                WriteLine("[-] Run as administrator level!\n");
                Environment.Exit(0);
            }

            if (user == "")
            {
                WriteLine("[-] Usage: NetUser.exe /del username");
                Environment.Exit(0);
            }

            try
            {
                var nStatus = PInvoke.Native.NetUserDel((IntPtr)null, user);
                var NERR_Success = 0;
                WriteLine(nStatus);

                if (nStatus == NERR_Success)
                {
                    WriteLine($"[+] user account {user} has been successfully deleted on localhost!\n");
                }
                else
                {
                    WriteLine($"[-] A system error {nStatus} occurred!\n");
                    HelpmsgCode((int)nStatus);
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            { 
                
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        public static void UserAdd(string name,string pass)
        {
            bool IsAdmin = Native.IsCurrentProcessAdmin();
            if (!IsAdmin)
            {
                WriteLine("[-] Run as administrator level!\n");
                Environment.Exit(0);
            }

            USER_INFO_1 ui;
            UInt32 dwLevel = 1;
            Int32 dwError = 0;
            if (name==""&&pass=="")
            {
                WriteLine("[-] Usage: NetUser.exe /add username password");
                Environment.Exit(0);
            }
            else
            {
                ui.name = name;
                ui.password = pass;
                ui.password_age = 0;
                ui.priv = 1;    // USER_PRIV_USER
                ui.home_dir = null;
                ui.comment = null;
                ui.flags = 1;
                ui.script_path = null;

                var nStatus = PInvoke.Native.NetUserAdd(null,dwLevel, ref ui,null);
                var NERR_Success = 0;
                if (nStatus == NERR_Success)
                {
                    Console.WriteLine($"[+] User {name} been successgully added on localhost\n");
                    // 添加到管理员组
                    LOCALGROUP_MEMBERS_INFO_3 gi;
                    UInt32 Level = 3;
                    UInt32 totalentries = 1;
                    gi.domainandname = ui.name;
                    var gStatus = PInvoke.Native.NetLocalGroupAddMembers(null, "Administrators", Level, ref gi, totalentries);
                    if (gStatus == NERR_Success)
                    {
                        Console.WriteLine($"[+] User {name} has been added into administrators group!\n");
                    }
                    else
                    {
                        Console.WriteLine($"[-] A system error has occurred: {gStatus}\n");

                        HelpmsgCode(nStatus);
                    }
                }
                else
                { 
                    Console.WriteLine($"[-] A system error has occurred: {nStatus}\n");

                    HelpmsgCode(nStatus);
                }
            }

        }

        public static void ActiveUser(string user,string value/* value为 yes 或者no yes为激活用户 no 锁定用户 */)
        {

            bool IsAdmin = Native.IsCurrentProcessAdmin();
            if (!IsAdmin)
            {
                WriteLine("[-] Run as administrator level!\n");
                Environment.Exit(0);
            }

            UInt32 dwLevel = 1008;
            USER_INFO_1008 ui;
            UInt32 nStatus;
            var NERR_Success = 0;
            if (user == "")
            {
                WriteLine("[-] Usage: NetUser.exe /active username!");
                Environment.Exit(0);
            }
            else
            {
                // https://github.com/dlang/druntime/blob/master/src/core/sys/windows/lmaccess.d
                // 查询 UF_LOCKOUT枚举项 对应的整型数字

                if (value == "yes")
                {
                    ui.usri1008_flags = 16;    // UF_LOCKOUT  激活账户      2 UF_ACCOUNTDISABLE 锁定账户

                    nStatus = PInvoke.Native.NetUserSetInfo(null, user, dwLevel, ref ui, null);
                    if (nStatus == NERR_Success)
                    {
                        WriteLine($"[+] user account {user} has been activated!");
                    }
                    else
                    {
                        WriteLine($"A systm error has occurred: {nStatus}\n");
                        HelpmsgCode((int)nStatus);
                    }
                }
                if (value == "no")
                {
                    ui.usri1008_flags = 2;
                    nStatus = PInvoke.Native.NetUserSetInfo(null, user, dwLevel, ref ui, null);
                    if (nStatus == NERR_Success)
                    {
                        WriteLine($"[+] user account {user} has been locked!");
                    }
                    else
                    {
                        WriteLine($"A systm error has occurred: {nStatus}\n");
                        HelpmsgCode((int)nStatus);
                    }
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static void ChangeUserPass(string user, string pass)
        {
            bool IsAdmin = Native.IsCurrentProcessAdmin();
            if (!IsAdmin)
            {
                WriteLine("[-] Run as administrator level!\n");
                Environment.Exit(0);
            }

            UInt32 dwLevel = 1003;
            USER_INFO_1003 ui;
            UInt32 nStatus;
            var NERR_Success = 0;
            if (user == "" || pass == "")
            {
                WriteLine("[-] Usage: NetUser.exe /change username password!");
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    ui.sPassword = pass;

                    nStatus = PInvoke.Native.NetUserSetInfo(null, user, dwLevel, ref ui, null);
                    if (nStatus == NERR_Success)
                    {
                        WriteLine($"[+] user {user}'s password has been changed! ({pass})");
                    }
                    else
                    {
                        WriteLine($"A systm error has occurred: {nStatus}\n");
                        HelpmsgCode((int)nStatus);
                    }
                }
                catch (Exception)
                { 
                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static void HelpmsgCode(int number)
        {
            switch (number)
            {
                case 2245:
                    {
                        WriteLine("[2245] The password does not meet the password policy requirements." +
    " Check the minimum password length, password complexity and password history requirements." +
    "密码不符合策略,请使用复杂密码 数字+大小写字母+特殊字符! ");
                        break;
                    }
                case 2224:
                    {
                        WriteLine("[2224] The account already exists. 该用户已经存在!");
                        break;
                    }

                case 2221:
                    {
                        WriteLine("[2221] The user name could not be found.找不到用户!");
                        break;
                    }

                case 5:
                    {
                        WriteLine("[5] Access is denied. 权限不足!");
                        break;
                    }
                case 1371:
                    {
                        WriteLine("[1371] Cannot perform this operation on built -in accounts\n" +
                            "系统内置用户不能删除!");
                        break;
                    }
                case 87:
                    {
                        WriteLine("[87] The parameter is incorrect.参数不正确!");
                        break;
                    }

                    
                default:
                    {
                        break;
                    }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public static void Help()
        {
            WriteLine("============================================");
            string Message = "Author: \n[*] github: https://github.com/bopin2020\n" +
                 "[*] Twitter: @bopin2020\n" +
                 "[*] Date:    2020/12/08 22:17\n";
            Console.WriteLine(Message);

            WriteLine("============================================");
            string HelpText = "Example: \n[*] NetUser.exe /add bopin bopin123@   =>     net user bopin bopin123@ /add(Add users 添加用户并加入Admin组)\n" +
                "[*] NetUser.exe /del bopin             =>     net user test /del(Delete users 删除用户)\n" +
                "[*] NetUser.exe /active guest yes      =>     net user guest /active:yes(Activate users 激活用户)\n" +
                "[*] NetUser.exe /active guest no       =>     net user guest /active:no(Lock users 锁定用户)\n" +
                "[*] NetUser.exe /change guest guest123 =>     net user administrator 123(Changed user's password 更改用户密码)\n";

            WriteLine(HelpText);

            string Description = "Note: \n[*] All of these handles will execute on the target localhost,not domain.\nBecause someone assumes adding users in Domain is not a common scenarios! Obviously that's seldom.";
            WriteLine(Description);
        }
    }
}
