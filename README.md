# TaskFlow

REST API менеджера задач: проекты, задачи, статусы и приоритеты.

**Стек:** C# · ASP.NET Core 8 Web API · Entity Framework Core · PostgreSQL · Docker · Swagger

## Что умеет

- CRUD для проектов, у каждого уникальный ключ (`WEB`, `API`)
- Задачи со статусами `New` → `InProgress` → `Done` и приоритетами `Low` / `Medium` / `High`
- Фильтрация задач по статусу и проекту, сортировка по приоритету
- Подсчёт открытых задач по каждому проекту
- Асинхронная работа с БД (async/await), DTO и валидация входных данных

## Запуск

Нужны [.NET 8 SDK](https://dotnet.microsoft.com/download) и [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
docker compose up -d                 # поднимает PostgreSQL на порту 5432
cd src/TaskFlow.Api
dotnet run                           # API на http://localhost:5080
```

Swagger: http://localhost:5080/swagger. Примеры запросов лежат в `src/TaskFlow.Api/requests.http`.

## Эндпоинты

| Метод | URL | Что делает |
| --- | --- | --- |
| GET | /api/projects?search= | Список проектов |
| GET | /api/projects/{id} | Проект по id |
| POST | /api/projects | Создать проект |
| PUT | /api/projects/{id} | Изменить проект |
| DELETE | /api/projects/{id} | Удалить проект |
| GET | /api/tasks?status=&projectId= | Список задач |
| GET | /api/tasks/{id} | Задача по id |
| POST | /api/tasks | Создать задачу |
| PATCH | /api/tasks/{id}/status | Сменить статус |
| DELETE | /api/tasks/{id} | Удалить задачу |

## Структура

```
src/TaskFlow.Api/
  Models/        сущности БД (Project, TaskItem)
  Data/          AppDbContext (EF Core)
  Dtos/          модели запросов и ответов API
  Controllers/   эндпоинты
```

## Планы

- [ ] Миграции EF Core вместо EnsureCreated
- [ ] Пагинация
- [ ] Исполнители задач
- [ ] Unit-тесты (xUnit)
- [ ] Dockerfile для API
- [ ] Десктоп-клиент на WPF (MVVM)
