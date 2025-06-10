CREATE SCHEMA itau;

USE itau;

CREATE TABLE usuarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    pct_corretagem DECIMAL(5,2) NOT NULL
);

CREATE TABLE ativos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    codigo VARCHAR(10) NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE operacoes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    qtd INT NOT NULL,
    preco_unit DECIMAL(15,4) NOT NULL,
    tipo_operacao ENUM('COMPRA', 'VENDA') NOT NULL,
    corretagem DECIMAL(10,2) NOT NULL,
    dt_hr DATETIME NOT NULL,
    
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    FOREIGN KEY (ativo_id) REFERENCES ativos(id),
    INDEX idx_usuario_ativo_data (usuario_id, ativo_id, dt_hr)
);

CREATE TABLE cotacoes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ativo_id INT NOT NULL,
    preco_unit DECIMAL(15,4) NOT NULL,
    dt_hr DATETIME(3) NOT NULL, -- milissegundos
    
    FOREIGN KEY (ativo_id) REFERENCES ativos(id),
    INDEX idx_ativo_data (ativo_id, dt_hr)
);

CREATE TABLE posicoes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    qtd INT NOT NULL,
    preco_medio DECIMAL(15,4) NOT NULL,
    pl DECIMAL(15,2) NOT NULL, -- Profit & Loss
    
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    FOREIGN KEY (ativo_id) REFERENCES ativos(id),
    UNIQUE KEY uk_usuario_ativo (usuario_id, ativo_id)
);
