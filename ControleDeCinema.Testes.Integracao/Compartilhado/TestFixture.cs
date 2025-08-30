using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected ControleDeCinemaDbContext dbContext;

    protected RepositorioFilmeEmOrm RepositorioFilmeEmOrm;
    protected RepositorioGeneroFilmeEmOrm RepositorioGeneroFilmeEmOrm;
    protected RepositorioIngressoEmOrm RepositorioIngressoEmOrm;
    protected RepositorioSalaEmOrm RepositorioSalaEmOrm;
    protected RepositorioSessaoEmOrm RepositorioSessaoEmOrm;

    private static IDatabaseContainer? dbContainer;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("controle-de-cinema-db-testes")
            .WithDatabase("ControleDeCinemaDbTestes")
            .WithUsername("postgres")
            .WithPassword("SenhaSuperSecreta")
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync();
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public virtual void ConfigurarTestes()
    {
        if (dbContainer is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado corretamente.");

        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(dbContainer.GetConnectionString());

        ConfigurarTabelas(dbContext);

        RepositorioFilmeEmOrm = new RepositorioFilmeEmOrm(dbContext);
        RepositorioGeneroFilmeEmOrm = new RepositorioGeneroFilmeEmOrm(dbContext);
        RepositorioIngressoEmOrm = new RepositorioIngressoEmOrm(dbContext);
        RepositorioSalaEmOrm = new RepositorioSalaEmOrm(dbContext);
        RepositorioSessaoEmOrm = new RepositorioSessaoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Filme>(RepositorioFilmeEmOrm.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Filme>>(RepositorioFilmeEmOrm.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(RepositorioGeneroFilmeEmOrm.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<GeneroFilme>>(RepositorioGeneroFilmeEmOrm.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<Sala>(RepositorioSalaEmOrm.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Sala>>(RepositorioSalaEmOrm.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<Sessao>(RepositorioSessaoEmOrm.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Sessao>>(RepositorioSessaoEmOrm.CadastrarEntidades);
    }

    private static async Task InicializarBancoDadosAsync()
    {
        await dbContainer!.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        await dbContainer!.StopAsync();
        await dbContainer.DisposeAsync();
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Ingressos.RemoveRange(dbContext.Ingressos);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);
        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);

        dbContext.SaveChanges();
    }
}
