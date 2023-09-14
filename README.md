
# WebAPI Filme

WebAPI simples escrita em C# com uso do framework .NET 6.0

Api para estudos onde relciona filmes, cinemas e sessões.

Endpoints de Filmes estão documentados em: https://localhost:8001/swagger/index.html

Necessário: SQL server
Configuração do BD na aplicação:

Em appsettings.json:

		"ConnectionStrings": {
								"FilmeConnection": "server=localhost;database=BDName;user=username;password=password;"
							  }

