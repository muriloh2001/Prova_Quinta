namespace Murilo_Henrique.Models;
public class FolhaPagamento
{
    
    public int Id { get; set; }
    public decimal Valor { get; set; }            
    public int Quantidade { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal SalarioBruto { get; set; }     
    public decimal ImpostoIrrf { get; set; }     
    public decimal ImpostoInss { get; set; }     
    public decimal ImpostoFgts { get; set; }     
    public decimal SalarioLiquido { get; set; }   
    public Funcionario? Funcionario { get; set; }
    public int FuncionarioId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}