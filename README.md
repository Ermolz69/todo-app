[![Build Validation](https://github.com/Ermolz69/todo-app/actions/workflows/build.yml/badge.svg)](https://github.com/Ermolz69/todo-app/actions/workflows/build.yml)

# Microsoft To-Do Clone

A simple full-stack task management application inspired by Microsoft To Do.

## Tech Stack

### Backend

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Swagger

### Frontend

* Angular 19
* TypeScript
* RxJS
* Tailwind CSS

### Infrastructure

* Docker Compose
* SQL Server Container

## Features

* User registration and login
* JWT authentication with refresh tokens
* Categories management
* Tasks CRUD operations
* Search and filtering
* Pagination
* Global exception handling
* Swagger API documentation

## Project Structure

```text
MicrosoftTo-Do
├── server
│   ├── Todo.Api
│   ├── Todo.Application
│   ├── Todo.Domain
│   ├── Todo.Infrastructure
│   └── Todo.sln
└── app
    └── web
        └── todo-client
```

## API Endpoints

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
```

### Categories

```http
GET    /api/categories
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

### Tasks

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

Example:

```http
GET /api/tasks?search=work&categoryId={id}&isCompleted=false&page=1&pageSize=10
```

## Running the Backend

```powershell
cd server

docker compose up -d

dotnet ef database update `
  --project Todo.Infrastructure `
  --startup-project Todo.Api

dotnet run --project Todo.Api
```

Swagger:

```text
http://localhost:5076/swagger
```

## Running the Frontend

```powershell
cd app\web\todo-client

npm install

npx ng serve
```

Frontend:

```text
http://localhost:4200
```

## Database

```text
Server=localhost,1433
Database=TodoDb
User Id=sa
Password=Your_password123
```

## Authentication Flow

```text
Login/Register
      ↓
Access Token + Refresh Token
      ↓
Authorization Header
      ↓
Protected API Endpoints
      ↓
Refresh Token When Needed
```

## Status

### Backend

* JWT authentication
* Refresh tokens
* Categories CRUD
* Tasks CRUD
* Search and filtering
* Pagination
* Swagger
* Global exception middleware

### Frontend

* Angular project setup
* Tailwind CSS setup
* API integration in progress
