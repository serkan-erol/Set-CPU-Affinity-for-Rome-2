Set-CPU-Affinity-for-Rome-2

What is this and why does it exist:
    Total War: Rome 2 is an old game. As such, it has some problems with newer hardware.
    One of those problems is getting stuck at loading screen of a battle. Well, that is the one I encountered. 
    After some research, I've found out this problems has something to do with the number of CPU cores/threads assigned for Rome 2.
    Changing this CPU affinity setting solves the problem in my case. Steps to solve:
        Task Manager -> Details -> Find Rome2.exe and right click -> Set affinity -> Disable some of the cores -> DONE!
        I suggest maximum of 8 cores/threads.
    However, EVERY TIME I launch the game, I have to do this again.
    This is where my simple console app comes into play. It handles the settings for you just with a double click.
    It is only for x64 based Windows systems!

Before using:
    It is Windows x64 only!
    If you have less than 3 logical cores, it will not work, and simply it will throw an error.
    
How to use:
    Option 1: Take the .exe file's shortcut and put it near your Rome 2 shortcut and simply launch both of them. Order of launch does not really matter. 
    Only difference, if you launch the game first, then launch the app; it takes effect immediately. 
    If you launch the app first, it can not find the game at initial check (since you did not launch it, yet). Then, waits for a minute before checking again.
    Option 2: Put the shortcut in `shell:startup`. So, it launches when Windows starts. As long as it is running, you do not have to worry about `Set Affinity`
    Though, an always open CMD window may annoy you!
    You do not have to keep it open at all times. As soon as you see the message "Set affinity for Rome2 (PID: -PID for current instance of Rome2-) at -Date and Time-"   
    you can close the window/exit the app and your settings are changed, your current session is ready to play. You just need to launch the app next time you launch the game, as well.

Where is the .exe:
    You can find it in: /bin/Release/net9.0/win-x64/publish
    It is self contained, meaning you do not have to download .NET Runtime to make it work. Everything is included. That is why the .exe of such a simple program takes ~70 MBs.

Enjoy!
