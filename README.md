# ICT Master Suite (.NET 8)

Reconstrucao profissional do ICT Master Suite com arquitetura em camadas, foco corporativo e base de crescimento para novos modulos sem acoplamento de UI com regras de negocio.

## Stack

- .NET 8
- WPF + MVVM
- Entity Framework Core + SQL Server Express
- Serilog (logging centralizado)
- FluentValidation
- ClosedXML
- QuestPDF

## Estrutura

```text
ICTMasterSuite.sln
src/
  ICTMasterSuite.Domain
  ICTMasterSuite.Application
  ICTMasterSuite.Infrastructure
  ICTMasterSuite.Presentation.Wpf
tests/
  ICTMasterSuite.Domain.Tests
  ICTMasterSuite.Application.Tests
```

## Diretrizes Arquiteturais

- `Domain`: entidades e contratos do core de negocio.
- `Application`: casos de uso, interfaces de servicos e validacoes.
- `Infrastructure`: EF Core, persistencia SQL Server, logging, integracoes e implementacoes concretas.
- `Presentation.Wpf`: shell da aplicacao, navegacao, tema e composicao de dependencias.

## Modulos iniciais contemplados

1. Autenticacao e controle de acesso
2. Gestao de usuarios e perfis
3. Finder de logs
4. Parser TRI e Agilent
5. Historico tecnico
6. Base de conhecimento
7. Configuracoes do sistema
8. Exportacao Excel/PDF
9. Auditoria
10. Sincronizacao offline (futura)

## Como executar

1. Instalar .NET 8 SDK e SQL Server Express.
2. Ajustar connection string em `src/ICTMasterSuite.Presentation.Wpf/appsettings.json`.
3. Rodar:

```bash
dotnet restore
dotnet build ICTMasterSuite.sln
dotnet run --project src/ICTMasterSuite.Presentation.Wpf
```

## Roadmap em Fases

### Fase 1 - Fundacao

- Solucao e projetos por camada
- DI, logging, excecao global, tema premium e shell de navegacao
- Contratos de servico e validacoes base

### Fase 2 - Autenticacao e Usuarios

- Entidades evoluidas: `User`, `Role`, `Permission`, `RolePermission`, `UserSession`
- Autenticacao com sessao local e hash seguro PBKDF2
- Fluxo WPF: login premium -> shell principal -> logout -> retorno ao login
- Gestao inicial de usuarios com listagem, busca, criacao, edicao e ativacao/inativacao
- Permissoes por modulo/acao com visibilidade de modulos no shell
- Auditoria minima de login/logout/gestao via Serilog

### Fase 3 - Finder e Parser

- Finder desacoplado com busca recursiva por `Directory.EnumerateFiles`
- Filtro de extensoes: `.csv`, `.dcl`, `.txt`, `.log`
- Regras de exclusao: ignora arquivos com `pass` e prefixo `p_`
- Parser TRI/Agilent com extracao basica (`serial`, `model`, `error`, `result`)
- Use case `SearchLogsWithAnalysisUseCase` orquestrando busca + analise

### Fase 4 - Historico e Base de Conhecimento

- Registro tecnico por ativo/equipamento
- Catalogo de procedimentos e falhas recorrentes
- Pesquisa semantica inicial e ligacao com historico

### Fase 5 - Relatorios, Updater e Refinamento Premium

- Exportacao corporativa Excel e PDF
- Refino visual dark/light completo
- Telemetria, hardening, instalador/updater e readiness para sync offline
