using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloAutenticacao;

public class AutenticacaoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public AutenticacaoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            wait.Until(d =>
                d.FindElement(By.CssSelector("input[data-se='inputEmail']")).Displayed &&
                d.FindElement(By.CssSelector("input[data-se='inputSenha']")).Displayed);
        }
        catch (WebDriverTimeoutException)
        {
            DumpOnFailure(driver, "autenticacao-timeout");
            throw;
        }
    }

    public AutenticacaoFormPageObject PreencherEmail(string email)
    {
        IWebElement input = wait.Until(d => d.FindElement(By.CssSelector("input[data-se='inputEmail']")));
        input.Clear();
        input.SendKeys(email);

        return this;
    }

    public AutenticacaoFormPageObject PreencherSenha(string senha)
    {
        IWebElement input = wait.Until(d => d.FindElement(By.CssSelector("input[data-se='inputSenha']")));
        input.Clear();
        input.SendKeys(senha);

        return this;
    }

    public AutenticacaoFormPageObject PreencherConfirmarSenha(string confirmarSenha)
    {
        IWebElement input = wait.Until(d => d.FindElement(By.CssSelector("input[data-se='inputConfirmarSenha']")));
        input.Clear();
        input.SendKeys(confirmarSenha);

        return this;
    }

    public AutenticacaoFormPageObject SelecionarTipoUsuario(string tipoConta)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("select[data-se='selectTipoUsuario']")).Displayed &&
            d.FindElement(By.CssSelector("select[data-se='selectTipoUsuario']")).Enabled
        );

        SelectElement selectTipoUsuario = new(driver.FindElement(By.CssSelector("select[data-se='selectTipoUsuario']")));

        wait.Until(_ => selectTipoUsuario.Options.Any(o => o.Text == tipoConta));

        selectTipoUsuario.SelectByText(tipoConta);

        return this;
    }

    public AutenticacaoIndexPageObject ClickSubmitRegistro()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
            !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) &&
            d.FindElements(By.CssSelector("form[action='/autenticacao/registro']")).Count == 0
        );
        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);

        return new(driver);
    }

    public AutenticacaoIndexPageObject ClickSubmitLogin()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
            !d.Url.Contains("/autenticacao/login", StringComparison.OrdinalIgnoreCase) &&
            d.FindElements(By.CssSelector("form[action='/autenticacao/login']")).Count == 0
        );
        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);

        return new(driver);
    }

    public AutenticacaoFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        wait.Until(d =>
        {
            bool segueNaAutenticacao = (d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) ||
                                        d.Url.Contains("/autenticacao/login", StringComparison.OrdinalIgnoreCase)) &&
                                        d.FindElement(By.CssSelector("form[data-se='form']")).Displayed;

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNaAutenticacao && (temMensagemValidacao || temMensagemAlerta);
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
