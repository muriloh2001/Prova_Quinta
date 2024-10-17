namespace Murilo_Henrique.Models;

public class Funcionario {

	public int Id { get; set; }
	public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}