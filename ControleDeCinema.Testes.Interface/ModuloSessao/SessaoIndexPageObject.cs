using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

public class SessaoIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SessaoIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public SessaoIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/sessoes");

        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return this;
    }

    public SessaoFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new(driver);
    }

    public SessaoFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnEditar']"))).Click();

        return new(driver);
    }

    public SessaoFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnExcluir']"))).Click();

        return new(driver);
    }

    public SessaoFormPageObject ClickEncerrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnEncerrar']"))).Click();

        return new(driver);
    }

    public SessaoFormPageObject ClickComprarIngresso()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnComprarIngresso']"))).Click();

        return new(driver);
    }

    public bool ContemSessao(string tituloFilme)
    {
        return driver.PageSource.Contains($"Sessão para {tituloFilme}");
    }
}
