using ControleDeCinema.Testes.Interface.ModuloSessao;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloIngresso;

public class IngressoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public IngressoFormPageObject(IWebDriver driver)
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
            DumpOnFailure(driver, "ingresso-timeout");
            throw;
        }
    }

    public IngressoFormPageObject SelecionarAssento(int assento)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("select[data-se='selectAssento']")).Displayed &&
            d.FindElement(By.CssSelector("select[data-se='selectAssento']")).Enabled
        );

        SelectElement selectAssento = new(driver.FindElement(By.CssSelector("select[data-se='selectAssento']")));

        wait.Until(_ => selectAssento.Options.Any(o => o.Text == assento.ToString() || o.GetAttribute("value") == assento.ToString()));

        selectAssento.SelectByText(assento.ToString());

        return this;
    }

    public IngressoFormPageObject MarcarMeiaEntrada()
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='checkboxMeiaEntrada']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='checkboxMeiaEntrada']")).Enabled
        );

        IWebElement checkboxMeiaEntrada = driver.FindElement(By.CssSelector("input[data-se='checkboxMeiaEntrada']"));

        if (!checkboxMeiaEntrada.Selected)
            checkboxMeiaEntrada.Click();

        return this;
    }

    public SessaoIndexPageObject ClickSubmitComoCliente()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));

        return new(driver);
    }

    public IngressoFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
        {
            bool segueNoFormulario =
                d.FindElement(By.CssSelector("form[data-se='form']")).Displayed;

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNoFormulario && (temMensagemValidacao || temMensagemAlerta);
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
