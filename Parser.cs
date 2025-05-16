using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ABIAParser;

public static class Parser
{
    private static FirefoxDriver? driver = null;

    public static void Init()
    {
        try
        {
            if (driver != null)
                Terminate();

            FirefoxOptions options = new();
            options.AddArgument("-headless");

            driver = new FirefoxDriver(options);
        }
        catch (Exception ex)
        {
            LogManager.Log($"(Parse Init Error) {ex.Message}");
        }
    }

    public static void Parse(out string urlContent, bool driverRestart)
    {
        if (driverRestart || driver == null)
            Init();

        urlContent = "2exFWD_OE";

        if (driver != null)
        {        
            driver.Navigate().GoToUrl("https://animebytes.tv/register/apply");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            urlContent = driver.FindElement(By.TagName("body")).Text;

            urlContent = Regex.Replace(urlContent, @"\s+", " ").Trim();
        }
        else
            LogManager.Log("(Parser Error) Firefox driver is null reference!");

        if (driverRestart)
            Terminate();
    }

    public static void Terminate()
    {   
        if (driver != null)     
            driver.Quit();
        else
            LogManager.Log("(Parser Warning) Firefox driver is null reference!");
        
        driver = null;
    }
}