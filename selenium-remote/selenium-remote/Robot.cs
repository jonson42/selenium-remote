using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utils;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace selenium_remote
{
    public class Robot
    {
        private IWebDriver _webDriver { get; set; }
        private string _appName { get; set; }
        private DriverManager _webDriverManager { get; set; }
        private Proxy _proxy {get;set;}
        public Robot(string appName, int x, int y,int width, int heigth,string proxy="")
        {
            _webDriverManager = new WebDriverManager.DriverManager();
            _webDriverManager.SetUpDriver(new ChromeConfig());
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            _proxy = new Proxy();
            if (!String.IsNullOrEmpty(proxy))
            {
                options.AddArguments(String.Format("--proxy-server={0}", proxy));
            }

            //options.AddArgument("headless");
            //options.AddArguments("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            _webDriver = new ChromeDriver(driverService, options);
            _appName = appName;
            Size site = new Size();
            site.Width = width;
            site.Height = heigth;
            _webDriver.Manage().Window.Size= site;
            Point point = new Point();
            point.X = x;
            point.Y = y;
            _webDriver.Manage().Window.Position = point;
        }
        
        public void GoToPage(string url,List<string> listSearch)
        {
            Random random = new Random();
            var itemRandom = random.Next(0, listSearch.Count);
            ChangeToFirstTab();
            if (url.EndsWith("{KeySearch}"))
            {
                url = url.Replace("{KeySearch}", listSearch[itemRandom]);
            }
            _webDriver.Navigate().GoToUrl(url);
        }

        public bool StartWithDictionNary(List<List<string>> list,Dictionary<string,string> dataFill,List<string> listSearch)
        {
            var loop = Int32.Parse(list[0][1].Replace(".0", ""));
            for(int i = 0; i < loop; i++)
            {
                foreach (var item in list)
                {
                    switch (item[0])
                    {
                        case "GoToPage":
                            GoToPage(item[1], listSearch);
                            break;
                        case "ClickClass":
                            CLickClass(item[1]);
                            break;
                        case "ClickId":
                            ClickId(item[1]);
                            //CloseDialog();
                            break;
                        case "ClickByJavascript":
                            ClickJavascript(item[1]);
                            break;
                        case "ClickByXPath":
                            CLickByXPath(item[1]);
                            break;
                        case "SetCookie":
                            SetCookie(item[1]);
                            break;
                        case "OpenNewTabWithLink":
                            OpenNewTabWithLink(item[1]);
                            break;
                        case "Sleep":
                            Sleep(item[1]);
                            break;
                        case "CloseOtherTab":
                            CloseOtherTab();
                            break;
                        case "InputDataByClass":
                            InputDataByClass(item[1], dataFill);
                            break;
                        case "InputDataById":
                            InputDataById(item[1], dataFill);
                            break;
                        case "ChangeToFirstTab":
                            ChangeToFirstTab();
                            break;
                        case "FindElementForExist"://UserJavascript
                            var result = FindElementForExistByXpath(item[1]);
                            if (!result)
                            {
                                goto Exit;
                            }
                            break;
                        case "UserJavascript":
                            UserJavascript(item[1], dataFill);
                            break;
                        case "ChangeToLastTab":
                            ChangeToLastTab();
                            break;
                        case "ResfeshPage":
                            ResfeshPage();
                            break;
                    }
                }
            }
            
            return true;
            Exit:
            return false;
        }
        private void CLickClass(string value)
        {
            try
            {
                _webDriver.FindElement(By.CssSelector(value)).Click();
            }
            catch(Exception ex)
            {

            }
            
        }
        private void CLickByXPath(string value)
        {
            try
            {
                _webDriver.FindElement(By.XPath(value)).Click();
            }
            catch (Exception ex)
            {

            }

        }
        private void ClickId(string value)
        {
            try
            {
                _webDriver.FindElement(By.Id(value)).Click();
            }
            catch
            {

            }

        }
        private void ClickJavascript(string value)
        {
            try
            {
                var valueStr = String.Format("document.evaluate(\"{0}\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click();", value);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_webDriver;
                js.ExecuteScript(valueStr);
            }
            catch
            {

            }

        }
        private void SetCookie(string value)
        {
            var cookieList = JsonConvert.DeserializeObject<CookieReq>(value);
            var listCookie = value.Split('\n');
            try
            {
                foreach (var item in cookieList.cookies)
                {
                    var itemName = item.name.Trim();
                    var itemValue = item.value.Trim();
                    Cookie cookie = new Cookie(itemName, itemValue);
                    _webDriver.Manage().Cookies.AddCookie(cookie);
                }
            }
            catch
            {

            }
        }

        private void OpenNewTabWithLink(string urlLink)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)_webDriver;
                js.ExecuteScript(String.Format("window.open('{0}', '_blank');", urlLink));
                _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
                _webDriver.Navigate().GoToUrl(urlLink);
            }
            catch 
            {

            }
        }
        private void InputDataByClass(string value,Dictionary<string, string> dataFill)
        {
            var className = value.Split('|')[0];
            var valueInput = value.Split('|')[1];
            dataFill.TryGetValue(valueInput, out valueInput);
            if (valueInput==null || valueInput.Length < 0)
            {
                valueInput = value.Split('|')[1];
            }
            
            try
            {
                _webDriver.FindElement(By.CssSelector(className)).SendKeys(valueInput);
            }
            catch(Exception ex )
            {

            }

        }

        private void ResfeshPage()
        {
            try
            {
                _webDriver.Navigate().Refresh();
            }
            catch
            {

            }
            
        }
        private void UserJavascript(string value, Dictionary<string, string> dataFill)
        {
            var actionCommand = value.Split('|')[0];
            var valueInput = "";
            if (value.Split('|').Length > 1)
            {
                 valueInput = value.Split('|')[1];
            }
            
            dataFill.TryGetValue(valueInput, out valueInput);
            if (value.Split('|').Length > 1 && (valueInput == null || valueInput.Length < 0))
            {
                valueInput = value.Split('|')[1];
            }

            try
            {
                //_webDriver.FindElement(By.CssSelector(className)).SendKeys(valueInput);
                var valueStr = String.Format(actionCommand, valueInput);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_webDriver;
                js.ExecuteScript(valueStr);
            }
            catch (Exception ex)
            {

            }
        }
        private void InputDataById(string value, Dictionary<string, string> dataFill)
        {
            var idName = value.Split('|')[0];
            var valueInput = value.Split('|')[1];
            dataFill.TryGetValue(valueInput, out valueInput);
            if (valueInput==null || valueInput.Length < 0)
            {
                valueInput = value.Split('|')[1];
            }
            try
            {
                _webDriver.FindElement(By.Id(idName)).SendKeys(valueInput);
            }
            catch
            {

            }
        }

        private bool FindElementForExistByXpath(string value)
        {
            try
            {
                if (_webDriver.FindElements(By.XPath(value)).Count > 0)
                {
                    CloseRobot();
                    return false;
                }
            }
            catch
            {

            }
            return true;
        }

        private void ChangeToFirstTab()
        {
            _webDriver.SwitchTo().Window(_webDriver.WindowHandles.First());
        }
        private void ChangeToLastTab()
        {
            _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
        }
        private void CloseOtherTab()
        {
            try
            {
                if (_webDriver.WindowHandles.Count >= 2)
                {
                    _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
                    _webDriver.Close();
                    _webDriver.SwitchTo().Window(_webDriver.WindowHandles.First());
                }
                
            }
            catch
            {
                
            }
        }

        public async Task StartWithAppNameAsync()
        {
            SeleniumClient client = new SeleniumClient();
            var data = await client.GetAsync<object>("todos/1");
        }

        private void Sleep(string value)
        {
            value = value.Replace(".", "").Replace(",", "");
            var valueInt = Int32.Parse(value);
            Task.Delay(valueInt).Wait();
        }
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        private static void CloseDialog()
        {
            var handle = FindWindow(null, "Spotify – Trình phát trên web");
            SetForegroundWindow(handle);
            //send alt+f4 using sendkeys method
            System.Windows.Forms.SendKeys.SendWait("%{F4}");
        }

        public void CloseRobot()
        {
            try
            {
                _webDriver.Quit();
                _webDriver.Close();
            }
            catch 
            {

            }
            
        }
    }
}
