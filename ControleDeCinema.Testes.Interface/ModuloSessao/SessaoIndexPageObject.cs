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
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
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
        return driver.PageSource.Contains($"SessÃ£o para {tituloFilme}");
    }

    public bool ContemInicio(string inicio)
    {
        DateTime dataInicio = DateTime.ParseExact(inicio, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

        var dataFormatadaPtBr = dataInicio.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"));

        var dataFormatadaEnUs = dataInicio.ToString("M/d/yyyy h:mm:ss tt", new CultureInfo("en-US"));

        return driver.PageSource.Contains($"value=\"{dataFormatadaPtBr}\"") ||
               driver.PageSource.Contains($"value=\"{dataFormatadaEnUs}\"");
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

    public bool ContemIngressosVendidos(int quantidadeVendida)
    {
        IWebElement ingressosVendidos = wait.Until(d => d.FindElement(By.CssSelector("input[data-se='ingressosVendidos']")));
        int value = Convert.ToInt32(ingressosVendidos.GetAttribute(attributeName: "value"));

        return driver.PageSource.Contains("Ingressos vendidos:") && value == quantidadeVendida;
    }

    public bool ContemIngressosDisponiveis(int quantidadeDisponivel)
    {
        IWebElement ingressosDisponiveis = wait.Until(d => d.FindElement(By.CssSelector("input[data-se='ingressosDisponiveis']")));
        int value = Convert.ToInt32(ingressosDisponiveis.GetAttribute(attributeName: "value"));

        return driver.PageSource.Contains("Ingressos disponÃ­veis:") && value == quantidadeDisponivel;
    }
}
