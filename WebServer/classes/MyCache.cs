using System;
using System.Dynamic;
using System.IO.Compression;

namespace WebServer.classes;

public static class MyCache
{
    //simplified implementation of a cache
    //later could be changed so the moest used sites are in cache and not used as often are not
    //rn site is too small to worry about this
    static int maxCacheSize; //bytes
    static int currentCacheUse = 0;
    static List<CachedItem> cachedItems;

    static object cacheLock = new();
    public static void Initialize()
    {
        maxCacheSize = Convert.ToInt32(Config.GetConfigValue("MaxCacheSize"));
        cachedItems = new();
    }

    public static bool GetFromCache(string resource, Stream stream)
    {
        for (int i = 0; i < cachedItems.Count; i++)
        {
            if(cachedItems[i].resourcePath == resource)
            {
                //Console.WriteLine("Taking from cache");

                stream.Write(cachedItems[i].item);

                NetworkLimiter.AddBytes(cachedItems[i].item.Length);
                
                return true;
            }
        }
        //Console.WriteLine(resource + "  Caching");

        AddToCache(resource);
        return false;
    }

    private static async Task AddToCache(string resource)
    {
        byte[] itemarr;

        using (FileStream fs = new FileStream(resource, FileMode.Open, FileAccess.Read))
        {
            using (var output = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    await fs.CopyToAsync(gzip).ConfigureAwait(false);
                }

                itemarr = output.ToArray();
            }
        }

        if (currentCacheUse + itemarr.Length < maxCacheSize)
        {
            var item = new CachedItem();
            item.item = itemarr;
            item.resourcePath = resource;
            item.size = itemarr.Length + resource.Length;

            lock (cacheLock)
            {
                currentCacheUse += item.size;

                cachedItems.Add(item);
            }
        }
    }

    struct CachedItem
    {
        public byte[] item;
        public string resourcePath;

        public int size; //in bytes
    }
}