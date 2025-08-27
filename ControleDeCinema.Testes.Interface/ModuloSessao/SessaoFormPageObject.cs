using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

public class SessaoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SessaoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));

        try
        {
            wait.Until(d =>
                d.FindElement(By.CssSelector(cssSelectorToFind: "form[data-se='form']")).Displayed);
        }
        catch (WebDriverTimeoutException)
        {
            DumpOnFailure(driver, "sessao-timeout");
            throw;
        }
    }

    public SessaoFormPageObject PreencherInicio(string inicio)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='inputInicio']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='inputInicio']")).Enabled
        );

        IWebElement input = driver.FindElement(By.CssSelector("input[data-se='inputInicio']"));

        DateTime dt = DateTime.ParseExact(
            inicio, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

        string ano = dt.ToString("yyyy", CultureInfo.InvariantCulture);
        string mes = dt.ToString("MM", CultureInfo.InvariantCulture);
        string dia = dt.ToString("dd", CultureInfo.InvariantCulture);
        string hora = dt.ToString("HH", CultureInfo.InvariantCulture);
        string min = dt.ToString("mm", CultureInfo.InvariantCulture);

        // Importante fazer nessa sequencia por conta do input type=date Chrome-PTBR
        input.Click();

        input.SendKeys(dia);

        input.SendKeys(mes);

        input.SendKeys(ano);
        input.SendKeys(Keys.ArrowRight);

        input.SendKeys(hora);

        input.SendKeys(min);

        input.SendKeys(Keys.Tab);

        return this;
    }

    public SessaoFormPageObject PreencherNumeroMaximoIngressos(int numeroMaximoIngressos)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='inputNumeroMaximoIngressos']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='inputNumeroMaximoIngressos']")).Enabled
        );

        IWebElement inputNumeroMaximoIngressos = driver.FindElement(By.CssSelector("input[data-se='inputNumeroMaximoIngressos']"));
        inputNumeroMaximoIngressos.Clear();
        inputNumeroMaximoIngressos.SendKeys(numeroMaximoIngressos.ToString());

        return this;
    }

    public SessaoFormPageObject SelecionarFilme(string tituloFilme)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("select[data-se='selectFilme']")).Displayed &&
            d.FindElement(By.CssSelector("select[data-se='selectFilme']")).Enabled
        );

        SelectElement selectFilme = new(driver.FindElement(By.CssSelector("select[data-se='selectFilme']")));

        wait.Until(_ => selectFilme.Options.Any(o => o.Text == tituloFilme));

        selectFilme.SelectByText(tituloFilme);

        return this;
    }

    public SessaoFormPageObject SelecionarSala(int numeroSala)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("select[data-se='selectSala']")).Displayed &&
            d.FindElement(By.CssSelector("select[data-se='selectSala']")).Enabled
        );

        SelectElement selectSala = new(driver.FindElement(By.CssSelector("select[data-se='selectSala']")));

        wait.Until(_ => selectSala.Options.Any(o => o.Text == numeroSala.ToString()));

        selectSala.SelectByText(numeroSala.ToString());

        return this;
    }

    public SessaoIndexPageObject ClickSubmit()
    {
        string url = driver.Url;

        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new(driver);
    }

    public SessaoIndexPageObject ClickSubmitExcluir(string tituloFilme)
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        wait.Until(d => !d.PageSource.Contains(tituloFilme));

        return new(driver);
    }

    public SessaoIndexPageObject ClickSubmitEncerrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnEncerrar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes/detalhes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnVoltar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));

        return new(driver);
    }

    public SessaoFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
        {
            bool segueNoCadastro = d.Url.Contains("/sessoes/cadastrar", StringComparison.OrdinalIgnoreCase) &&
                                   d.FindElement(By.CssSelector("form[data-se='form']")).Displayed;

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNoCadastro && (temMensagemValidacao || temMensagemAlerta);
        });

        return this;
    }

    public bool EstourouValidacao(string nomeCampo = "")
    {
        if (!string.IsNullOrWhiteSpace(nomeCampo))
        {
            IWebElement span = driver.FindElement(By.CssSelector($"span[data-valmsg-for='{nomeCampo}']"));
            if (!string.IsNullOrWhiteSpace(span.Text?.Trim()))
                return true;
        }

        ReadOnlyCollection<IWebElement> alerts = driver.FindElements(By.CssSelector("div.alert[role='alert']"));
        return alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));
    }

    private static void DumpOnFailure(IWebDriver driver, string prefix)
    {
        try
        {
            Screenshot shot = ((ITakesScreenshot)driver).GetScreenshot();
            string png = Path.Combine(Path.GetTempPath(), $"{prefix}-{DateTime.Now:HHmmss}.png");
            shot.SaveAsFile(png);

            string html = Path.Combine(Path.GetTempPath(), $"{prefix}-{DateTime.Now:HHmmss}.html");
            File.WriteAllText(html, driver.PageSource);
        }
        catch { /* best-effort */ }
    }
}
