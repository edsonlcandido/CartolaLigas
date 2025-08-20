# Engenharia do CartolaLigas

**Versão:** 2.0  
**Última atualização:** 2025-08-20  
**Autor:** edsonlcandido

## Stack Tecnológica

### Backend Principal
- **PocketBase**: Solução backend tudo-em-um que fornece:
  - Banco de dados SQLite
  - Autenticação e autorização
  - API RESTful automática
  - SDK Admin para gerenciamento

### Middleware de Orquestração
- **N8N**: Plataforma de automação low-code:
  - Gerenciamento de workflows
  - Integrações com APIs externas
  - Sistema de filas e processamento assíncrono
  - Webhooks personalizados

### Frontend
- [A definir - Recomendação: SPA com React/Vue e TypeScript]
- Cliente HTTP para comunicação com endpoints do N8N
- Interface responsiva para desktop e mobile

### Integração Externa
- API do Cartola FC para obtenção de dados de times, jogadores e pontuações

## Estrutura do Projeto

```
/
├── backend/                      # Configurações do PocketBase
│   ├── pb_migrations/            # Migrações do banco de dados
│   ├── pb_hooks/                 # Hooks e lógica personalizada
│   └── pb_data/                  # Dados do PocketBase (não versionar)
│
├── n8n/                          # Configurações e workflows do N8N
│   ├── workflows/                # Arquivos JSON dos workflows
│   │   ├── auth_flow.json        # Fluxo de autenticação
│   │   ├── time_registration.json # Cadastro de times
│   │   ├── queue_processor.json  # Processador de filas
│   │   └── ...
│   └── credentials/              # Credenciais (não versionar)
│
├── frontend/                     # Código fonte do frontend
│   ├── src/
│   ├── public/
│   └── ...
│
└── docs/                         # Documentação
    ├── arquitetura.md
    └── engenharia.md
```

## Workflows N8N Implementados

### 1. Autenticação e Usuários
- **Registro de Usuários**: Cria novos usuários e dispara email de verificação
- **Renovação de Tokens**: Middleware para refresh de tokens JWT

### 2. Gestão de Times e Ligas
- **Busca de Times**: Integração com a API Cartola para busca de times
- **Vinculação de Times**: Associa times do Cartola aos usuários
- **Criação de Ligas**: Cria e gerencia ligas personalizadas

### 3. Processamento de Pontuações

#### Workflow de Processamento de Fila
```
┌───────────┐       ┌───────────┐       ┌──────────┐       ┌──────────┐
│           │       │           │       │          │       │          │
│  Webhook  ├──────►│ Buscar    ├──────►│ Verificar├──────►│ Extrair  │
│  Trigger  │       │ Times     │       │ Items > 0│       │ Items    │
│           │       │ Pendentes │       │          │       │          │
└───────────┘       └───────────┘       └──────────┘       └──────────┘
                                                                │
                                                                ▼
┌───────────┐       ┌───────────┐       ┌──────────┐       ┌──────────┐
│           │       │           │       │          │       │          │
│ Próximo   │◄──────┤ Marcar    │◄──────┤ Salvar   │◄──────┤ Obter    │
│ Lote      │       │ Job como  │       │ Pontuação│       │ Dados do │
│ (recursão)│       │ Finalizado│       │ no BD    │       │ Cartola  │
└───────────┘       └───────────┘       └──────────┘       └──────────┘
```

**Detalhamento:**
1. **Webhook** (`/ligas/v1/job-queue-created/`): Inicia o processamento
2. **Consulta Jobs**: Busca times pendentes na view `time_rodada_aguardando`
3. **Verificação**: Confirma se existem itens para processar
4. **Extração**: Separa cada item para processamento individual
5. **Loop**: Processa cada item sequencialmente
6. **Obtenção de Dados**: Consulta a API do Cartola FC
7. **Persistência**: Salva pontuações no PocketBase
8. **Atualização de Status**: Marca jobs como finalizados
9. **Recursão**: Chama novamente o webhook para continuar o processamento

**Código Relevante:**
```javascript
// Consulta da API do Cartola FC
{
  "url": "=https://api.cartola.globo.com/time/id/{{ $('Loop Over Items').item.json.cartola_time_id }}/{{ $json.rodada }}"
}

// Salvamento de pontuações
{
  "method": "POST",
  "url": "https://api.ligas.ehtudo.app/api/collections/times_pontuacao/records",
  "bodyParameters": {
    "time": "={{$('Loop Over Items').item.json.time_id}}",
    "rodada": "={{$('Loop Over Items').item.json.rodada}}",
    "pontos": "={{$json.pontos == null ? 0: $json.pontos}}",
    "cartoletas": "={{ $json.patrimonio }}"
  }
}

// Atualização de status
{
  "method": "PATCH",
  "url": "=https://api.ligas.ehtudo.app/api/collections/job_queue/records/{{$('Loop Over Items').item.json.id}}",
  "bodyParameters": {
    "status": "\"finalizado\""
  }
}
```

## Coleções PocketBase

### Coleções do Sistema
- **_superusers**: Administradores do sistema
- **_authOrigins, _externalAuths, _mfas, _otps**: Coleções de suporte à autenticação

### Coleções de Domínio

#### Usuários
- **users**: Usuários do sistema com campos personalizados (name, avatar, role, max_leagues)

#### Ligas
- **ligas**: Ligas criadas pelos usuários
- **ligas_duplicate**: Versão estendida com relações (usado para testes)
- **ligas_times**: Tabela de junção entre ligas e times

#### Times
- **times**: Times dos usuários com IDs do Cartola FC
- **times_duplicate**: Versão estendida com relações (usado para testes)
- **times_pontuacao**: Pontuações dos times por rodada

#### Sistema de Processamento
- **job_queue**: Sistema de filas para processamento assíncrono
- **time_rodada_aguardando** (View): Consulta que mostra times pendentes

#### Configurações
- **settings**: Configurações globais do sistema
- **atletas**: Dados de atletas do Cartola FC

## Regras de Acesso no PocketBase

### Users
```javascript
listRule: "id = @request.auth.id"           // Usuário só vê seus dados
viewRule: "id = @request.auth.id"           // Usuário só vê seus dados
createRule: ""                             // Qualquer pessoa pode se registrar
updateRule: "id = @request.auth.id"         // Usuário só edita seus dados
authRule: "verified = true"                // Email verificado obrigatório
```

### Ligas
```javascript
listRule: ""                               // Visível publicamente
viewRule: ""                               // Visível publicamente
createRule: "@request.auth.id != \"\""      // Requer autenticação
updateRule: "user_id = @request.auth.id"    // Apenas criador pode atualizar
deleteRule: "user_id = @request.auth.id"    // Apenas criador pode excluir
```

## Desafios de Engenharia e Soluções

### 1. Processamento Assíncrono de Dados do Cartola FC

**Desafio:** Obter pontuações de múltiplos times sem sobrecarregar a API externa.

**Solução:** 
- Sistema de filas implementado no N8N
- Processamento sequencial com possível expansão para paralelo
- Mecanismo de recursão para processar lotes continuamente

### 2. Sincronização em Tempo Real

**Desafio:** Manter pontuações atualizadas durante rodadas em andamento.

**Solução:**
- Fila de processamento acionada por eventos
- Priorização de times em ligas ativas
- Status "aguardando" para jobs que precisam ser reprocessados

### 3. Integração entre N8N e PocketBase

**Desafio:** Manter consistência entre os sistemas.

**Solução:**
- N8N como camada de orquestração
- PocketBase como fonte única da verdade para dados
- Views SQL para consultas otimizadas

## Áreas de Melhoria

### Processamento de Filas
- **Implementar retry com backoff**: Para falhas temporárias na API do Cartola
- **Adicionar rate limiting controlado**: Para respeitar limites da API externa
- **Implementar processamento paralelo**: Com limite de concorrência

### Segurança
- **Substituir tokens hardcoded**: Usar variáveis de ambiente ou credentials no N8N
- **Implementar logging de segurança**: Para auditoria de operações sensíveis
- **Revisar permissões**: Garantir princípio do menor privilégio

### Monitoramento
- **Adicionar telemetria**: Para acompanhar performance e uso de recursos
- **Implementar alertas**: Para falhas recorrentes ou gargalos
- **Criar dashboard**: Para visualização do estado do sistema

## Roadmap Técnico

### Curto Prazo (1-2 meses)
1. Melhorias no processamento de filas (retry, rate limiting)
2. Implementação de logging e monitoramento
3. Substituição de credenciais hardcoded

### Médio Prazo (3-6 meses)
1. Cache para reduzir chamadas à API do Cartola FC
2. Processamento paralelo otimizado
3. Sistema de notificações para usuários

### Longo Prazo (6+ meses)
1. Avaliação de migração para banco de dados mais robusto
2. API pública documentada
3. Infraestrutura de alta disponibilidade
