## 🌐 WebAPI em .NET 6 com Dapper e MimeKit

O projeto foi desenvolvido como uma WebAPI em .NET 6, utilizando tecnologias como:
  🛠️ Dapper
  🛠️ AutoMapper
  🛠️ Serilog
  🛠️ MailKit e MimeKit
  🛠️ Repository Pattern
  🛠️ Arquitetura de camadas

A finalidade do projeto era a curiosidade de como é desenvolvido um software para envio de e-mails e aproveitar também para solidificar e compreender conceitos de mapeamento de entidades com Dtos e AutoMapper e uso de persistência em banco com Dapper, e uso do Serilog para efetuar logs persistidos diretamente em banco.

A API é acessível via Swagger e o projeto conta com apenas dois endpoints, sendo um para criar e-mails e um para resgatar um e-mail específico do banco através do seu Id.

### 📀 Estrutura do Projeto

O projeto é organizado em uma solução com três projetos, sendo um projeto de WebAPI e dois projetos de bibliotecas de classes. Sendo eles:

  WebMail.API: Contém os controladores, Dtos e serviços responsáveis por gerenciar as requisições, criar e-mails e consumir as camadas de domínio e infraestrutura. Esta camada também possui um serviço de background responsável por consultar os e-mail que devem ser enviados periodicamente e então enviá-los.

  WebMail.Domain: Contém a classe de domínio que representa a entidade de e-mail.

  WebMail.Infrastructure: Camada de acesso ao banco através de dapper com SQL.

### 💡 Funcionamento

Através do controlador, é possível criar um e-mail, informando uma origem (remetente), um destino (destinatário), um assunto e um corpo do e-mail. Ao efetuar a requisição, o objeto de requisição é enviado para o serviço de criação de e-mail que irá mapear o Dto de requisição para um objeto de domínio e em seguida irá persistir em um banco de dados SqlServer adicionando a data de geração. Após a inserção no banco será retornado um Dto de resposta com as mesmas informações do e-mail agora acrescidas do Id e data de geração, de forma que o usuário pode usar o Id para consultar o e-mail e se já existe data de envio do mesmo.

Em um serviço de background que roda periodicamente, conforme configuração no appsettings.json, é feita uma consulta de todos os e-mails pendentes de envio. Se houverem e-mail pendentes, o serviço usando ParellelForeach, irá paginar os e-mails para poder efetuar o envio em threads paralelas de forma a acelerar o processo. Em cada thread, usando a biblioteca do MailKit e MimeKit, ele cria os objetos que representam os e-mails e instância clientes Smtp e os autentica com o provedor de envio de e-mail e enfim efetua o envio e atualiza no banco com a data de envio.

As configurações de connection string e provedor de e-mail são todas feitas no appsettings.json.

### ⏏️ Oportunidades de Melhoria

Entre as oportunidades de melhoria do sistema, temos:
  ✔️ Remoção do código Sql da camada de infraestrutura, deixando toda a abstração por conta do Dapper ou substituindo o Dapper pelo Entity Framework;
  ✔️ Reorganizar as camadas, levando os Dtos e interfaces para a camada de domínio;
  ✔️ Criação de uma camada de aplicação para separar os serviços da camada de API;
  ✔️ Adição de validação na criação de e-mail de forma a conferir se os endereços de e-mails são válidos;
  ✔️ Modificar o funcionamento periódico do serviço de envio de e-mail de forma a prevenir o pooling no banco de dados;
  ✔️ Incluir o padrão Result e remover o uso de exceptions no envio de e-mail para que isso não bloqueie o fluxo e ele possa logar os resultados de cada envio mal sucedido e marcar no banco os e-mails que não puderam ser enviados;
  ✔️ Seguindo a ideia anterior, disponibilizar uma rota para consulta de e-mails com envio mal sucedido, filtrado por tempo e com paginação, dando possibilidade para que o usuário possa tentar reenviar futuramente, efetuando as devidas correções para a conclusão do envio.    
