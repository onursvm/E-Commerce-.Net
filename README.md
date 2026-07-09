# E-Commerce Platform (Layered Architecture)

A robust, scalable, and production-ready E-Commerce backend system built with **.NET / C#** following modern software engineering principles and clean architecture.

## 🚀 Key Features
* **Advanced Product & Order Management:** Full CRUD operations for products, categories, and shopping carts.
* **Authentication & Authorization:** Secure user management and role-based access control (Admin/User).
* **Scalable Architecture:** Designed to handle high-throughput requests with optimized business logic.

## 🏗️ Architecture & Design
The project is strictly designed around **SOLID Principles** and implemented using a **Layered (N-Tier) Architecture** to ensure clean separation of concerns and high maintainability:

* **API / Presentation Layer:** Handles HTTP requests, routes endpoints, and manages controllers.
* **Business Logic Layer:** Contains core business rules, validation logic, and orchestrates data flow.
* **Data Access Layer:** Manages database interactions and repository patterns.
* **Core / Entities Layer:** Defines central data models and Data Transfer Objects (DTOs).

## 🛠️ Tech Stack
* **Backend:** .NET / C#
* **Database:** SQL Server
* **Architecture:** N-Tier Layered Architecture, Repository Pattern

## 💻 How to Run Locally
1. Clone the repository: `git clone https://github.com/onursvm/E-Commerce-.Net.git`
2. Update the `appsettings.json` file with your local SQL Server Connection String.
3. Open the solution in Visual Studio.
4. Run `Update-Database` in the Package Manager Console to apply migrations.
5. Press `F5` to start the API.
