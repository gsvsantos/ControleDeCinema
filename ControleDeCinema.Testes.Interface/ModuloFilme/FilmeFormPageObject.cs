using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

public class FilmeFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public FilmeFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));

        try
        {
            wait.Until(d =>
                d.FindElement(By.CssSelector("form[data-se='form']")).Displayed);
        }
        catch (WebDriverTimeoutException)
        {
            DumpOnFailure(driver, "filme-timeout");
            throw;
        }
    }

    public FilmeFormPageObject PreencherTitulo(string titulo)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='inputTitulo']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='inputTitulo']")).Enabled
        );

        IWebElement inputTitulo = driver.FindElement(By.CssSelector("input[data-se='inputTitulo']"));
        inputTitulo.Clear();
        inputTitulo.SendKeys(titulo);

        return this;
    }

    public FilmeFormPageObject PreencherDuracao(int duracao)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='inputDuracao']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='inputDuracao']")).Enabled
        );

        IWebElement inputDuracao = driver.FindElement(By.CssSelector("input[data-se='inputDuracao']"));
        inputDuracao.Clear();
        inputDuracao.SendKeys(duracao.ToString());

        return this;
    }

    public FilmeFormPageObject MarcarLancamento()
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='checkboxLancamento']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='checkboxLancamento']")).Enabled
        );

        IWebElement checkboxLancamento = driver.FindElement(By.CssSelector("input[data-se='checkboxLancamento']"));

        if (!checkboxLancamento.Selected)
            checkboxLancamento.Click();

        return this;
    }

    public FilmeFormPageObject SelecionarGenero(string descricao)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("select[data-se='selectGenero']")).Displayed &&
            d.FindElement(By.CssSelector("select[data-se='selectGenero']")).Enabled
        );

        SelectElement selectGenero = new(driver.FindElement(By.CssSelector("select[data-se='selectGenero']")));

        wait.Until(_ => selectGenero.Options.Any(o => o.Text == descricao));

        selectGenero.SelectByText(descricao);

        return this;
    }

    public FilmeIndexPageObject ClickSubmit()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new(driver);
    }

    public FilmeIndexPageObject ClickSubmitExcluir(string titulo)
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/filmes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        wait.Until(d => !d.PageSource.Contains(titulo));

        return new(driver);
    }

    public FilmeFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
        {
            bool segueNoCadastro = d.Url.Contains("/filmes/cadastrar", StringComparison.OrdinalIgnoreCase) &&
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
