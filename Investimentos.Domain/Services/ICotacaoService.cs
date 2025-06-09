
namespace Investimentos.Domain.Services
{
    public interface ICotacaoService
    {
        Task ProcessarCotacaoKafkaAsync(string mensagem);
    }
}
