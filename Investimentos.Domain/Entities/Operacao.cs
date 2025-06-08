namespace Investimentos.Domain.Entities
{
    public class Operacao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AtivoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string TipoOperacao { get; set; } = null!;
        public decimal Corretagem { get; set; }
        public DateTime DataHora { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public Ativo Ativo { get; set; } = null!;
    }
}
