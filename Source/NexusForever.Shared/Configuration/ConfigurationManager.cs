using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
    public static class ConfigurationManager<T>
    {
        public static T Config { get; private set; }


        public static void Initialise(string file)
        {
            file = SelectFirstExistingFile(file, Path.ChangeExtension(file, ".local.json"), "global.json");
            SharedConfiguration.Initialize(file);
            Config = SharedConfiguration.Configuration.Get<T>();
        }

        private static string SelectFirstExistingFile(params string[] files)
        {
            foreach (string file in files)
            {
                if(System.IO.File.Exists(file)) 
                    return file;
            }

            return files.FirstOrDefault();
        }
    }
}
