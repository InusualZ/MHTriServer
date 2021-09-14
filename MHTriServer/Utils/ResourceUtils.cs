using System.IO;
using System.Reflection;

namespace MHTriServer.Utils
{
    public static class ResourceUtils
    {
        public static Stream GetResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources." + name);
        }

        public static byte[] GetResourceBytes(string name)
        {
            using var stream = GetResource(name);
            var bytes = new byte[stream.Length];
            stream.Read(bytes);
            return bytes;
        }
    }
}
