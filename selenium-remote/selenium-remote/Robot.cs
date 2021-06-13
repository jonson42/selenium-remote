using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Robot(string appName)
        {
            _webDriverManager = new WebDriverManager.DriverManager();
            _webDriverManager.SetUpDriver(new ChromeConfig());
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddArguments("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            _webDriver = new ChromeDriver(options);
            _appName = appName;
        }
        public void GoToPage(string url)
        {
            _webDriver.Navigate().GoToUrl(url);
        }

        public void StartWithDictionNary(Dictionary<string,string> list)
        {
            for(var i = 0; i < Int32.Parse(list["Repeat"]);i++)
            {
                foreach (var item in list)
                {
                    switch (item.Key)
                    {
                        case "GoToPage":
                            GoToPage(item.Value);
                            break;
                        case "ClickClass":
                            CLickClass(item.Value);
                            break;
                        case "ClickId":
                            ClickId(item.Value);
                            break;
                        case "ClickByJavascript":
                            ClickJavascript(item.Value);
                            break;
                        case "SetCookie":
                            SetCookie(item.Value);
                            break;
                        case "OpenNewTabWithLink":
                            OpenNewTabWithLink(item.Value);
                            break;
                    }
                }
            }
            
        }
        private void CLickClass(string value)
        {
            try
            {
                _webDriver.FindElement(By.ClassName(value)).Click();
            }
            catch
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
                IJavaScriptExecutor js = (IJavaScriptExecutor)_webDriver;
                js.ExecuteScript(value);
            }
            catch
            {

            }

        }
        private void SetCookie(string value)
        {
            var listCookie = value.Split('\n');
            try
            {
                foreach (var item in listCookie)
                {
                    var itemLine = item.Trim();
                    var itemName = itemLine.Split(' ')[0].Trim();
                    var itemValue = itemLine.Split(' ')[1].Trim();
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
                _webDriver.Navigate().GoToUrl(urlLink);
            }
            catch 
            {

            }
           
        }
        private void CloseOtherTab()
        {
            try
            {
                _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
                _webDriver.Close();
                _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
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
    }
}
