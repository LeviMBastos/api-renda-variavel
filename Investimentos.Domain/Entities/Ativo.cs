namespace Investimentos.Domain.Entities
{
    public class Ativo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nome { get; set; } = null!;

        public ICollection<Operacao> Operacoes { get; set; } = new List<Operacao>();
        public ICollection<Cotacao> Cotacoes { get; set; } = new List<Cotacao>();
        public ICollection<Posicao> Posicoes { get; set; } = new List<Posicao>();
    }
}
