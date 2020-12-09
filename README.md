## Description:

This tool achieves "net user" in Window API. I made this to be used with [Cobalt Strike's execute-assembly](https://blog.cobaltstrike.com/2018/04/09/cobalt-strike-3-11-the-snake-that-eats-its-tail/)

![image-20201209142127990](https://github.com/bopin2020/NetUser/tree/main/NetUser/image-20201209142127990.png)

Compiled with .NET 3.0 (Windows Vista's default)+. Needs to be run as admin levels.

**Born Platform:**

Window 10 20H2 and Virsual Studio 2019 16.8

## Why?

As we all known, "net user test test /add" I mean net.exe or net1.exe usually can be flagged as a hacker attack, and  some AV/EDRs maybe hook theirs.In order to fix this isuse,We can add/delete user with Windows API,but if I don't wanna upload my binary into target disk,how should I do? Fortunately, [Cobalt Strike's execute-assembly](https://blog.cobaltstrike.com/2018/04/09/cobalt-strike-3-11-the-snake-that-eats-its-tail/) allow us to load a .NET assembly in-memory,we don't need load binary from disk,that's cool!:) 

On the other hand,I wanna improve my programming skills, so I made it with P/Invoke, as far as know there is also an awesome project [D/Invoke](https://thewover.github.io/Dynamic-Invoke/), I will try it in future!

## How it works?

As you can see, The pic is so clear and easy,have a try.

You also can compile with .NET Framework 3.0 or other version.

## Credits

Thanks for:

* P/Invoke https://www.pinvoke.net/
* Microsoft https://docs.microsoft.com/en-us/windows/win32/api/lmaccess/
* 7089bAt@PowerLi https://cloud.tencent.com/developer/article/1669051 (Help me fix how C# deal with APIs inside with pointers,by "ref" keywords. I'm a newbie with C#:))
* Due to this,I create this tool. https://github.com/securesean/DecryptAutoLogon

## Download

Compiled Version [HERE](https://github.com/bopin2020/NetUser/tree/main/NetUser/NetUser.exe)



