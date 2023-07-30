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

## **Authentication Endpoints**

### **Login**

Endpoint: `POST /api/v1/auth/login`

This endpoint allows users to log in by providing their email and password.

Request Body:

```json
{
  "Email": "user@example.com",
  "Password": "password123"
}
```

Responses:

- 200 OK: Successfully authenticated. The response contains the user's authentication token.
- 204 No Content: The user with the provided email was not found.
- 400 Bad Request: Invalid email or password.
- 401 Unauthorized: Incorrect password.

### **Register**

Endpoint: `POST /api/v1/auth/register`

This endpoint allows users to register by providing their email, username, and password.

Request Body:

```json
{
  "Email": "user@example.com",
  "Username": "newuser",
  "Password": "password123"
}
```

Responses:

- 200 OK: Successfully registered.
- 400 Bad Request: Invalid email, username, or password.
- 400 Bad Request: This email is already being used.
- 400 Bad Request: This username is already being used.

### **Change Password**

Endpoint: `POST /api/v1/auth/changePassword`

This endpoint allows users to change their password by providing their email, current password, and new password.

Request Body:

```json
{
  "Email": "user@example.com",
  "Password": "oldpassword",
  "NewPassword": "newpassword123"
}
```

Responses:

- 200 OK: Successfully changed the password.
- 400 Bad Request: Invalid email, current password, or new password.
- 400 Bad Request: User not found.
- 401 Unauthorized: Incorrect password.

## **User Profile Endpoints**

### **Get User Profile**

Endpoint: `GET /api/v1/user/{id}`

This endpoint allows users with appropriate authentication to retrieve a user profile based on the user's ID.

Parameters:
- `id` (Guid): The ID of the user profile to retrieve.

Responses:

- 200 OK: Successfully retrieved the user profile. The response contains the user profile information.
- 400 Bad Request: Invalid user ID.
- 400 Bad Request: User not found.

### **Update User Profile**

Endpoint: `PUT /api/v1/user/{id}`

This endpoint allows users with appropriate authentication to update a user profile based on the user's ID.

Parameters:
- `id` (Guid): The ID of the user profile to update.

Request Body:

```json
{
  "Email": "updateduser@example.com",
  "UserName": "updateduser",
  "ImgUrl": "https://example.com/profile-image.jpg"
}
```

Responses:

- 200 OK: Successfully updated the user profile. The response contains the ID of the updated user profile.
- 400 Bad Request: Invalid user ID.
- 400 Bad Request: Invalid user data (e.g., missing fields in the request body).
- 400 Bad Request: User not found.

### **Delete User Profile**

Endpoint: `DELETE /api/v1/user/{id}`

This endpoint allows users with appropriate authentication to delete a user profile based on the user's ID.

Parameters:
- `id` (Guid): The ID of the user profile to delete.

Responses:

- 200 OK: Successfully deleted the user profile. The response contains the ID of the deleted user profile.
- 400 Bad Request: Invalid user ID.
- 400 Bad Request: User not found.

## **Table Endpoints**

### **Create Table**

Endpoint: `POST /api/v1/table/`

This endpoint allows authenticated users to create a new table.

Request Body:

```json
{
  "Owner": "user@example.com",
  "Title": "Table Title",
  "ImgUrl": "https://example.com/table-image.jpg",
  "Description": "Table description",
  "MaxPlayers": 4,
  "Platform": "Platform",
  "SystemGame": "System Game",
  "Genres": ["Adventure", "ScienceFiction"]
}
```

Responses:

- 200 OK: Successfully created the table.
- 400 Bad Request: Invalid request. Table data is missing.

### **Get Table Categories**

Endpoint: `GET /api/v1/table`

This endpoint allows authenticated users to retrieve tables with optional filters (e.g., genres, date).

Query Parameters:
- `filters` (string[]): Array of filters to apply to the table list.

Responses:

- 200 OK: Successfully retrieved the list of tables with optional filters.
- 501 Not Implemented: The endpoint is not implemented yet.

### **Get Table by ID**

Endpoint: `GET /api/v1/table/{id}`

This endpoint allows authenticated users to retrieve a table based on the table's ID.

Parameters:
- `id` (Guid): The ID of the table to retrieve.

Responses:

- 200 OK: Successfully retrieved the table.
- 400 Bad Request: Table not found.

### **Update Table**

Endpoint: `PUT /api/v1/table/{id}`

This endpoint allows authenticated users to update a table based on the table's ID.

Parameters:
- `id` (Guid): The ID of the table to update.

Request Body:

```json
{
  "Title": "Updated Table Title",
  "ImgUrl": "https://example.com/updated-table-image.jpg",
  "Description": "Updated table description",
  "MaxPlayers": 6,
  "Platform": "Updated Platform",
  "SystemGame": "Updated System Game",
  "Genres": ["Adventure", "ScienceFiction", "Mystery"]
}
```

Responses:

- 200 OK: Successfully updated the table.
- 400 Bad Request: Table not found.
- 400 Bad Request: Table is not active.
- 400 Bad Request: Request cannot be null.

### **Delete Table**

Endpoint: `DELETE /api/v1/table/{id}`

This endpoint allows authenticated users to delete a table based on the table's ID.

Parameters:
- `id` (Guid): The ID of the table to delete.

Responses:

- 200 OK: Successfully deleted the table.
- 400 Bad Request: Table not found.
- 400 Bad Request: Table is not active.

### **Get Tables by User ID**

Endpoint: `GET /api/v1/table/user/{id}`

This endpoint allows authenticated users to retrieve tables in which a user is participating based on the user's ID.

Parameters:
- `id` (Guid): The ID of the user to retrieve the tables for.

Responses:

- 200 OK: Successfully retrieved the list of tables for the specified user ID.
- 400 Bad Request: No tables found for the specified user ID.

### **Join Table**

Endpoint: `POST /api/v1/table/{tableId}/user/{userId}/join`

This endpoint allows authenticated users to join a table based on the table's ID and the user's ID.

Parameters:
- `tableId` (Guid): The ID of the table to join.
- `userId` (Guid): The ID of the user joining the table.

Responses:

- 200 OK: Successfully joined the table.
- 400 Bad Request: Invalid table or user ID.
- 400 Bad Request: Table not found.
- 400 Bad Request: Table is not active.
- 400 Bad Request: This user owns or already participates.
- 400 Bad Request: Table is full.

### **Leave Table**

Endpoint: `POST /api/v1/table/{tableId}/user/{userId}/leave`

This endpoint allows authenticated users to leave a table based on the table's ID and the user's ID.

Parameters:
- `tableId` (Guid): The ID of the table to leave.
- `userId` (Guid): The ID of the user leaving the table.

Responses:

- 200 OK: Successfully left the table.
- 400 Bad Request: Invalid table or user ID.
- 400 Bad Request: Table not found.
- 400 Bad Request: Table is not active.
- 400 Bad Request: User is not participating in the table.

## Contribution

Explain how other developers can contribute to your project. Include instructions on how to clone the repository, set up the development environment, and submit pull requests.

## License

Specify the license under which your API is published.

## Contact

Provide contact information so that users can reach out to you for questions or issues.

## Acknowledgments

If you want to thank specific individuals or projects, you can include this section at the end of the README.
