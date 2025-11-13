# Система менеджмента проектов

## Предварительные требования
- .NET 10
- Node.js 11.6.0 (пакетный менеджер npm)
- Next.js 16.0.3

## Сборка и запуск

### 1. Бэкенд (ASP.NET Core)
```bash
dotnet ef database update -s ProjectsManager.Api -p ProjectsManager.DataAccess

dotnet run
```

### Тесты
```
dotnet test
```

### 2. Фронтэнд (Next.js)
```bash
# ProjectsManager.NextjsClient/projects-manager

npm install
npm run dev # npm run build
```

## Используемые сторонние библиотеки
### Фронтенд
- antd - UI-библиотека с готовыми React-компонентами
- axios - HTTP-клиент для взаимодействия с API
- i18next - библиотека для интернационализации

### Бэкенд
- Moq - библиотека для мокинга зависимостей в тестах
- CSharpFunctionalExtensions - реализации функциональных подходов (Result/Either)