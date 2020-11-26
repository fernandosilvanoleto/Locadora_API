IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(450) NULL,
    [Email] nvarchar(max) NULL,
    [Endereco] nvarchar(max) NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Filmes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Genero] nvarchar(max) NULL,
    [Ano] int NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Filmes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EstoqueFilme] (
    [Id] int NOT NULL IDENTITY,
    [FilmeId] int NOT NULL,
    [Quantidade] int NOT NULL,
    CONSTRAINT [PK_EstoqueFilme] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EstoqueFilme_Filmes_FilmeId] FOREIGN KEY ([FilmeId]) REFERENCES [Filmes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Locacao] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [ClienteId1] int NULL,
    [FilmeId] int NOT NULL,
    [FilmeId1] int NULL,
    [DataAlocacao] datetime2 NOT NULL,
    [DataDevolucaoPrevista] datetime2 NOT NULL,
    [DataDevolucaoEntregaPeloCliente] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Locacao] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Locacao_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Locacao_Clientes_ClienteId1] FOREIGN KEY ([ClienteId1]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Locacao_Filmes_FilmeId] FOREIGN KEY ([FilmeId]) REFERENCES [Filmes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Locacao_Filmes_FilmeId1] FOREIGN KEY ([FilmeId1]) REFERENCES [Filmes] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Clientes_Nome] ON [Clientes] ([Nome]) WHERE [Nome] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_EstoqueFilme_FilmeId] ON [EstoqueFilme] ([FilmeId]);
GO

CREATE INDEX [IX_Locacao_ClienteId] ON [Locacao] ([ClienteId]);
GO

CREATE INDEX [IX_Locacao_ClienteId1] ON [Locacao] ([ClienteId1]);
GO

CREATE INDEX [IX_Locacao_FilmeId] ON [Locacao] ([FilmeId]);
GO

CREATE INDEX [IX_Locacao_FilmeId1] ON [Locacao] ([FilmeId1]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201125094755_PrimeiraMigration', N'5.0.0');
GO

COMMIT;
GO

