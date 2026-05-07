# 💰 Investment Control System | Sistema de Controle de Investimentos

<div align="center">

**Escolha o idioma / Choose language:**

[![pt-br](https://img.shields.io/badge/PT--BR-Português-green)](#pt-br) [![en-us](https://img.shields.io/badge/EN--US-English-blue)](#en-us)

</div>

---

<a name="pt-br"></a>

## 🇧🇷 Português (Brazilian Portuguese)

# 💰 Sistema de Controle de Investimentos

Este projeto simula um sistema completo para controle de investimentos em renda variável, com foco em **cálculo de posições**, **lucros/prejuízos (P&L)** e **integração com cotações em tempo real via Kafka**.

---

## 🚀 Como Rodar o Projeto

### 🧱 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://www.docker.com/)
- [MySQL](https://www.mysql.com/) rodando com:
  - **Usuário**: `root`
  - **Senha**: `root`

### ⚙️ Passos de Execução

1. **Subir o Kafka com Docker**

```bash
docker-compose up --build
```

2. **Executar a API principal**

```bash
cd Investimentos.Api
dotnet run
```

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

### 📦 Modelagem de Banco de Dados (MySQL)

Script de criação: [`script-criacao-tabelas.sql`](script-criacao-tabelas.sql)

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

### 🧮 Lógica de Negócio

- **Cálculo do Preço Médio Ponderado** implementado com validações.
- **Total Investido por Ativo**
- **P&L por Ativo**
- **P&L Global**
- **Total de Corretagem por Cliente**

Código disponível em `/Investimentos.Infra/Services`.

---

### ✅ Testes

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

### 🔁 Integração Kafka

- **Produtor**: `Investimentos.WorkerKafkaCotacoesProducer`
- **Consumidor (Worker .NET)**: `Investimentos.WorkerCotacaoConsumer`
- Estratégias incluídas:
  - **Retry automático**
  - **Idempotência**
  - **Persistência eficiente das cotações**

---

### 🧯 Engenharia do Caos

Caso o serviço de cotações falhe:
- **Circuit Breaker** impede chamadas consecutivas.
- **Fallback** mantém posição com último valor conhecido.
- **Observabilidade** com logs de falha e métricas de tentativa.

---

### 📈 Escalabilidade e Performance

- **Auto-scaling horizontal**: adição de instâncias com containers (Kubernetes, App Services etc.).
- **Balanceadores**:

| Estratégia          | Quando Usar                           | Prós                                   | Contras                                |
|---------------------|---------------------------------------|----------------------------------------|----------------------------------------|
| Round-robin         | Cargas uniformes e serviços stateless | Simples, fácil de implementar          | Não considera latência real            |
| Baseado em latência | Cargas variáveis e serviços pesados   | Otimiza resposta, evita gargalos       | Requer health checks e métricas reais  |

---

## 📁 Estrutura do Projeto

```plaintext
Investimentos.Api/                         --> API principal
Investimentos.Domain/                      --> Entidades e regras de domínio
Investimentos.Infra/                       --> Contexto EF Core, repositórios, serviços
Investimentos.Tests/                       --> Testes unitários
Investimentos.WorkerKafkaCotacoesProducer/ --> Produtor de cotações (Kafka)
Investimentos.WorkerCotacaoConsumer/       --> Worker consumidor de cotações
criacao_tabelas.sql                        --> Script de criação do banco
```

---

## Documentação da API (OpenAPI 3.0 JSON)

```json
{
  "openapi": "3.0.4",
  "info": {
    "title": "Investimentos API",
    "version": "v1"
  },
  "paths": {
    "/api/Ativo/ObterUltimaCotacao/{codigoAtivo}": {
      "get": {
        "tags": ["Ativo"],
        "parameters": [
          {
            "name": "codigoAtivo",
            "in": "path",
            "required": true,
            "schema": {"type": "string"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Investimentos/comprar": {
      "post": {
        "tags": ["Investimentos"],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {"$ref": "#/components/schemas/OperacaoCompraDto"}
            }
          }
        },
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Investimentos/preco-medio": {
      "get": {
        "tags": ["Investimentos"],
        "parameters": [
          {
            "name": "usuarioId",
            "in": "query",
            "schema": {"type": "integer", "format": "int32"}
          },
          {
            "name": "ativoId",
            "in": "query",
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/{usuarioId}": {
      "get": {
        "tags": ["Operacao"],
        "parameters": [
          {
            "name": "usuarioId",
            "in": "path",
            "required": true,
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/getTotalCorretagens": {
      "get": {
        "tags": ["Operacao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/getTop10ClientesPorCorretagemAsync": {
      "get": {
        "tags": ["Operacao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Posicao/getTop10ClientesPorPl": {
      "get": {
        "tags": ["Posicao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Usuario": {
      "post": {
        "tags": ["Usuario"],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {"$ref": "#/components/schemas/UsuarioCriacaoDto"}
            }
          }
        },
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Usuario/{id}": {
      "get": {
        "tags": ["Usuario"],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    }
  },
  "components": {
    "schemas": {
      "OperacaoCompraDto": {
        "type": "object",
        "properties": {
          "usuarioId": {"type": "integer", "format": "int32"},
          "codigoAtivo": {"type": "string", "nullable": true},
          "quantidade": {"type": "integer", "format": "int32"}
        },
        "additionalProperties": false
      },
      "UsuarioCriacaoDto": {
        "type": "object",
        "properties": {
          "nome": {"type": "string", "nullable": true},
          "email": {"type": "string", "nullable": true},
          "percentualCorretagem": {"type": "number", "format": "double"}
        },
        "additionalProperties": false
      }
    }
  }
}
```

---

## 📌 Considerações Finais

Este projeto visa cobrir não só os aspectos de desenvolvimento backend com .NET e MySQL, mas também aplicar **conceitos avançados de arquitetura, testes e resiliência**, frequentemente cobrados em entrevistas técnicas e necessários em ambientes de produção.

---

<a name="en-us"></a>

## 🇺🇸 English (English)

# 💰 Investment Control System

This project simulates a complete system for managing variable income investments, focusing on **position calculation**, **profit/loss (P&L)**, and **integration with real-time quotes via Kafka**.

---

## 🚀 How to Run the Project

### 🧱 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker + Docker Compose](https://www.docker.com/)
- [MySQL](https://www.mysql.com/) running with:
  - **User**: `root`
  - **Password**: `root`

### ⚙️ Execution Steps

1. **Start Kafka with Docker**

```bash
docker-compose up --build
```

2. **Run the main API**

```bash
cd Investimentos.Api
dotnet run
```

3. **Run the Quote Producer Worker (simulates external service)**

```bash
cd Investimentos.WorkerKafkaCotacoesProducer
dotnet run
```

4. **Run the Internal Worker that Consumes Quotes**

```bash
cd Investimentos.WorkerCotacaoConsumer
dotnet run
```

---

## 🧠 Technical Architecture

### 📦 Database Modeling (MySQL)

Creation script: [`script-criacao-tabelas.sql`](script-criacao-tabelas.sql)

Main tables:

- `usuarios` (id, name, email, brokerage_pct)
- `ativos` (id, code, name)
- `operacoes` (id, user_id, asset_id, qty, unit_price, operation_type, brokerage, dt_hr)
- `cotacoes` (id, asset_id, unit_price, dt_hr)
- `posicoes` (id, user_id, asset_id, qty, avg_price, pl)

**Type Justification**:
- `DECIMAL` with precision for financial calculations.
- `DATETIME(3)` to store milliseconds in quotes.
- `ENUM` to guarantee operation type (BUY/SELL).
- Composite indexes optimizing critical queries.

---

### 2. ⚡ Indexes and Performance

#### Proposed Index:
```sql
CREATE INDEX idx_usuario_ativo_data
    ON operacoes (usuario_id, ativo_id, dt_hr DESC);
```

#### Optimized Query:
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

#### Position Update (C#):
```csharp
public async Task UpdatePositionAsync(int assetId, decimal newQuote)
{
    var positions = await _context.Positions
        .Where(p => p.AssetId == assetId)
        .ToListAsync();

    foreach (var position in positions)
    {
        position.PL = (newQuote - position.AveragePrice) * position.Quantity;
    }

    await _context.SaveChangesAsync();
}
```

---

### 🧮 Business Logic

- **Weighted Average Price Calculation** implemented with validations.
- **Total Invested per Asset**
- **P&L per Asset**
- **Global P&L**
- **Total Brokerage per Client**

Code available in `/Investimentos.Infra/Services`.

---

### ✅ Testing

#### Unit Tests (xUnit):
- Positive cases
- Error cases (empty list, zero quantity)

#### Mutation Tests:
> Mutation tests validate the effectiveness of unit tests by intentionally injecting failures.

🧪 **Mutation Example**:
```diff
// Original
valueWithBrokerage = (c.Qty * c.UnitPrice) + c.Brokerage;
// Mutant
valueWithBrokerage = (c.Qty * c.UnitPrice) - c.Brokerage;
```

If tests don't fail with the mutant, they aren't properly covering the logic.

---

### 🔁 Kafka Integration

- **Producer**: `Investimentos.WorkerKafkaCotacoesProducer`
- **Consumer (Worker .NET)**: `Investimentos.WorkerCotacaoConsumer`
- Included strategies:
  - **Automatic Retry**
  - **Idempotency**
  - **Efficient Quote Persistence**

---

### 🧯 Chaos Engineering

If the quote service fails:
- **Circuit Breaker** prevents consecutive calls.
- **Fallback** maintains position with last known value.
- **Observability** with failure logs and attempt metrics.

---

### 📈 Scalability and Performance

- **Horizontal auto-scaling**: adding instances with containers (Kubernetes, App Services, etc.).
- **Load Balancers**:

| Strategy          | When to Use                          | Pros                                   | Cons                                   |
|-------------------|--------------------------------------|----------------------------------------|----------------------------------------|
| Round-robin       | Uniform loads and stateless services | Simple, easy to implement              | Doesn't consider real latency          |
| Latency-based     | Variable loads and heavy services    | Optimizes response, avoids bottlenecks | Requires health checks and real metrics|

---

## 📁 Project Structure

```plaintext
Investimentos.Api/                         --> Main API
Investimentos.Domain/                      --> Entities and domain rules
Investimentos.Infra/                       --> EF Core context, repositories, services
Investimentos.Tests/                       --> Unit tests
Investimentos.WorkerKafkaCotacoesProducer/ --> Quote producer (Kafka)
Investimentos.WorkerCotacaoConsumer/       --> Quote consumer worker
criacao_tabelas.sql                        --> Database creation script
```

---

## API Documentation (OpenAPI 3.0 JSON)

```json
{
  "openapi": "3.0.4",
  "info": {
    "title": "Investimentos API",
    "version": "v1"
  },
  "paths": {
    "/api/Ativo/ObterUltimaCotacao/{codigoAtivo}": {
      "get": {
        "tags": ["Ativo"],
        "parameters": [
          {
            "name": "codigoAtivo",
            "in": "path",
            "required": true,
            "schema": {"type": "string"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Investimentos/comprar": {
      "post": {
        "tags": ["Investimentos"],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {"$ref": "#/components/schemas/OperacaoCompraDto"}
            }
          }
        },
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Investimentos/preco-medio": {
      "get": {
        "tags": ["Investimentos"],
        "parameters": [
          {
            "name": "usuarioId",
            "in": "query",
            "schema": {"type": "integer", "format": "int32"}
          },
          {
            "name": "ativoId",
            "in": "query",
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/{usuarioId}": {
      "get": {
        "tags": ["Operacao"],
        "parameters": [
          {
            "name": "usuarioId",
            "in": "path",
            "required": true,
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/getTotalCorretagens": {
      "get": {
        "tags": ["Operacao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Operacao/getTop10ClientesPorCorretagemAsync": {
      "get": {
        "tags": ["Operacao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Posicao/getTop10ClientesPorPl": {
      "get": {
        "tags": ["Posicao"],
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Usuario": {
      "post": {
        "tags": ["Usuario"],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {"$ref": "#/components/schemas/UsuarioCriacaoDto"}
            }
          }
        },
        "responses": {"200": {"description": "OK"}}
      }
    },
    "/api/Usuario/{id}": {
      "get": {
        "tags": ["Usuario"],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {"type": "integer", "format": "int32"}
          }
        ],
        "responses": {"200": {"description": "OK"}}
      }
    }
  },
  "components": {
    "schemas": {
      "OperacaoCompraDto": {
        "type": "object",
        "properties": {
          "usuarioId": {"type": "integer", "format": "int32"},
          "codigoAtivo": {"type": "string", "nullable": true},
          "quantidade": {"type": "integer", "format": "int32"}
        },
        "additionalProperties": false
      },
      "UsuarioCriacaoDto": {
        "type": "object",
        "properties": {
          "nome": {"type": "string", "nullable": true},
          "email": {"type": "string", "nullable": true},
          "percentualCorretagem": {"type": "number", "format": "double"}
        },
        "additionalProperties": false
      }
    }
  }
}
```

---

## 📌 Final Considerations

This project aims to cover not only backend development aspects with .NET and MySQL, but also apply **advanced concepts of architecture, testing, and resilience**, frequently required in technical interviews and necessary in production environments.

---

<div align="center">

[⬆ Back to top](#-investment-control-system--sistema-de-controle-de-investimentos) | [Voltar ao topo](#pt-br)

</div>
