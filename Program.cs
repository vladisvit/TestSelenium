using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Threading;

namespace TestSelenium
{
  class Program
  {
    static void SetUpOptions(IWebDriver driver)
    {
      IOptions options = driver.Manage();
      driver.Url = "https://careers.veeam.com";
      options.Window.Maximize();
    }

    static void Main(string[] args)
    {

      int expectedResult;
      if (args.Length != 3 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || !int.TryParse(args[2], out expectedResult))
      {
        Console.WriteLine("Please use the tools with next agruments <0- value of the dropdown country> <1- value of the droupdown language> <2 value of the expectedResult (integer)>");
        return;
      }

      var country = args[0];
      var language = args[1];

      Console.WriteLine("Start the Chrome browser");
      var directoryInfo =Directory.GetParent(Environment.CurrentDirectory);
      var parent = directoryInfo.Parent.Parent.FullName;
      var pathToChromeDriver = Path.Combine(parent, "chrome");
      IWebDriver driver = new ChromeDriver(pathToChromeDriver);
      SetUpOptions(driver);

      SetUpElement(driver, "#country-element", By.CssSelector($"span[data-value='{country}']"));
      SetUpElement(driver, "#language", By.XPath($"//label[text()[contains(.,'{language}')]]/span"));

      driver.ExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight-600)");
      var buttonAllJob = driver.FindElement(By.XPath("//a[contains(@class, 'content-loader-button load-more-button')]"));
      if (buttonAllJob.Displayed)
      {
        buttonAllJob.Click();
      }

      // this code does not work driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
      Thread.Sleep(3000); // only way to wait loading all job elements

      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
      var resultFindedJobs = wait.Until(e => e.FindElements(By.XPath("//div[@class='container']//div[contains(@class, 'vacancies-blocks-col')]")));
      Console.WriteLine($"TotalJobs:{resultFindedJobs.Count}");

      if (resultFindedJobs.Count == expectedResult)
      {
        Console.WriteLine("Find all job elements");
      }
      else
      {
        Console.WriteLine("Wrong result!!!");
      }

      driver.Quit();
    }

    private static void SetUpElement(IWebDriver driver, string elementId, By by)
    {
      IWebElement webElement = driver.FindElement(By.CssSelector(elementId));
      driver.ExecuteJavaScript("arguments[0].scrollIntoView();", webElement);
      webElement.Click();
      if (webElement.Displayed)
      {
        IWebElement element = driver.FindElement(by);
        driver.ExecuteJavaScript("arguments[0].scrollIntoView();", element);
        if (element.Displayed)
        {
          element.Click();
        }
      }
    }
  }
}
