# TagStudio 🏷️

TagStudio is a modern, high-performance web application for managing tags and related entries. Built with **.NET 9.0** and **Angular 20**, it leverages **.NET Aspire** for local development orchestration and cloud-native architecture.

---

## 🚦 Getting Started

### **Prerequisites**
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Required for Aspire resources)
- [Node.js](https://nodejs.org/) (for WebClient)

### **Installation & Running**

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/TagStudio.git
   cd TagStudio
   ```

2. **Run Everything (via Aspire)**:
   This will start SQL Server, RabbitMQ, ElasticSearch, the Backend, and the Frontend.
   ```bash
   dotnet run --project src/AppHost
   ```
   *Access the Aspire Dashboard at the URL provided in the terminal.*

3. **Run Tests**:
   ```bash
   dotnet test
   ```

4. **Run Frontend Independently**:
   ```bash
   cd src/WebClient
   npm install
   npm start
   ```

---

## 🚀 Key Features

- **Tag Management**: Organize content with a flexible tagging system.
- **Global Search**: High-performance fuzzy search powered by **ElasticSearch**.
- **Real-time Synchronization**: Decoupled services communicating via **MassTransit** and **RabbitMQ**.
- **Modern UI**: Polished frontend built with **Angular 20** and **Tailwind CSS**.
- **Blob Storage**: Secure file management using **Azure Blob Storage** (local support via Azurite).
- **Secure by Design**: Integrated identity management with JWT and Refresh Token support.

---

## 🛠️ Tech Stack

### **Backend**
- **Framework**: .NET 9.0
- **Web API**: [FastEndpoints](https://fast-endpoints.com/) (Minimal APIs alternative)
- **Database**: SQL Server + Entity Framework Core
- **Messaging**: MassTransit with RabbitMQ
- **Search Engine**: ElasticSearch
- **Orchestration**: .NET Aspire
- **Mapping**: Riok.Mapperly
- **Validation**: Ardalis.GuardClauses

### **Frontend**
- **Framework**: Angular 20 (Preview)
- **Styling**: Tailwind CSS
- **State Management**: Angular Signals

### **Testing**
- **Unit/Functional**: xUnit, Shouldly, NSubstitute
- **Integration**: Testcontainers (MsSql, Azurite)

---

## 📂 Project Structure

- `src/AppHost`: .NET Aspire orchestrator – the entry point for running the entire system.
- `src/WebApi`: Main API gateway and middleware configuration.
- `src/Tags`: Service for tag and entry management (Feature-sliced).
- `src/Identity`: Authentication and user management service.
- `src/Search`: Dedicated search service using ElasticSearch.
- `src/Shared`: Common contracts, models, and interfaces used across services.
- `src/WebClient`: Angular frontend.
- `tests/`: Comprehensive unit and functional test suites.

---

## 🏗️ Architecture

TagStudio follows an **Event-Driven Microservices** architecture:
- **Tags Service**: The primary source of truth for entries.
- **Search Service**: Synchronizes data from the Tags service via integration events (`EntryCreated`, etc.) to maintain a high-performance ElasticSearch index.
- **Identity Service**: Handles user registration and authentication, providing JWTs for secure communication.

---
