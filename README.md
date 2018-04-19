# rarp
This is C# implementation of RARP (Inverse ARP, actually). It uses only CLR 2.0 features, so it should work on almost every recent Windows OS (starting from Windows XP, since it uses `iphlpapi.dll`). It's a console application taking as input MAC address to resolve and outputting corresponding IP address if resolve successed and nothing otherwise. It is single-threaded, so resolving may take a while.

*WARNING!* The code hasn't been tested in modern Windows versions (starting Wondows 8).

This project was developed for finding my toy robo-car since it's IP was constantly changing in local Wi-Fi network 
and it's MAC was steel stable (obviously).
