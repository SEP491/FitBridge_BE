# 🏋️‍♂️ FitBridge Backend

![FitBridge Banner](https://img.shields.io/badge/FitBridge-Backend-blue?style=for-the-badge&logo=dotnet)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat&logo=postgresql)
![Neo4j](https://img.shields.io/badge/Neo4j-008CC1?style=flat&logo=neo4j)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=flat&logo=redis)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker)

> **The central nervous system of the FitBridge Ecosystem.**  
> A high-performance, scalable backend built with **.NET 9**, employing **Clean Architecture** to power a next-generation fitness platform.

---

## 🚀 Ecosystem Repositories

FitBridge is a multi-platform solution. Explore our other repositories:

*   📱 **Mobile App (Native)**: [FitBridge_Native](https://github.com/SEP491/FitBridge_Native.git)
*   💻 **Web Portal**: [FitBridge_Web](https://github.com/SEP491/FitBridge_Web.git)
*   🤖 **AI Chatbot**: [FitBridge_Bot](https://github.com/SEP491/FitBridge_Bot.git)

---

## 🌟 Introduction

**FitBridge** is an all-in-one platform designed to bridge the gap between **Gym Owners**, **Personal Trainers (PTs)**, and **Trainees**. 

The Backend API serves as the robust core, handling everything from real-time messaging and video call signaling to complex graph-based recommendation algorithms for matching trainees with the perfect PTs.

## ✨ Key Features

### 🏢 Comprehensive Management
*   **Gym Management**: Slots, Subscriptions, Courses, and Assets management.
*   **PT Management**: Freelance & Gym-associated PT support, package creation, and scheduling.
*   **Booking System**: Seamless booking flow for gym slots and PT sessions.
*   **Review System**: Review and rating system for gyms and PTs.
*   **E-Commerce**: Shop for gym supplements.

### 🧠 Intelligent Core
*   **Graph Engine (Neo4j)**: Advanced relationship mapping between Muscles, Exercises, Certifications, and Users for hyper-personalized experience.
*   **AI Integration**: Integrated with OpenAI and a specialized Python Chatbot service for intelligent user assistance.

### ⚡ Real-Time Interactions
*   **Messaging System**: Real-time chat powered by **SignalR** and **Redis**.
*   **Video Call Signaling**: WebRTC signaling support for virtual training sessions.
*   **Live Notifications**: Instant updates via Apple Push Notification Service (APNS) and Firebase (FCM).

### 💳 Commerce & Security
*   **Payments**: Secure integration with **payOS**.
*   **Security**: Robust Authentication & Authorization system.

---

## 🏗️ Technology Stack

We use a cutting-edge stack to ensure performance, scalability, and maintainability.

| Category | Technology | Usage |
|----------|------------|-------|
| **Core Framework** | **.NET 9** | High-performance Web API |
| **Architecture** | **Clean Architecture** | Domain, Application, Infrastructure, API separation |
| **Primary Database** | **PostgreSQL** | Relational data (Users, Orders, Bookings) |
| **Graph Database** | **Neo4j** | Complex relationships & Recommendations |
| **Caching/PubSub** | **Redis** | High-speed caching & SignalR backplane |
| **Background Jobs** | **Quartz.NET** | Scheduled tasks & cron jobs |
| **Real-Time** | **SignalR** | WebSocket communication |
| **AI** | **OpenAI** | Intelligent processing |

---

## 📂 Project Structure

The solution follows the **Clean Architecture** principles:

*   **`FitBridge_API`**: The entry point. Contains Controllers, SignalR Hubs, and Middleware configuration.
*   **`FitBridge_Application`**: Contains business logic, CQRS commands/queries, and interfaces.
*   **`FitBridge_Domain`**: The heart of the system. Contains Entities, Enums, and Value Objects.
*   **`FitBridge_Infrastructure`**: Implementation of interfaces. Database contexts (EF Core, Neo4j), External Services (Email, Payment, Storage).

---

## 🛠️ Getting Started

### Prerequisites
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop) (Recommended for DBs)
*   PostgreSQL, Redis, Neo4j instances.

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/SEP491/FitBridge_BE.git
    cd FitBridge_BE
    ```

2.  **Configure Environment**
    Update `appsettings.json` (or `appsettings.Development.json`) with your connection strings:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=FitBridgeDb;Username=postgres;Password=yourpassword",
      "Redis": "localhost:6379",
      "Neo4j": "bolt://localhost:7687"
    }
    ```

3.  **Run with Docker (Optional)**
    ```bash
    docker-compose up -d
    ```

4.  **Run the API**
    ```bash
    cd FitBridge_API
    dotnet run
    ```

5.  **Explore API**
    Visit Swagger UI at `https://localhost:5001/swagger/index.html` (port may vary).

### Full document for this project
* [Documentation](https://github.com/SEP491/FitBridge_BE/blob/main/docs/FitBridge_BE_Documentation.md)
