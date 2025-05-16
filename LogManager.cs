namespace ABIAParser;

public sealed class LogManager
{
    private static string LogPath = "";

    public static bool Init()
    {
        LogPath = AppContext.BaseDirectory + "log.l";

        try
        {
            if (!File.Exists(LogPath))
                File.Create(LogPath).Close();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void Log(string message, bool withDate = true)
    {
        if (withDate)
            message = $"[{DateTime.Now}] {message}\n";
        
        bool append = true;

        if (File.ReadAllLines(LogPath).Length >= 1000)
            append = false;

        using StreamWriter sW = new(LogPath, append);

        sW.Write(message);
    }
}