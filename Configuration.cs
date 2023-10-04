using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Java_Bytecode_Toolkit
{
    public class Configuration
    {
        [JsonInclude]
        public bool EnableLogging { get; set; } = false;

        public static Configuration Open()
        {
            if (Directory.Exists(App.Current.CONFIG_DIR_FILE_PATH) == false)
            {
                Directory.CreateDirectory(App.Current.CONFIG_DIR_FILE_PATH);
            }

            if (File.Exists(App.Current.CONFIG_FILE_PATH) == false)
            {
                File.Create(App.Current.CONFIG_FILE_PATH).Close();
            }

            string configFileContents = File.ReadAllText(
                App.Current.CONFIG_FILE_PATH
            );

            if (configFileContents == "")
            {
                return new Configuration();
            }

            return JsonSerializer.Deserialize<Configuration>(configFileContents);
        }

        public Configuration()
        {

        }

        public void Save()
        {
            File.WriteAllText(
                App.Current.CONFIG_FILE_PATH,
                JsonSerializer.Serialize(this)
            );
        }
    }
}
