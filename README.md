# 📘 README – Inventory & Asset Tracking Microservices (ASP.NET 8)

This project implements a **Microservices Architecture** using **.NET 8** for an Inventory and Asset Tracking System. Each domain is encapsulated as a separate Web API service with clear separation of concerns.

---

## 🏗️ Folder Overview

### 🔧 `AssetService/`
Handles asset information (e.g., devices, machines).
- CRUD asset entries
- Track status, classification, and ownership

### 📦 `InventoryService/`
Tracks stock of assets (in/out/remaining).
- Inventory movement
- Stock alerts

### 👤 `UserService/`
Manages users (e.g., employees, admins).
- Authentication/authorization
- User info & roles

### 📝 `AssignmentService/`
Handles asset handover and returns.
- Assign/return assets to users
- Handover logs

### 📊 `ReportingService/`
Generates analytical reports.
- Export Excel/PDF
- Filter by date/type/user

### 🌐 `ApiGateway/`
Entry point for all external requests.
- Routing with Ocelot
- JWT Authentication, logging middleware

### 📚 `SharedKernel/`
Contains shared code used across services.
- DTOs, constants, interfaces

---

## 🗂️ Folder Structure in Each Service (e.g., `AssetService`)
```
AssetService/
├── Controllers/            // API endpoints
├── Services/               // Business logic
│   └── Interfaces/
├── Repositories/           // Data access layer
│   └── Interfaces/
├── Models/                 // Domain entities and DTOs
│   ├── Entities/
│   └── DTOs/
├── Data/                   // EF Core context and migrations
├── Mappings/               // AutoMapper profiles
├── Program.cs
└── appsettings.json
```

---

## 🚪 ApiGateway Setup (Ocelot)
```
ApiGateway/
├── ocelot.json             // Route config
├── Program.cs              // Startup logic
├── Middleware/             // Custom middlewares (logging, auth)
└── appsettings.json
```
---

## ✅ Start With:
Implement `AssetService` with basic CRUD, map DTOs using AutoMapper, and expose endpoints using `AssetController`.

---

Happy Coding 👨‍💻
