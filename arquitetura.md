# Arquitetura do CartolaLigas

**Versão:** 2.0  
**Última atualização:** 2025-08-20  
**Autor:** edsonlcandido

## Visão Geral

O CartolaLigas é uma aplicação para gerenciamento de ligas do Cartola FC, o fantasy game de futebol brasileiro. A arquitetura é baseada em três componentes principais:

1. **Frontend**: Interface de usuário para interação com o sistema
2. **N8N Middleware**: Camada de orquestração e integração para fluxos de trabalho complexos
3. **PocketBase Backend**: API e banco de dados para persistência, autenticação e regras de negócio básicas

## Diagrama de Arquitetura

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│             │       │             │       │             │       │             │
│  Frontend   │◄─────►│    N8N      │◄─────►│  PocketBase │◄─────►│   SQLite    │
│             │       │  Middleware │       │   Backend   │       │  Database   │
└─────────────┘       └──────┬──────┘       └─────────────┘       └─────────────┘
                             │
                             ▼
                      ┌─────────────┐
                      │             │
                      │ API Cartola │
                      │     FC      │
                      │             │
                      └─────────────┘
```

## Componentes Principais

### 1. Frontend

O frontend é responsável pela interface com o usuário, permitindo a criação de ligas, adição de times e visualização de rankings e estatísticas.

### 2. N8N Middleware

O N8N serve como middleware de orquestração e integração, desempenhando funções críticas:

- **API Gateway**: Expõe endpoints RESTful para o frontend
- **Orquestrador de Workflows**: Implementa fluxos de trabalho complexos
- **Integrador de APIs**: Comunica com a API do Cartola FC
- **Processador de Filas**: Executa tarefas assíncronas para atualização de dados

### 3. PocketBase Backend

O PocketBase fornece:

- **Banco de Dados**: SQLite para persistência de dados
- **Autenticação**: Sistema de gestão de usuários e autenticação JWT
- **API RESTful**: Endpoints automáticos para as coleções
- **Sistema de Permissões**: Controle de acesso granular aos recursos

## Modelo de Dados

### Coleções Principais no PocketBase

1. **users**: Usuários do sistema
   - Campos padrão: id, email, password, verified, etc.
   - Campos personalizados: name, avatar, role (user/admin), max_leagues

2. **ligas**: Ligas criadas pelos usuários
   - id: Identificador único
   - user_id: Relação com o usuário criador
   - name: Nome da liga
   - slug: Identificador amigável para URLs

3. **times**: Times dos usuários
   - id: Identificador único
   - slug: Identificador amigável para URLs
   - name: Nome do time no sistema
   - user_id: Relação com o usuário dono
   - cartola_time_id: ID do time no Cartola FC
   - nome_cartola: Nome do time no Cartola FC

4. **ligas_times**: Associação entre ligas e times
   - liga_id: Relação com a liga
   - time_id: Relação com o time

5. **times_pontuacao**: Pontuações dos times por rodada
   - time: Relação com o time
   - rodada: Número da rodada
   - pontos: Pontuação na rodada
   - cartoletas: Patrimônio em cartoletas

6. **job_queue**: Fila de trabalhos para processamento
   - time_id: Relação com o time
   - rodada: Número da rodada
   - status: Estado do trabalho (aguardando, processando, finalizado, erro)

7. **time_rodada_aguardando** (View): Times aguardando processamento

## Fluxos Principais

### 1. Registro e Autenticação

- Usuário se registra através do N8N, que cria o registro no PocketBase
- Sistema envia email de verificação
- Usuário se autentica recebendo um token JWT

### 2. Criação e Gestão de Ligas

- Usuário cria ligas através do N8N (limitado por max_leagues)
- Adiciona times à liga através da integração com a API do Cartola FC
- O N8N manipula os registros correspondentes no PocketBase

### 3. Atualização de Pontuações

Este é um fluxo crítico que utiliza um sistema de filas:

```
┌────────────┐     ┌────────────┐     ┌────────────┐
│ Cliente/   │     │    N8N     │     │ PocketBase │
│ Agendador  ├────►│ Enfileira  ├────►│ Armazena   │
│            │     │ times      │     │ na fila    │
└────────────┘     └────────────┘     └────────────┘
                         │                  
                         ▼                  
                   ┌────────────┐     ┌────────────┐
                   │    N8N     │     │ API Cartola│
                   │ Processa   ├────►│ Obtém      │
                   │ fila       │     │ pontuações │
                   └────────────┘     └────────────┘
                         │                  
                         ▼                  
                   ┌────────────┐          
                   │ PocketBase │          
                   │ Atualiza   │          
                   │ pontuações │          
                   └────────────┘          
```

#### Detalhamento do Processamento de Fila:

1. **Trigger Inicial**: Webhook `/ligas/v1/job-queue-created/` inicia o processamento
2. **Consulta de Times Pendentes**: Busca na view `time_rodada_aguardando`
3. **Processamento em Lote**: Para cada time:
   - Consulta a API do Cartola FC para obter pontuações
   - Salva os dados na coleção `times_pontuacao`
   - Marca o job como "finalizado" no PocketBase
4. **Recursão**: Após processar um lote, o sistema chama novamente o webhook para continuar o processamento

## Integrações Externas

### API do Cartola FC

- **Endpoint de Times**: `/time/id/{cartola_time_id}/{rodada}`
- **Endpoint de Busca**: `/busca?q={query}`
- **Frequência de Atualização**: Durante rodadas, atualizações ocorrem a cada processamento da fila

## Segurança

### Autenticação

- Sistema baseado em JWT para autenticação
- Tokens com duração limitada e refresh
- Verificação de email obrigatória (authRule: "verified = true")

### Controle de Acesso

- N8N serve como camada intermediária para controle de acesso
- PocketBase implementa regras por coleção para proteção adicional
- Administradores têm acesso expandido às configurações

## Considerações de Escalabilidade

- **Processamento de Filas**: O sistema atual processa itens sequencialmente
- **Banco de Dados**: SQLite pode ser um gargalo para escalabilidade
- **Melhorias Futuras**:
  - Processamento paralelo controlado
  - Sistema de retry com backoff exponencial
  - Monitoramento de performance e erros

## Próximos Passos na Arquitetura

1. Implementação de sistema de cache para reduzir chamadas à API externa
2. Melhorias no processamento paralelo de filas
3. Sistema avançado de monitoramento e alertas
4. Estratégias de backup e recuperação automatizadas
5. Consideração de migração para banco de dados mais robusto em caso de crescimento
