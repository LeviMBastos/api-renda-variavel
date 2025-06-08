namespace Investimentos.Domain.Entities
{
    public class Posicao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AtivoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal PL { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public Ativo Ativo { get; set; } = null!;
    }
}
