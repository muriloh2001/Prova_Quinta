using Microsoft.EntityFrameworkCore;
using Murilo_Henrique.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();



app.MapGet("/", () => "Hello World!");

//Funcionario Endpoints
app.MapPost("/funcionario/cadastrar", async (Funcionario funcionario, AppDbContext ctx) =>
{
    await ctx.AddAsync(funcionario);
    await ctx.SaveChangesAsync();
    return Results.Created($"/funcionario/{funcionario.Id}", funcionario);
});



app.MapGet("/funcionario/listar", (AppDbContext ctx) =>
    ctx.Funcionarios.Any() 
        ? Results.Ok(ctx.Funcionarios.ToList()) 
        : Results.NotFound("Nenhum Funcionário Encontrado")
);

app.MapDelete("/funcionario/deletar/{id}", ( int id,  AppDbContext ctx) =>
{
    Funcionario? funcionario = ctx.Funcionarios.Find(id);
    if (funcionario == null)
    {
        return Results.NotFound();
    }
    ctx.Funcionarios.Remove(funcionario);
    ctx.SaveChanges();
    return Results.Ok(funcionario);
});


//Folha Pgamento Endpoints
app.MapPost("/folhaPagamento/cadastrar", async (FolhaPagamento folhaPagamento, AppDbContext ctx) =>
{
    var funcionario = await ctx.Funcionarios.FindAsync(folhaPagamento.FuncionarioId);
    if (funcionario == null) return Results.NotFound("Funcionário Não Encontrado");

    folhaPagamento.Funcionario = funcionario;

    folhaPagamento.SalarioBruto = CalcularSalarioBruto(folhaPagamento);
    folhaPagamento.ImpostoIrrf = CalcularImpostoRenda(folhaPagamento.SalarioBruto);
    folhaPagamento.ImpostoInss = CalcularInss(folhaPagamento.SalarioBruto);
    folhaPagamento.ImpostoFgts = folhaPagamento.SalarioBruto * 0.08m;  
    folhaPagamento.SalarioLiquido = folhaPagamento.SalarioBruto - folhaPagamento.ImpostoIrrf - folhaPagamento.ImpostoInss;

    await ctx.AddAsync(folhaPagamento);
    await ctx.SaveChangesAsync();

    return Results.Created($"/folhaPagamento/{folhaPagamento.Id}", folhaPagamento);
});

decimal CalcularSalarioBruto(FolhaPagamento folhaPagamento) => folhaPagamento.Quantidade * folhaPagamento.Valor / folhaPagamento.Mes;

decimal CalcularImpostoRenda(decimal salarioBruto) =>
    salarioBruto switch
    {
        <= 1903.98m => 0,
        <= 2826.65m => salarioBruto * 0.075m,
        <= 3751.05m => salarioBruto * 0.15m,
        <= 4664.68m => salarioBruto * 0.225m,
        _ => salarioBruto * 0.275m
    };

decimal CalcularInss(decimal salarioBruto) =>
    salarioBruto switch
    {
        <= 1693.72m => salarioBruto * 0.08m,
        <= 2822.90m => salarioBruto * 0.09m,
        <= 5645.80m => salarioBruto * 0.11m,
        _ => 621.03m
    };

app.MapGet("/folhaPagamento/listar", async(AppDbContext ctx) =>{
    var folhaPagamentos = await ctx.FolhaPagamentos
                          .Include(f => f.Funcionario)
                          .ToListAsync();

    return folhaPagamentos.Any() 
        ? Results.Ok(folhaPagamentos) 
        : Results.NotFound("Nenhuma FolhaPagamento Encontrada Para o Mês e Ano Solicitado.");
});

app.MapGet("/folhaPagamento/buscar/{cpf}/{mes}/{ano}", async (string cpf, int mes, int ano, AppDbContext ctx) =>
{
    var folhaPagamento = await ctx.FolhaPagamentos
        .Include(f => f.Funcionario)
        .FirstOrDefaultAsync(f => f.Funcionario != null && 
                                  f.Funcionario.Cpf == cpf && 
                                  f.Mes == mes && f.Ano == ano);

    return folhaPagamento != null 
        ? Results.Ok(folhaPagamento) 
        : Results.NotFound("Nenhuma FolhaPagamento Encontrada");
});

app.MapGet("/folhaPagamento/filtrar/{mes}/{ano}", async (int mes, int ano, AppDbContext ctx) =>
{
    var folhaPagamentos = await ctx.FolhaPagamentos
                          .Include(f => f.Funcionario)
                          .Where(f => f.Mes == mes && f.Ano == ano)
                          .ToListAsync();

    return folhaPagamentos.Any() 
        ? Results.Ok(folhaPagamentos) 
        : Results.NotFound("Nenhuma FolhaPagamento Encontrada Para o Mês e Ano Solicitado.");
});

app.MapDelete("/folhapagamento/deletar/{id}", ( int id,  AppDbContext ctx) =>
{
    FolhaPagamento? folhaPagamento = ctx.FolhaPagamentos.Find(id);
    if (folhaPagamento == null)
    {
        return Results.NotFound();
    }
    ctx.FolhaPagamentos.Remove(folhaPagamento);
    ctx.SaveChanges();
    return Results.Ok(folhaPagamento);
});



app.Run();
