# rarp
This is C# implementation of RARP (Inverse ARP, actually). It uses only CLR 2.0 features, so it should work on almost every recently used Windows PC (starting from Windows XP, since it uses iphlpapi.dll). It is a console application taking as input MAC address desired to be resolved and outputting IP address if resolving successed and nothing otherwise. It is single-threaded, so resolving may take a while.

This project was developed for finding my toy robo-car since it's IP was constantly changing in local Wi-Fi network 
and it's MAC was steel stable (obviously).
