# DawsonDial – College Management System

## Overview

DawsonDial is a full-scale **college management desktop application** built with **C# (.NET)**, **Avalonia UI**, **Entity Framework Core**, and **PostgreSQL**. The project simulates real-world academic administration workflows such as user management, course enrollment, scheduling, events, and administrative logging.

The system supports **three distinct roles** — **Admin**, **Student**, and **Teacher** — each with its own permissions, menus, and responsibilities. DawsonDial was designed with clean architecture principles, strong separation of concerns, and secure authentication practices.

---

## User Roles & Features

### Authentication & Access Control
- Login as **Admin**, **Student**, or **Teacher**
- Register directly as a **Student** from the application
- Secure password hashing using **PBKDF2 (SHA-256)** with salt
- Role-based menu routing and access enforcement

---

## Admin Features

### Admin Menu
Admins have full control over the system through a centralized admin interface.

**Profile**
- View personal profile information
- Update profile details
- Reset password

**Admin Logs**
- View a complete audit log of all admin actions
- Logs include action description, timestamp, and responsible admin

**User Management**
- View all users in the system
- Create new users (Admin, Student, Teacher)
- Update user information
- Enable or disable user accounts

**Room Management**
- View all rooms
- Create new rooms
- Modify room details

**Course Management**
- View all courses
- Create new courses
- Modify course information

**Event Management**
- View all events
- Create and update events

---

## Student Features

### Student Menu

**Profile**
- View and update profile information
- Change password

**Courses**
- View all available courses
- Enroll in a course
- View enrolled courses
- Drop courses

**Scheduling & Conferences**
- View available conferences
- Book teacher meetings
- View personal schedule
- View upcoming conferences and meetings

---

## Teacher Features

### Teacher Menu

**Profile**
- View and update profile information

**Courses**
- View assigned courses

**Scheduling**
- Manage availability and schedules

**Events**
- View and manage events related to assigned courses

---

## Technical Features

- Role-based navigation and authorization
- Secure password hashing and verification
- PostgreSQL relational database with EF Core migrations
- Admin activity logging with cascading relations
- Repository and service-layer architecture
- MVVM pattern with Avalonia UI
- Strong separation between GUI, services, and data layers

---

## Technology Stack

### Backend & Core
- C# (.NET)
- Entity Framework Core
- PostgreSQL

### Frontend
- Avalonia UI (XAML)
- MVVM Architecture

### Security
- PBKDF2 password hashing (SHA-256)
- Environment-based database configuration

### Tooling
- Docker (PostgreSQL)
- Git / GitHub

---

## Project Structure

```
DawsonDial/
├── DawsonDial/              # Core application logic
│   ├── Controllers/         # Application controllers
│   ├── Helpers/             # Utility helpers
│   ├── Migrations/          # EF Core migrations
│   ├── Models/              # Domain models (People, Rooms, Events, etc.)
│   ├── Repositories/        # Data access layer (interfaces & implementations)
│   ├── Security/            # Password hashing & security utilities
│   ├── Services/            # Business logic & service layer
│   └── Views/               # Core views / handlers
│
├── DawsonDialGUI/           # Desktop UI (Avalonia)
│   ├── Assets/              # Images and static assets
│   ├── Converters/          # XAML value converters
│   ├── ViewModels/          # MVVM ViewModels
│   └── Views/               # XAML Views
│
├── DawsonDialTests/         # Unit tests
│   ├── ManagerService/      # Service-level tests
│   └── Models/              # Model tests
│
├── db_cred.sh               # Database environment variables
├── docker-compose.yml       # PostgreSQL container setup
└── README.md                # Project documentation
```

---

## Running the Project Locally

> ⚠️ **Important:** This project requires multiple setup steps due to database configuration and environment variables.

### Prerequisites
- .NET SDK
- PostgreSQL **or** Docker
- Git

---

### 1. Clone the Repository
```bash
git clone <repository-url>
cd DawsonDial
```

---

### 2. Start PostgreSQL (Docker Recommended)

```bash
docker-compose up -d
```

This will start PostgreSQL on port `5432`.

---

### 3. Load Database Environment Variables

Using **Git Bash**:

```bash
source ./db_cred.sh
```

This exports:
- `POSTGRES_HOST`
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_PORT`

---

### 4. Apply Database Migrations

```bash
dotnet ef database update --project DawsonDial --startup-project DawsonDial
```

This creates all required tables.

---

### 5. Run the Application

```bash
dotnet run --project DawsonDialGUI
```

The desktop application will launch.

---

## Demo

A full demo walkthrough of the application is available here:

https://www.youtube.com/watch?v=5MVbZ24Strk

---

## Final Notes

DawsonDial demonstrates advanced C# application architecture, secure authentication, database integration, and a complete role-based desktop UI suitable for real-world academic systems.

