// A simple program to set affinity for Rome2.exe (or any other process for that matter)
// Put it (or its shortcut) into win+R > shell:startup to make it run at Windows startup
// Without this, Rome 2 gets stuck at battle loading screens on many modern systems

using System.Diagnostics;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]

// Name of the process (without .exe)
const string GAME_PROCESS_NAME = "Rome2";
// Minimum required number of cores
const int MINIMUM_REQUIRED_CORES = 3;

/* Decimal version of the bit representation for threads to activate.
 For example; 1401 = 0/1/0/1 0111 1001
           CPUs -> 11/10/9/8 7654 3210
 From right to left -> CPU0, CPU3, CPU4-5-6, CPU 8, and CPU10 would be active */
// Suggested: 255 = 1111 1111 in binary (first 8 cores/threads enabled).
// However, one may not have as many as 8 cores/threads! So, we calculate based on the total number of cores
static int CalculateAffinityMask()
{
    int processorCount = Environment.ProcessorCount;

    // If the user has less than 8 cores/threads, adjust settings accordingly.
    // Also, if they have less than 3; why are they really here?
    // We will disable the use of at least 2 of them for this game!
    // Check if the system meets minimum requirements
    if (Environment.ProcessorCount < MINIMUM_REQUIRED_CORES)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Warning: Your system has only {Environment.ProcessorCount} logical processors.");
        Console.WriteLine($"This application requires at least {MINIMUM_REQUIRED_CORES} processors to function properly.");
        Console.WriteLine("\nPress any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
        Environment.Exit(1); // Exit with error code 1
    }
    else if (processorCount is >= 3 and <= 5)
    {
        // Without -1 it would enable all the cores, and you are here for a reason!
        return (1 << processorCount) - 1; 
        /* So, -1 excludes the first core. Lowering the total core count
         in hopes of helping with the loading screen problems.
         Since you only have a few cores, we do not want to disable most/many of them.
         Although I doubt anyone with less than 8 cores will encounter this problem, it does
         not hurt to add a couple of more lines to help someone, in case I'm wrong */
    }
    else if (processorCount <= 8)
    {
        return (1 << processorCount) - 3;
        /* So, -3 excludes the first 2 cores. Lowering the total core count
         in hopes of helping with the loading screen problems */
    }
    
    // If the user's CPU has more than 8 cores, limit it to 8 cores
    return 255;
}

int desiredAffinity = CalculateAffinityMask();
        
// Keep track if we already modified this process
// Default -1 to prevent any errors (or to cause some on purpose ¯\_(ツ)_/¯ )
var lastSetPID = -1;

Console.WriteLine("Monitoring for game process...");
Console.WriteLine("Press Ctrl+C to exit");

// Keeps looping as long as the app runs, looking for a specific process
while (true)
{
    var process = Process.GetProcessesByName(GAME_PROCESS_NAME).FirstOrDefault();
        
    // If we find the process we are looking for
    if (process != null)
    {
        // Only set affinity if we haven't processed this PID before
        if (process.Id != lastSetPID)
        {
            Console.WriteLine($"Found {GAME_PROCESS_NAME}.exe (PID: {process.Id})");

            try
            {
                // Set the ProcessorAffinity as requested
                /* (IntPtr) is a pointer to a 32-bit integer.
                 We give a decimal integer and use its bit representation to indicate cores.
                 However, it is unnecessary here, according to the compiler. ¯\_(ツ)_/¯ */
                process.ProcessorAffinity = (IntPtr)desiredAffinity;
                lastSetPID = process.Id;
                Console.WriteLine($"Set affinity for {GAME_PROCESS_NAME} (PID: {process.Id}) at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                lastSetPID = -1;
                Console.WriteLine($"Error setting affinity: {ex.Message}");
            }
        }
    }
    
    // There is no process
    else 
    {
        // No game process found, the user did shut the game down. Reset lastSetPID to default
        if (lastSetPID != -1)
        {
            Console.WriteLine($"Game process (PID: {lastSetPID}) no longer running");
            lastSetPID = -1; 
        }
    }
    // Checks once every minute if the target game/process/app is running 
    await Task.Delay(60000);
}
