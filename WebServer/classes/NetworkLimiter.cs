using System;
using Org.BouncyCastle.Crypto;

namespace WebServer.classes;

public class NetworkLimiter
{
    public static bool CanRespond = true;

    static int MaxBytesSent;
    static int CurrentBytesSent;

    static object bytesSentLock = new();
    static NetworkLimiter()
    {
        //if 0 - no limit

        var max = Convert.ToInt32(Config.GetConfigValue("MaxBytesSentPerMinute"));
        MaxBytesSent = max;

        if (max != 0)
        {
            CurrentBytesSent = 0;

            Sweeper();
        }
    }

    static async Task Sweeper()
    {
        while (true)
        {
            await Task.Delay(60000);

            CurrentBytesSent = 0;
            CanRespond = true;
        }
    }


    public static async Task AddBytes(int b)
    {
        if(MaxBytesSent == 0)
        {return;}
        
        lock (bytesSentLock)
        {
            if (CurrentBytesSent + b >= MaxBytesSent)
            {
                CanRespond = false;
            }
            else
            {
                CurrentBytesSent += b;
            }
        }
    }
}
