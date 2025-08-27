using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

public class IngressoIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public IngressoIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public IngressoIndexPageObject IrParaSessoesComoCliente(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/sessoes");

        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));

        return this;
    }
}
