# Sistema de Gestão Organizacional (OrgChart)

Sistema completo de gestão de estrutura organizacional com organograma hierárquico, desenvolvido com ASP.NET Core MVC, seguindo princípios de Clean Architecture e Domain-Driven Design (DDD).

## 🎯 Objetivo

Aplicação web para cadastrar e visualizar a estrutura organizacional de uma empresa (organograma), permitindo manutenção de departamentos, cargos e colaboradores, com persistência em banco SQL Server.

## 🚀 Funcionalidades Implementadas

### MVP (Requisitos Obrigatórios)

✅ **Cadastro de Departamentos**
- CRUD completo
- Campos: Id, Nome, Código (único), Status (Ativo/Inativo)
- Busca por nome
- Validação de duplicidade de código

✅ **Cadastro de Cargos**
- CRUD completo
- Campos: Id, Nome, Nível (Estagiário a Diretoria), Status (Ativo/Inativo)
- Enum para níveis hierárquicos
- Listagem ordenada por nível

✅ **Cadastro de Colaboradores**
- CRUD completo
- Campos: Id, Nome, Email (único), Departamento, Cargo, Data de Admissão, Gerente
- Validação de email único
- Seleção de gerente com dropdown
- **Validação robusta de ciclos na hierarquia** (não permite A gerenciar B, B gerenciar C, C gerenciar A)
- Validações de tela e servidor (FluentValidation)

✅ **Visualização do Organograma**
- Página dedicada exibindo a árvore hierárquica completa
- Visualização em cards indentados com cores por nível
- Exibe informações completas: nome, cargo, departamento, email
- Contador de subordinados
- Ordenação lógica por nível e nome
- Suporte a múltiplas raízes (colaboradores sem gerente)

✅ **Auditoria Básica**
- Timestamps de criação (CreatedAt)
- Timestamps de atualização (UpdatedAt)
- Exibição nas telas de detalhes

✅ **Soft Delete**
- Exclusão lógica com flag IsDeleted
- Timestamp de exclusão (DeletedAt)
- Query filters automáticos no EF Core
- Possibilidade de restauração

✅ **Endpoint API REST**
- `GET /api/orgchart` retornando JSON da árvore hierárquica
- Formato adequado para integração com bibliotecas de org chart
- Exemplo: `https://localhost:7123/api/orgchart` (use a porta exibida no console)

✅ **Validações Completas**
- FluentValidation na camada Application
- Validações de domínio nas entidades
- Mensagens de erro claras e amigáveis
- Validação de ciclos hierárquicos

✅ **Clean Code e Arquitetura**
- Separação em 4 camadas (Domain, Application, Infrastructure, Web)
- Responsabilidades bem definidas
- DTOs para todas as operações
- Repository Pattern + Unit of Work
- Dependency Injection
- Value Objects (Email)
- Serviços de domínio

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com **DDD**, dividido em 4 camadas:

```
OrgChart/
├── src/
│   ├── OrgChart.Domain/           # Núcleo do negócio
│   │   ├── Entities/              # Department, Position, Employee
│   │   ├── ValueObjects/          # Email
│   │   ├── Enums/                 # PositionLevel
│   │   ├── Interfaces/            # Contratos de repositórios
│   │   └── Services/              # HierarchyValidationService
│   │
│   ├── OrgChart.Application/      # Casos de uso
│   │   ├── DTOs/                  # Data Transfer Objects
│   │   ├── Services/              # Serviços de aplicação
│   │   ├── Validators/            # FluentValidation
│   │   └── Common/                # Result pattern
│   │
│   ├── OrgChart.Infrastructure/   # Implementações técnicas
│   │   ├── Data/                  # DbContext, Migrations, Seed
│   │   └── Repositories/          # Implementação dos repositórios
│   │
│   └── OrgChart.Web/              # Interface do usuário
│       ├── Controllers/           # MVC Controllers
│       ├── Views/                 # Razor Views (Bootstrap 5)
│       └── wwwroot/               # Assets estáticos
```

### Princípios Aplicados

- **SOLID**: Single Responsibility, Open/Closed, Dependency Inversion
- **DDD**: Entidades ricas, Value Objects, Serviços de Domínio
- **Clean Architecture**: Dependências apontando para o core
- **Repository Pattern**: Abstração de acesso a dados
- **Unit of Work**: Gerenciamento de transações

## 🛠️ Tecnologias Utilizadas

- **Framework**: ASP.NET Core 10.0 (MVC)
- **Banco de Dados**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 10.0
- **Validação**: FluentValidation 12.1
- **Frontend**: Bootstrap 5.3 + Bootstrap Icons
- **Linguagem**: C# 12

## 📋 Pré-requisitos

- .NET SDK 10.0 ou superior
- SQL Server LocalDB (instalado com Visual Studio) ou SQL Server
- Visual Studio 2022 / VS Code / Rider (opcional)
- Navegador web moderno

## 🚀 Como Executar

### 1. Clone o repositório

```bash
git clone https://github.com/gabrielms96/OrgChart.git
cd OrgChart
```

### 2. Configure a Connection String (Opcional)

A aplicação está configurada para usar SQL Server LocalDB por padrão. Se precisar alterar:

Edite `src/OrgChart.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrgChartDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**Alternativa para SQL Server completo:**
```
"DefaultConnection": "Server=localhost;Database=OrgChartDb;User Id=seu_usuario;Password=sua_senha;TrustServerCertificate=true;"
```

### 3. Restaure as dependências e compile

```bash
dotnet restore
dotnet build
```

### 4. Execute a aplicação

```bash
dotnet run --project src/OrgChart.Web/OrgChart.Web.csproj
```

**Ou usando o executável diretamente:**
```bash
cd src/OrgChart.Web
dotnet run
```

### 5. Acesse no navegador

Após executar o comando acima, observe a saída do console. A aplicação exibirá as URLs disponíveis como por exemplo:

```
Now listening on: http://localhost:5028
Now listening on: https://localhost:7123
```

### 🗄️ Banco de Dados

O banco de dados é **criado e populado automaticamente** na primeira execução!

- **Migrations são aplicadas automaticamente** ao iniciar
- **Dados de exemplo (seed)** são inseridos automaticamente
- Inclui: 5 departamentos, 12 cargos, 13 colaboradores com hierarquia exemplo

#### Dados de exemplo incluem:
- CEO sem gerente
- Diretores reportando ao CEO
- Gerentes reportando aos diretores
- Coordenadores e desenvolvedores em hierarquia
- Diferentes níveis (Estagiário, Júnior, Pleno, Sênior, etc.)

Se precisar recriar o banco manualmente:

```bash
# Remover migrations e banco
dotnet ef database drop --project src/OrgChart.Infrastructure --startup-project src/OrgChart.Web

# Recriar migrations
dotnet ef migrations add InitialCreate --project src/OrgChart.Infrastructure --startup-project src/OrgChart.Web

# Aplicar migrations
dotnet ef database update --project src/OrgChart.Infrastructure --startup-project src/OrgChart.Web
```

## 📚 Estrutura do Banco de Dados

### Tabelas

**Departments**
- Id (PK)
- Name (required, max 200)
- Code (unique, nullable, max 50)
- IsActive (bool)
- CreatedAt, UpdatedAt, IsDeleted, DeletedAt

**Positions**
- Id (PK)
- Name (required, max 200)
- Level (int enum: 0-6)
- IsActive (bool)
- CreatedAt, UpdatedAt, IsDeleted, DeletedAt

**Employees**
- Id (PK)
- Name (required, max 200)
- Email (required, unique, max 255)
- DepartmentId (FK)
- PositionId (FK)
- ManagerId (FK self-reference, nullable)
- HireDate (DateTime)
- CreatedAt, UpdatedAt, IsDeleted, DeletedAt

### Relacionamentos

- Employee → Department (N:1)
- Employee → Position (N:1)
- Employee → Employee (Manager/Subordinates, 1:N)

Todas as FKs com `DeleteBehavior.Restrict` para proteção de integridade.

## 🎨 Interface do Usuário

- Design moderno com **Bootstrap 5**
- **Bootstrap Icons** para melhor UX
- Layout responsivo (mobile-friendly)
- Menu de navegação intuitivo
- Alertas de sucesso/erro (TempData)
- Modais de confirmação para exclusões
- Formulários com validação client-side
- Organograma visual com cores por nível hierárquico

## 🔒 Validações Implementadas

### Domínio (Entidades)
- Nomes obrigatórios e com limite de caracteres
- Email em formato válido (Value Object)
- Datas de admissão não futuras
- Colaborador não pode ser gerente de si mesmo

### Aplicação (Services)
- Email único no sistema
- Código de departamento único
- Gerente deve existir
- Departamento e cargo devem existir
- **Validação de ciclos na hierarquia** (BFS algorithm)

### Apresentação (Views)
- FluentValidation integrado
- Validação HTML5
- Scripts de validação client-side
- Mensagens de erro contextuais

## 📈 Decisões Técnicas

### Por que Clean Architecture?

Escolhi Clean Architecture para:
- **Testabilidade**: Lógica de negócio isolada e testável
- **Manutenibilidade**: Fácil localizar e modificar código
- **Escalabilidade**: Adicionar features sem impactar o existente
- **Independência**: Core não depende de framework, BD ou UI

### Por que DDD?

Domain-Driven Design foi escolhido para:
- **Modelagem rica**: Entidades com comportamento, não apenas dados
- **Validações centralizadas**: Regras no domínio
- **Value Objects**: Email como tipo próprio com validação
- **Serviços de domínio**: Validação de hierarquia como serviço especializado

### Por que EF Core?

- **Produtividade**: Migrations automáticas, LINQ
- **Convenções**: Configuração por convenção + Fluent API
- **Query Filters**: Soft delete transparente
- **Relacionamentos**: Fácil trabalhar com hierarquias

### Validação de Ciclos

Implementei BFS (Breadth-First Search) para detectar ciclos:
1. Ao atribuir gerente, verifica se o novo gerente é subordinado (direto ou indireto)
2. Percorre toda a subárvore do colaborador
3. Se encontra o novo gerente na subárvore, rejeita a operação

### Result Pattern

Uso de `Result<T>` para encapsular sucesso/falha:
- Evita exceptions para fluxo de negócio
- Retorno consistente com mensagens de erro
- Facilita tratamento na UI

## 🧪 Testes

**Status**: Implementação básica funcional pronta. Testes unitários seriam o próximo passo.

**Áreas prioritárias para testes:**
- HierarchyValidationService (validação de ciclos)
- Validações de entidades
- Serviços de aplicação
- Repositórios (integration tests)

## 🔄 Melhorias Futuras

### Backlog de Features
- [ ] Paginação nas listagens
- [ ] Filtros avançados (por departamento, cargo, etc.)
- [ ] Exportação para PDF/Excel
- [ ] Importação de colaboradores via CSV
- [ ] Histórico de mudanças (audit log completo)
- [ ] Dashboard com métricas
- [ ] Busca global
- [ ] Fotos de perfil dos colaboradores
- [ ] Integração com Active Directory/LDAP

### Melhorias Técnicas
- [ ] Testes unitários (xUnit)
- [ ] Testes de integração
- [ ] Padronização de idioma (todo código em inglês)
- [ ] Docker Compose (SQL + App)
- [ ] CI/CD (GitHub Actions)
- [ ] Logging estruturado (Serilog)
- [ ] Health checks
- [ ] API completa RESTful
- [ ] Autenticação e autorização
- [ ] Rate limiting
- [ ] Caching (Redis)

### UX/UI
- [ ] Organograma com biblioteca visual (vis.js, d3.js)
- [ ] Drag & drop no organograma
- [ ] Zoom e pan no organograma
- [ ] Busca em tempo real
- [ ] Dark mode
- [ ] Animações e transições

## 📝 O que ficou faltando / Trade-offs

### Conscientes
- **Testes automatizados**: Priorizei entrega funcional completa
- **Autenticação**: Fora do escopo do MVP
- **Paginação**: Listagens simples suficientes para MVP
- **Docker**: Configurável mas não obrigatório

### Limitações conhecidas
- Organograma funcional mas simples (não é interativo)
- Sem busca avançada (apenas busca básica em departamentos)
- Sem relatórios ou exportações
- Sem versionamento de organograma
- **Inconsistência de idioma**: Mistura de português (nomes de variáveis, comentários) com inglês (nomenclaturas técnicas).


## 👨‍💻 Desenvolvimento

**Tempo estimado**: ~4-6 horas de desenvolvimento focado

**Commits organizados** por feature/camada:
- Setup inicial da solução
- Domain layer completo
- Application layer com serviços
- Infrastructure com EF Core
- Web layer com Bootstrap 5
- Organograma e refinamentos

## 📄 Licença

Este projeto foi desenvolvido como teste técnico e é de uso livre para avaliação.

---
