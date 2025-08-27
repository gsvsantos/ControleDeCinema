using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

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

    public SessaoIndexPageObject ClickDetalhes()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnDetalhes']"))).Click();

        return this;
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

    public bool ContemInicio(string inicio)
    {
        DateTime dt = DateTime.ParseExact(
            inicio, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

        return driver.PageSource.Contains(dt.ToString());
    }

    public bool ContemMaxIngressos(int numeroMaximoIngressos)
    {
        return driver.PageSource.Contains(numeroMaximoIngressos.ToString());
    }

    public bool ContemFilme(string tituloFilme)
    {
        return driver.PageSource.Contains(tituloFilme);
    }

    public bool ContemSala(int numeroSala)
    {
        return driver.PageSource.Contains(numeroSala.ToString());
    }

    public bool ContemStatus()
    {
        return driver.PageSource.Contains("Encerrada") || driver.PageSource.Contains("Aberta");
    }
}
