using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloAutenticacao;

public class AutenticacaoIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public AutenticacaoIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public AutenticacaoFormPageObject IrParaRegistro(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/autenticacao/registro");

        wait.Until(d => d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("input[data-se='inputEmail']")).Displayed);

        return new(driver);
    }

    public AutenticacaoFormPageObject IrParaLogin(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/autenticacao/login");

        wait.Until(d => d.Url.Contains("/autenticacao/login", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("input[data-se='inputEmail']")).Displayed);

        return new(driver);
    }

    public AutenticacaoIndexPageObject FazerLogout(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}");

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
        wait.Until(d => d.FindElement(By.CssSelector("form[action='/autenticacao/logout']"))).Submit();

        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count == 0);

        return this;
    }

    public bool EstaLogado()
    {
        return wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
    }
}
