# FAT Management API

Responsável por manter UserProfile, Table. Se comunicando com a base de dados MongoDb.

## Funcionalidades

As principais funcionalidades da API.

- Manter usuário
- Manter mesa
- Autenticação de usuário
- Entrar e sair de uma mesa

## Instalação e execução

Para executar o projeto é necessário atender alguns requisitos mínimos:

- .Net 7
- MongoDb
- Projeto clonado

```bash
$ git clone https://github.com/saulogp/fat.management.api.git
$ cd  fat.management.api/Presentation
$ dotnet restore
$ dotnet run
```

## Usage

Explain how to use your API. Provide examples of requests and responses, and showcase the different available endpoints.

### Endpoint 1

Describe the purpose of the endpoint and how it should be used.

```
GET /endpoint1
```

**Query Parameters:**

- `parameter1`: Description of parameter 1.
- `parameter2`: Description of parameter 2.

**Successful Response:**

```json
{
  "result": "value"
}
```

**Status Codes:**

- `200`: OK. Successful response.
- `400`: Bad Request. Incorrect parameters.
- `500`: Internal Server Error. Server internal error.

### Endpoint 2

Describe the purpose of the endpoint and how it should be used.

```
POST /endpoint2
```

**Request Body:**

```json
{
  "property1": "value1",
  "property2": "value2"
}
```

**Successful Response:**

```json
{
  "result": "value"
}
```

**Status Codes:**

- `201`: Created. Entity has been successfully created.
- `400`: Bad Request. Incorrect parameters.
- `500`: Internal Server Error. Server internal error.

## Contribution

Explain how other developers can contribute to your project. Include instructions on how to clone the repository, set up the development environment, and submit pull requests.

## License

Specify the license under which your API is published.

## Contact

Provide contact information so that users can reach out to you for questions or issues.

## Acknowledgments

If you want to thank specific individuals or projects, you can include this section at the end of the README.