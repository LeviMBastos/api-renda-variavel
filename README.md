# 💰 Sistema de Controle de Investimentos

Este projeto simula um sistema completo para controle de investimentos em renda variável, com foco em **cálculo de posições**, **lucros/prejuízos (P&L)** e **integração com cotações em tempo real via Kafka**. Ele abrange desde a modelagem do banco relacional até estratégias de escalabilidade e testes mutantes.

---

## 🚀 Como Rodar o Projeto

### 🧱 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://www.docker.com/)
- [MySQL](https://www.mysql.com/) rodando com:
  - **Usuário**: `root`
  - **Senha**: `root`

### ⚙️ Passos de Execução

1. **Subir o MySQL + Kafka com Docker**

```bash
docker-compose up --build
```

2. **Executar a API principal**

```bash
cd Investimentos.Api
dotnet run
```

A API será exposta por padrão em `https://localhost:5001` ou `http://localhost:5000`.

3. **Executar o Worker Produtor de Cotações (simula serviço externo)**

```bash
cd Investimentos.WorkerKafkaCotacoesProducer
dotnet run
```

4. **Executar o Worker Interno que Consome as Cotações**

```bash
cd Investimentos.WorkerCotacaoConsumer
dotnet run
```

---

## 🧠 Estrutura Técnica

### 1. 📦 Modelagem de Banco de Dados (MySQL)

Script de criação: [`/scripts/criacao_tabelas.sql`](scripts/criacao_tabelas.sql)

Principais tabelas:

- `usuarios` (id, nome, email, pct_corretagem)
- `ativos` (id, codigo, nome)
- `operacoes` (id, usuario_id, ativo_id, qtd, preco_unit, tipo_operacao, corretagem, dt_hr)
- `cotacoes` (id, ativo_id, preco_unit, dt_hr)
- `posicoes` (id, usuario_id, ativo_id, qtd, preco_medio, pl)

**Justificativa dos tipos**:
- `DECIMAL` com precisão para cálculos financeiros.
- `DATETIME(3)` para armazenar milissegundos em cotações.
- `ENUM` para garantir tipo da operação (COMPRA/VENDA).
- Índices compostos otimizando queries críticas.

---

### 2. ⚡ Índices e Performance

#### Índice Proposto:
```sql
CREATE INDEX idx_usuario_ativo_data
    ON operacoes (usuario_id, ativo_id, dt_hr DESC);
```

#### Consulta Otimizada:
```sql
SELECT 
    id, tipo_operacao, preco_unit, corretagem, dt_hr
FROM operacoes
WHERE 
    usuario_id = @UsuarioId
    AND ativo_id = @AtivoId
    AND dt_hr >= DATE_SUB(NOW(), INTERVAL 30 DAY)
ORDER BY dt_hr DESC;
```

#### Atualização de Posição (C#):
```csharp
public async Task AtualizarPosicaoAsync(int ativoId, decimal novaCotacao)
{
    var posicoes = await _context.Posicoes
        .Where(p => p.AtivoId == ativoId)
        .ToListAsync();

    foreach (var posicao in posicoes)
    {
        posicao.PL = (novaCotacao - posicao.PrecoMedio) * posicao.Quantidade;
    }

    await _context.SaveChangesAsync();
}
```

---

### 3. 🧮 Lógica de Negócio

- **Cálculo do Preço Médio Ponderado** implementado com validações.
- **Total Investido por Ativo**
- **P&L por Ativo**
- **P&L Global**
- **Total de Corretagem por Cliente**

Código disponível em `/Investimentos.Infra/Services`.

---

### 4. ✅ Testes

#### Unitários (xUnit):
- Casos positivos
- Casos de erro (lista vazia, quantidade zero)

#### Mutantes:
> Testes mutantes validam a eficácia dos testes unitários, injetando falhas propositalmente.

🧪 **Exemplo de mutação**:
```diff
// Original
valorComCorretagem = (c.Qtd * c.PrecoUnit) + c.Corretagem;
// Mutante
valorComCorretagem = (c.Qtd * c.PrecoUnit) - c.Corretagem;
```

Se os testes não falharem com o mutante, eles não estão cobrindo corretamente a lógica.

---

### 5. 🔁 Integração Kafka

- **Produtor**: `Investimentos.WorkerKafkaCotacoesProducer`
- **Consumidor (Worker .NET)**: `Investimentos.WorkerCotacaoConsumer`
- Estratégias incluídas:
  - **Retry automático**
  - **Idempotência**
  - **Persistência eficiente das cotações**

---

### 6. 🧯 Engenharia do Caos

Caso o serviço de cotações falhe:
- **Circuit Breaker** impede chamadas consecutivas.
- **Fallback** mantém posição com último valor conhecido.
- **Observabilidade** com logs de falha e métricas de tentativa.

---

### 7. 📈 Escalabilidade e Performance

- **Auto-scaling horizontal**: adição de instâncias com containers (Kubernetes, App Services etc.).
- **Balanceadores**:

| Estratégia          | Quando Usar                           | Prós                                   | Contras                                |
|---------------------|---------------------------------------|----------------------------------------|----------------------------------------|
| Round-robin         | Cargas uniformes e serviços stateless | Simples, fácil de implementar          | Não considera latência real            |
| Baseado em latência | Cargas variáveis e serviços pesados   | Otimiza resposta, evita gargalos       | Requer health checks e métricas reais  |

---

## 📁 Estrutura do Projeto

```plaintext
Investimentos.Api/                      --> API principal
Investimentos.Domain/                   --> Entidades e regras de domínio
Investimentos.Infra/                    --> Contexto EF Core, repositórios, serviços
Investimentos.WorkerKafkaCotacoesProducer/ --> Produtor de cotações (Kafka)
Investimentos.WorkerCotacaoConsumer/       --> Worker consumidor de cotações
tests/                                  --> Testes unitários
scripts/criacao_tabelas.sql             --> Script de criação do banco
```

---

## 📌 Considerações Finais

Este projeto visa cobrir não só os aspectos de desenvolvimento backend com .NET e MySQL, mas também aplicar **conceitos avançados de arquitetura, testes e resiliência**, frequentemente cobrados em desafios técnicos de alto nível.
