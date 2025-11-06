# Taskify

Приложение для управления задачами с фронтендом на React и бекендом на .NET.

## Технологии

### Backend
- .NET 9
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- MediatR
- FluentValidation
- CQRS
- Clean Architecture
- Serilog


### Frontend
- React
- TypeScript
- Vite
- Axios
- MobX
- FSD

## Требования

- Docker
- Docker Compose

## Запуск проекта

Склонируйте репозиторий и запустите проект:

```bash
docker-compose up --build
```

Приложение будет доступно по адресам:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432

## Остановка проекта

```bash
docker-compose down
```

Для удаления данных базы данных:

```bash
docker-compose down -v
```
