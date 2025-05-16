namespace ABIAParser;

public static class Program
{
    private static ProgramMode mode = ProgramMode.Undefined;

    public static void Main(string[] args)
    {
        if (!LogManager.Init())
        {
            Console.Write("\nFailed to initialize log manager! Aborting...\n");
            return;
        }

        if (ConfigManager.Init())
            LogManager.Log("Successfully initialized!");
        else
        {
            LogManager.Log("(Fatal Error) failed to load ConfigManager! Aborting...");
            return;
        }
        
        MailManager.Init(ConfigManager.ReadConfig());

        AppDomain.CurrentDomain.ProcessExit += (sender, e) => Parser.Terminate();

        while (true)
        {
            if (args.Length == 1)
                if (args[0].Contains("-p"))
                    mode = ProgramMode.Parser;

            if (mode == ProgramMode.Undefined)
                while (true)
                {
                    Console.Clear();
                    
                    Console.WriteLine("Select mode:\n" +
                        "1: Parsing\n" +
                        "2: Config redacter\n" +
                        "3: Abort");

                    string key = Console.ReadLine() ?? "";

                    switch (key)
                    {
                        case "1":
                            mode = ProgramMode.Parser;
                            break;
                        case "2":
                            mode = ProgramMode.ConfigRedacter;
                            break;
                        case "3":
                            Console.Clear();
                            return;
                    }

                    if (mode != ProgramMode.Undefined)
                    {
                        Console.Clear();

                        break;
                    }
                }

            switch (mode)
            {
                case ProgramMode.Parser:
                    ConfigManager.ReadConfig(out Dictionary<EProperty, string> config);

                    _ = int.TryParse(config[EProperty.DelayMS], out int delayMS);
                    delayMS = delayMS < 5000 ? 5000 : delayMS;

                    ManualResetEventSlim exitEvent = new(false);
                    
                    Thread parser = new(() => 
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.Write("Enter 'q' to terminate parsing...");

                            if (exitEvent.Wait(delayMS))
                                break;

                            Parser.Parse(out string urlContent, driverRestart: delayMS >= 15000);

                            if (urlContent.Equals("2exFWD_OE"))
                            {
                                LogManager.Log("(Error) Parser returned error code! Terminating...");

                                return;
                            }

                            if (urlContent.Contains("applications are currently closed"))
                                LogManager.Log("Applications may be closed now...");
                            else
                            {
                                if (!MailManager.SendMail("\n\nINVITE APPLICATION IS OPENED NOW! https://animebytes.tv/register/apply"))
                                {
                                    LogManager.Log("(Fatal Error) Failed to send a message by mail!");
                                    return;
                                }

                                LogManager.Log("INVITE APPLICATIONS ARE OPENED https://animebytes.tv/register/apply");
                            }
                        }
                    });

                    parser.Start();

                    while (true)
                    {
                        if ((Console.ReadLine() ?? "q").ToLower().Equals("q"))
                            break;
                    }

                    exitEvent.Set();

                    if (delayMS < 15000)
                        Parser.Terminate();
                    break;
                case ProgramMode.ConfigRedacter:
                    ConfigManager.ReadConfig(out config);

                    foreach (KeyValuePair<EProperty, string> property in config)
                    {
                        Console.Clear();
                        Console.Write($"Do you want to edit '{property.Key}' = '{property.Value}' (y/n)?\n(default=n) = ");

                        string answer = Console.ReadLine() ?? "";

                        if (answer.ToLower().Equals("y"))
                        {
                            Console.Clear();

                            Console.WriteLine($"Current value for '{property.Key}': {property.Value}");

                            config[property.Key] = Console.ReadLine() ?? "NS";
                        }
                    }

                    ConfigManager.WriteConfig(config);
                    break;
            }

            mode = ProgramMode.Undefined;
            args = [];
        }
    }

    public enum ProgramMode : byte
    {
        Undefined = 0,
        Parser,
        ConfigRedacter
    }
}