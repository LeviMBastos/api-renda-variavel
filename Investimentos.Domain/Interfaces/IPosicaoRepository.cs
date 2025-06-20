﻿using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Domain.Interfaces
{
    public interface IPosicaoRepository
    {
        Task<List<UsuarioPlDto>> ObterTop10ClientesPorPlAsync();
        Task<Posicao?> GetPosicaoAsync(int usuarioId, int ativoId);
        Task AddAsync(Posicao posicao);
        Task UpdateAsync(Posicao posicao);
    }
}
