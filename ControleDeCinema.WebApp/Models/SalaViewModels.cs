using ControleDeCinema.Dominio.ModuloSala;
using System.ComponentModel.DataAnnotations;

namespace ControleDeCinema.WebApp.Models;

public abstract class FormularioSalaViewModel
{
    [Required(ErrorMessage = "O campo \"Número\" é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Número\" precisa conter um valor acima de 0.")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "O campo \"Capacidade\" é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Capacidade\" precisa conter um valor acima de 0.")]
    public int Capacidade { get; set; }

    public static Sala ParaEntidade(FormularioSalaViewModel viewModel)
    {
        return new Sala(viewModel.Numero, viewModel.Capacidade);
    }
}

public class CadastrarSalaViewModel : FormularioSalaViewModel
{
    public CadastrarSalaViewModel() { }
}

public class EditarSalaViewModel : FormularioSalaViewModel
{
    public Guid Id { get; set; }

    public EditarSalaViewModel() { }

    public EditarSalaViewModel(Guid id, int numero, int capacidade) : this()
    {
        Id = id;
        Numero = numero;
        Capacidade = capacidade;
    }
}

public class ExcluirSalaViewModel
{
    public Guid Id { get; set; }
    public int Numero { get; set; }

    public ExcluirSalaViewModel() { }

    public ExcluirSalaViewModel(Guid id, int numero)
    {
        Id = id;
        Numero = numero;
    }
}

public class VisualizarSalasViewModel
{
    public List<DetalhesSalaViewModel> Registros { get; set; }

    public VisualizarSalasViewModel(List<Sala> Salas)
    {
        Registros = Salas
            .Select(DetalhesSalaViewModel.ParaDetalhesVm)
            .ToList();
    }
}

public class DetalhesSalaViewModel
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public int Capacidade { get; set; }

    public DetalhesSalaViewModel(Guid id, int numero, int capacidade)
    {
        Id = id;
        Numero = numero;
        Capacidade = capacidade;
    }

    public static DetalhesSalaViewModel ParaDetalhesVm(Sala Sala)
    {
        return new DetalhesSalaViewModel(
            Sala.Id,
            Sala.Numero,
            Sala.Capacidade
        );
    }
}
