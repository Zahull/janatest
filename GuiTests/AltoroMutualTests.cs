using System;
using System.Globalization;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using Structura.GuiTests.PageObjects;
using Structura.GuiTests.SeleniumHelpers;
using Structura.GuiTests.Utilities;
using Tests.PageObjects;

namespace Structura.GuiTests
{
    [TestFixture]
    public class AltoroMutualTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseUrl;

        [SetUp]
        public void SetupTest()
        {
            driver = new DriverFactory().Create();
            baseUrl = ConfigurationHelper.Get<string>("TargetUrl");
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
                driver.Close();
            }
            catch (Exception)
            {
                // Ignore errors if we are unable to close the browser
            }
            verificationErrors.ToString().Should().BeEmpty("No verification errors are expected.");
        }

        [Test]
        public void LoginWithValidCredentialsShouldSucceed()
        {
            // Arrange
            // Act
            new LoginPage(driver).LoginAsAdmin(baseUrl);

            // Assert
            new MainPage(driver).GetAccountButton.Displayed.Should().BeTrue();
        }

        [Test]
        public void LoginWithInvalidCredentialsShouldFail()
        {
            // Arrange
            // Act
            new LoginPage(driver).LoginAsNobody(baseUrl);

            // Assert
            Action a = () =>
            {
                var displayed = new MainPage(driver).GetAccountButton.Displayed; // throws exception if not found
            };
            a.ShouldThrow<NoSuchElementException>().WithMessage("Could not find element by: By.Id: btnGetAccount");
        }
        
        [Test]
        public void RequestGoldenVisaShouldBeAccepted()
        {
            // Arrange
            new LoginPage(driver).LoginAsAdmin(baseUrl);
            var page = new RequestGoldVisaPage(driver);
            new MainPage(driver).NavigateToTransferFunds();

            // Act
            page.PerformRequest();

            // Assert

            // Need to wait until the results are displayed on the web page
            Thread.Sleep(500);
            
            page.SuccessMessage.Text.StartsWith(
                "Your new Altoro Mutual Gold VISA with a $10000 and 7.9% APR will be sent in the mail."
                , true, CultureInfo.InvariantCulture).Should().BeTrue();
        }
    }
}


