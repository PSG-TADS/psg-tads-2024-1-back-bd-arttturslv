# Relatório do Trabalho Prático - Sistema de Locadora de Veículos

## Participantes

> - Artur Marcos 

## Introdução
### Contextualização do Projeto e sua Importância

O presente relatório tem como objetivo fornecer uma análise abrangente do projeto desenvolvido para incentivar os alunos a aprenderem conceitos de backend, utilizando tecnologias modernas e práticas. O projeto consiste na implementação de uma locadora de veículos, utilizando C#, ASP.NET Core, Entity Framework, SQL e Swagger para testes, visando familiarizar os alunos com essas ferramentas, e também proporcionar uma compreensão mais profunda por meio da aplicação em um contexto real.

### Objetivos do Sistema de Locadora de Veículos

Nesse contexto, o projeto é focado em desenvolver uma API utilizando ASP.NET Core para um sistema de locadora de veículos, onde clientes podem realizar reservas, alugar e devolver veículos.

### Tecnologias Utilizadas
- IDE: Visual Studio Community
- Repositório: Github
- Ambiente SQL: SQL Server Manager Studio

### Principais Funcionalidades
As principais funcionalidades do sistema relacionadas a clientes são:
- Cadastro e gestão de clientes.
- Pesquisa de todos ou de um cliente específico.

As principais funcionalidades do sistema relacionadas a veículos são:
- Cadastro e gestão de veículos.
- Pesquisa de todos ou de um veículo específico.
- Pesquisa de todos os veículos disponíveis.

As principais funcionalidades do sistema relacionadas a locações são:
- Cadastro e gestão de locações.
- Pesquisa de uma locação específica.
- Pesquisa de todas as locações ativas disponíveis.

## Requisitos Funcionais

| ID      | Descrição do Requisito                                     | Prioridade |
|---------|-------------------------------------------------------------|------------|
| RF-001  | O sistema deve permitir o cadastro de clientes              | ALTA       |
| RF-002  | O sistema deve permitir visualizar, atualizar e excluir informações de clientes existentes | ALTA |
| RF-003  | O sistema deve permitir a pesquisa de todos os clientes e clientes por CPF | BAIXA |
| RF-004  | O sistema deve permitir o cadastro de veículos              | ALTA       |
| RF-005  | O sistema deve permitir visualizar, atualizar e excluir informações de veículos existentes | ALTA |
| RF-006  | O sistema deve permitir a pesquisa de todos os veículos e veículos por placa | BAIXA |
| RF-007  | O sistema deve permitir buscar todos os veículos disponíveis | ALTA |
| RF-008  | O sistema deve permitir a criação de locações               | ALTA       |
| RF-009  | O sistema deve permitir visualizar e atualizar informações de locações existentes | ALTA |
| RF-010  | O sistema deve permitir a pesquisa de todas as locações e locações por ID | MÉDIA |
| RF-011  | O sistema deve permitir buscar todas as locações ativas      | BAIXA      |

## Arquitetura do Sistema

A arquitetura adotada para o desenvolvimento do sistema segue o padrão MVC (Model, View, Controller), uma abordagem que organiza a aplicação em três camadas distintas. A camada View é responsável pela apresentação dos dados aos usuários finais, e neste projeto, não há Views, essa responsabilidade foi assumida diretamente pelo Swagger, que fornece uma interface interativa para testar e documentar as APIs desenvolvidas. A camada Controller, por sua vez, é responsável por receber as requisições HTTP, realizar validações e tratamentos de erros, garantindo a integridade e segurança das operações realizadas. Finalmente, a camada Model descreve as entidades da aplicação, sendo responsável pela manipulação de dados.

## Implementação Técnica

### Principais Classes e Responsabilidades

- **Cliente**: Representa a entidade Cliente, responsável por armazenar informações como CPF, nome, telefone e email. Também possui uma coleção de locações associadas a esse cliente.
- **Locacao**: Representa a entidade de Locação, que armazena informações como o ID da locação, CPF do cliente, placa do veículo, datas de início e término da locação e status. Após a devolução, armazena a data de devolução, custo total do carro, e descrição.
- **Veiculo**: Representa a entidade Veículo, que armazena informações como placa, modelo, marca, ano, preço diário de locação, adicional por atraso diário e disponibilidade do veículo.

### Detalhamento das tecnologias utilizadas

- **C# 12.0 e ASP.NET Core (.NET 8.0)**: O ASP.NET Core oferece um ambiente de execução de alto desempenho para aplicativos Web, enquanto o C# 12.0 (ou superior) nos permite escrever código limpo e legível.
- **Entity Framework Core**: Optamos pelo Entity Framework Core para lidar com o mapeamento objeto-relacional (ORM), facilitando a interação com o banco de dados SQL Server. Os pacotes NuGet utilizados incluem: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools` e `Microsoft.VisualStudio.Web.CodeGeneration.Design`.
- **Swagger (Swashbuckle.AspNetCore)**: Integramos o Swagger ao nosso projeto para facilitar a documentação e teste de nossas APIs. O Swashbuckle.AspNetCore é uma biblioteca popular que gera automaticamente uma interface interativa com base nos endpoints da API, simplificando o processo de desenvolvimento e colaboração.
- **SQL Server Management**: Para gerenciar nosso banco de dados, utilizamos o SQL Server Management Studio. Essa ferramenta oferece uma interface intuitiva e robusta para administrar e manter o banco de dados SQL Server, permitindo-nos realizar operações de forma eficiente e segura.

## Descrição do Banco de Dados

### Modelo Conceitual
![Modelo Conceitual](/docs/concepcao/Modelo%20conceitual%20-%20Versão%20Final.PNG)

### Tabela Cliente:
| Campo           | Descrição                                                                           |
|-----------------|-------------------------------------------------------------------------------------|
| CPF (PK)        | Chave primária que identifica unicamente cada cliente.                                |
| Nome            | Nome do cliente.                                                                    |
| Telefone        | Número de telefone do cliente.                                                      |
| Email           | Endereço de e-mail do cliente.                                                      |
| Locacoes        | Relacionamento um para muitos com a tabela Locacao, representando as locações associadas a esse cliente. |

### Tabela Locacao:
| Campo           | Descrição                                                                           |
|-----------------|-------------------------------------------------------------------------------------|
| LocacaoID (PK)  | Chave primária que identifica unicamente cada locação.                               |
| CPF (FK)        | Chave estrangeira que faz referência ao CPF do cliente associado a esta locação.    |
| Placa (FK)      | Chave estrangeira que faz referência à placa do veículo associado a esta locação.    |
| DataInicio      | Data de início da locação.                                                          |
| DataFim         | Data de término da locação.                                                         |
| DataDevolucao   | Data em que o veículo foi devolvido.                                                |
| ValorTotal      | Valor total da locação.                                                             |
| Descricao       | Descrição adicional sobre a locação.                                                |

### Tabela Veiculo:
| Campo           | Descrição                                                                           |
|-----------------|-------------------------------------------------------------------------------------|
| Placa (PK)      | Chave primária que identifica unicamente cada veículo.                               |
| Modelo          | Modelo do veículo.                                                                  |
| Marca           | Marca do veículo.                                                                   |
| Ano             | Ano de fabricação do veículo.                                                       |
| PrecoDiario     | Preço diário de locação do veículo.                                                 |
| AdicionalAtraso | Valor adicional por atraso diário na devolução do veículo.                          |
| Disponivel      | Indica se o veículo está disponível para locação (true) ou não (false).             |

## Testes Realizados e Resultados Obtidos

As imagens do teste podem ser acessadas pelo link: [/imagens](/docs/concepcao/imagens/)

## Conclusão

O projeto foi concluído com sucesso, alcançando todos os objetivos propostos.
