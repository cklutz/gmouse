using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    private const uint MOUSEEVENTF_MOVE = 0x0001;

    static void Main(string[] args)
    {
        const int checkInterval = 1000;
        int inactivityThreshold = 8 * 60 * 1000; 

        if (args.Length > 1)
        {
            inactivityThreshold = int.Parse(args[1]);
        }

        Console.WriteLine($"Inactivity timeout is set to {inactivityThreshold}");
        Console.WriteLine("Hit CTRL+C to exit.");

        while (true)
        {
            uint idleTime = GetIdleTime() + checkInterval + 5;
            if (idleTime > inactivityThreshold)
            {
                // Move mouse cursor 1 pixel back and forth
                mouse_event(MOUSEEVENTF_MOVE, 1, 0, 0, 0);
                mouse_event(MOUSEEVENTF_MOVE, unchecked((uint)-1), 0, 0, 0);
            }
            Thread.Sleep(checkInterval);
        }
    }

    private static uint GetIdleTime()
    {
        var lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        if (GetLastInputInfo(ref lastInputInfo))
        {
            return (uint)Environment.TickCount - lastInputInfo.dwTime;
        }
        else
        {
            throw new Exception("Failed to get last input info.");
        }
    }
}
