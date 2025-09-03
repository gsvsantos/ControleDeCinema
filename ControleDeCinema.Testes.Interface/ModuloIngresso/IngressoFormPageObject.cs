using ControleDeCinema.Testes.Interface.ModuloSessao;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

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
        wait.Until(d =>
            d.FindElement(By.CssSelector("form[data-se='form']")).Displayed);
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
}
