using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

public class IngressoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public IngressoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        try
        {
            // Mantém o padrão do SessaoForm: espera o form aparecer
            wait.Until(d =>
                d.FindElement(By.CssSelector("form[data-se='form']")).Displayed
                || d.FindElement(By.TagName("form")).Displayed
            );
        }
        catch (WebDriverTimeoutException)
        {
            DumpOnFailure(driver, "ingresso-timeout");
            throw;
        }
    }

    public IngressoFormPageObject SelecionarAssento(int assento)
    {
        // Mantém padrão: tenta data-se e, se não houver, usa name="Assento"
        IWebElement selectEl = null!;
        wait.Until(d =>
        {
            selectEl = d.FindElements(By.CssSelector("select[data-se='selectAssento']")).FirstOrDefault()
                       ?? d.FindElements(By.Name("Assento")).FirstOrDefault();
            return selectEl is not null && selectEl.Displayed && selectEl.Enabled;
        });

        SelectElement select = new(selectEl);
        try { select.SelectByText(assento.ToString()); }
        catch (NoSuchElementException) { select.SelectByValue(assento.ToString()); }

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

    public SessaoIndexPageObject ClickSubmit()
    {
        string url = driver.Url;

        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new(driver);
    }

    public IngressoFormPageObject ClickSubmitEsperandoErros()
    {
        IWebElement btn = null!;
        wait.Until(d =>
        {
            btn = d.FindElements(By.CssSelector("button[data-se='btnConfirmar']")).FirstOrDefault()
                ?? d.FindElements(By.CssSelector("button[type='submit']")).FirstOrDefault();
            return btn is not null && btn.Displayed && btn.Enabled;
        });

        btn.Click();

        wait.Until(d =>
        {
            bool segueNoForm =
                d.Url.Contains("/ingresso", StringComparison.OrdinalIgnoreCase) &&
                (d.FindElements(By.CssSelector("form[data-se='form']")).Any(f => f.Displayed)
                 || d.FindElements(By.TagName("form")).Any(f => f.Displayed));

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNoForm && (temMensagemValidacao || temMensagemAlerta);
        });

        return this;
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
