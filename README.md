
# 🎥 Controle de Cinema 🎥

## 📖 Introdução  
O **Controle de Cinema** é um sistema desenvolvido para gerenciar todas as operações de um cinema: desde o cadastro de gêneros, filmes e salas, até a criação de sessões, controle de ingressos e autenticação de usuários.

> 🎬 *"Se pode exibir, pode testar. Se pode vender ingresso, deve validar o comportamento."*

Este projeto foi entregue com a aplicação já estruturada, mas a atividade proposta tem como **principal objetivo** a implementação de **testes automatizados**, garantindo qualidade, confiabilidade e aderência aos requisitos descritos no documento **Casos de Teste - Controle de Cinema.pdf**.

---

[![wakatime](https://wakatime.com/badge/user/d66fe803-196c-4729-b330-f8a026db44ec/project/87dc4507-84ce-4534-ace5-54506ce6386f.svg)](https://wakatime.com/badge/user/d66fe803-196c-4729-b330-f8a026db44ec/project/87dc4507-84ce-4534-ace5-54506ce6386f)

## 🧩 Módulos do Sistema

Mesmo que o foco seja os testes, entender os módulos é essencial, pois cada um contém regras de negócio cobertas nos cenários de validação.

### 🎭 Gênero  
- Cadastro, edição, exclusão e listagem de gêneros.
- Validação de duplicidade e obrigatoriedade de campos.

### 🏟️ Sala  
- Cadastro de salas com número e capacidade.  
- Validação de capacidade positiva e número único.

### 🎬 Filme  
- Cadastro, edição, exclusão e listagem de filmes vinculados a gêneros.
- Validação de duração positiva, título único e obrigatoriedade de campos.

### 📅 Sessão  
- Cadastro de sessões associadas a um filme e uma sala.
- Controle de conflito de horários para a mesma sala.

### 🎟️ Ingressos  
- Compra unitária de ingressos por assento.
- Bloqueio de compra para sessões lotadas.

⚠️ **Observações Importantes**  
- O PDF menciona **preço da sessão** → **não implementado** → **CT028** e parte do **CT021** não se aplicam.  
- O PDF prevê **compra por quantidade** → **não implementada** → validação equivalente ocorre via **CT031**.

---

## 🧪 Testes Automatizados

Os testes foram desenvolvidos com foco em **cobertura, clareza e manutenibilidade**, baseados no PDF de casos de teste.

### ✅ Testes Unitários
- Cobertura acima de **75% do código**.  
- Uso de **mocks** para isolar dependências externas.  
- Validação de regras de negócio, como duplicidades e campos obrigatórios.

### 🔗 Testes de Integração
- Operações **CRUD** para gêneros, filmes, salas e sessões.  
- Conexão com **banco de dados em container** via **Testcontainers**.  
- Validação de persistência e integridade dos dados.

### 🖥️ Testes de Interface (Selenium)
- Cobrem fluxos principais **de ponta a ponta**.  
- Seletores robustos com atributos `data-se` para garantir estabilidade.  
- Uso de **esperas explícitas** para reduzir falhas intermitentes.  

**Exemplos de Fluxos Testados:**
- Cadastro, edição e exclusão de entidades.  
- Login e logout.  
- Compra de ingresso e bloqueio em sessões lotadas.

### ⚠️ Casos de Teste Fora de Escopo
| Caso | Motivo |
|------|-----------------------------------------------|
| **CT028** | Campo `Preço` não existe em `Sessão`. |
| **CT032** | Compra por quantidade não implementada. |

📄 Para detalhes completos, consulte **[Casos de Teste - Controle de Cinema.pdf](https://github.com/user-attachments/files/22068934/Casos.de.Teste.-.Controle.de.Cinema.pdf)**.

---

## 🚀 Como Executar a Aplicação  

> **Importante:** Para rodar a aplicação localmente, **você precisa ter um banco de dados PostgreSQL disponível**, seja **instalado localmente** ou via **Docker**.

### **1. Pré-requisitos**
- **.NET SDK 8.0** ou superior  
- **PostgreSQL 16+** (local ou Docker)  
- **Visual Studio 2022** ou superior  
- **Docker** *(opcional, mas recomendado)*  
- **Git** instalado para clonar o repositório  

---

### **2. Configuração do Banco de Dados**

#### **Opção A — Via Docker (recomendado)**
```bash
docker run --name postgres-controle-cinema -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ControleDeCinema -p 5432:5432 -d postgres:16
```

#### **Opção B — PostgreSQL local**
1. Instale o [PostgreSQL](https://www.postgresql.org/download/).
2. Crie o banco de dados:
   ```sql
   CREATE DATABASE "ControleDeCinema";
   ```

---

### **3. Clone o Repositório**
```bash
git clone https://github.com/Compila-logo-existe/ControleDeCinema
```

### **4. Acesse a Pasta Principal**
```bash
cd ControleDeCinema
cd ControleDeCinema.WebApp
```

### **5. Configure a Connection String**

A aplicação utiliza a variável **`SQL_CONNECTION_STRING`** para se conectar ao banco de dados.  
Durante o desenvolvimento, o projeto utiliza o recurso de **User Secrets** do .NET, garantindo que dados sensíveis, como usuário e senha do banco, **não fiquem expostos** no repositório.

---

#### **Configuração via User Secrets** ✅ *(recomendado)*

1. **Inicialize o User Secrets** *(caso ainda não tenha feito)*  
   No terminal, dentro da pasta do projeto **ControleDeCinema.WebApp**:

   ```bash
   dotnet user-secrets init
   ```

   Isso criará um identificador único `<UserSecretsId>` no arquivo `.csproj` do projeto WebApp.

---

2. **Defina a variável `SQL_CONNECTION_STRING`**  
   Ainda no terminal, execute:

   ```bash
   dotnet user-secrets set "SQL_CONNECTION_STRING" "Host=localhost;Port=5432;Database=ControleDeCinema;Username=postgres;Password=postgres"
   ```

---

3. **Confirme se a variável foi configurada corretamente**  
   ```bash
   dotnet user-secrets list
   ```

   Saída esperada:
   ```
   SQL_CONNECTION_STRING = Host=localhost;Port=5432;Database=ControleDeCinema;Username=postgres;Password=postgres
   ```

---

Com isso, o método `AddEntityFrameworkConfig` localizará automaticamente a connection string ao rodar o projeto, sem necessidade de `.env` ou `appsettings.json`.

### **6. Restaure as Dependências**
```bash
dotnet restore
```

### **7. Compile a Aplicação**
```bash
dotnet build --configuration Release
```

### **8. Execute o Projeto**

#### **Ambiente de Desenvolvimento** *(https:7131 / http:5217)*
```bash
dotnet run --project ControleDeCinema.WebApp.csproj --launch-profile "https [Dev]"
```

#### **Ambiente de Produção Local** *(https:8081 / http:8080)*
>> Para rodar localmente, precisa configurar user-secrets para _NEWRELIC_LICENSE_KEY_.
```bash
dotnet run --project ControleDeCinema.WebApp.csproj --launch-profile "https [Prod]"
```

---

### **9. Acesse a Aplicação**
- **Desenvolvimento** → [https://localhost:7131](https://localhost:7131) ou [http://localhost:5217](http://localhost:5217)
- **Produção local** → [https://localhost:8081](https://localhost:8081) ou [http://localhost:8080](http://localhost:8080)

---

## 🧪 Como Executar os Testes

### **1. Acesse a pasta do projeto de testes**
> Ou rode o comando do passo 2. direto na pasta principal para executar **todos** os testes.
> Caso esteja no .WebApp, use ```cd ../``` para voltar à pasta principal.
```bash
cd ControleDeCinema.Testes.Unidades
cd ControleDeCinema.Testes.Integracao
cd ControleDeCinema.Testes.Interface
```

### **2. Execute os testes**
```bash
dotnet test
```

---

## 🛠️ Tecnologias Utilizadas

![Tecnologias](https://skillicons.dev/icons?i=github,visualstudio,vscode,cs,dotnet,html,css,bootstrap,selenium)

* **C# / .NET 8.0** – backend e testes
* **ASP.NET MVC** – camada web
* **Entity Framework Core** – persistência de dados
* **Selenium** – testes de interface
* **MSTest** – testes unitários e de integração
* **Testcontainers** – isolamento de ambiente para testes de integração
* **FluentResults** – padronização de respostas da aplicação
* **GitHub Actions** – pipeline de CI/CD

---

## 📌 Requisitos

* **.NET SDK 8.0** ou superior
* **PostgreSQL 16+** (local ou via Docker)
* **Visual Studio 2022** (ou superior, com suporte a ASP.NET MVC)
* **Navegador moderno** (Edge, Chrome, Firefox etc.)

---

## 🧠 Filosofia do Projeto

> "Cada sessão merece ser exibida.  
> Cada ingresso precisa ser validado.  
> Cada teste deve existir para garantir a experiência do usuário."

— *Compila, Logo Existe*
