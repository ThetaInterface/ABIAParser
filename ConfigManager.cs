using static ABIAParser.EProperty;

namespace ABIAParser;

public sealed class ConfigManager
{
    private static string ConfigPath = ""; 
    
    public static bool Init()
    {
        ConfigPath = AppContext.BaseDirectory + "config.ini";

        try 
        {
            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath).Close();

                var @default = new Dictionary<EProperty, string>
                {
                    { SMTPServer, "smtp.gmail.com" },
                    { SMTPPort, "587" },
                    { SMTPUser, "NS" },
                    { SMTPUserPassword, "NS" },
                    { SendFrom, "NS" },
                    { SendTo, "NS" },
                    { DelayMS, "900000" }
                };

                WriteConfig(@default);
            }

            return true;
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Initialize Config Manager Erorr) {ex.Message}");

            return false;
        }
    }

    public static void ReadConfig(out Dictionary<EProperty, string> config)
    {
        config = [];

        try
        {
            using StreamReader sR = new(ConfigPath);
            
            while (!sR.EndOfStream)
            {
                string line = sR.ReadLine() ?? "";

                if (line != "")
                {
                    string[] pair = line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (pair.Length == 2)
                        config.Add(Enum.Parse<EProperty>(pair[0]), pair[1]);
                    else
                        LogManager.Log($"(Read Config Error) Corrupted config line! Content: {line}");
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Read Config Error) {ex.Message}");
        }
    }

    public static Dictionary<EProperty, string> ReadConfig()
    {
        Dictionary<EProperty, string> config = [];

        try
        {
            using StreamReader sR = new(ConfigPath);
            
            while (!sR.EndOfStream)
            {
                string line = sR.ReadLine() ?? "";

                if (line != null)
                {
                    string[] pair = line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (pair.Length == 2)
                        config.Add(Enum.Parse<EProperty>(pair[0]), pair[1]);
                    else
                        LogManager.Log($"(Read Config Error) Corrupted config line! Content: {line}");
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Read Config Error) {ex.Message}");
        }

        return config;
    }


    public static void WriteConfig(Dictionary<EProperty, string> config)
    {
        try
        {
            using StreamWriter sW = new(ConfigPath);

            string text = "";

            foreach (KeyValuePair<EProperty, string> pair in config)
                text += $"{pair.Key}:{pair.Value}\n";
            
            sW.Write(text);
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Write Config Error) {ex.Message}");
        }
    }
}