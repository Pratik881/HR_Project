# HR Management System

An ASP.NET Core-based HR Management System that helps streamline employee management, leave tracking, and secure access control for Admins, HR personnel, and Employees.

## 📌 Features

### 🔐 Authentication & Authorization
- Secure login using JWT tokens
- Role-based access control (`Admin`, `HR`, `Employee`)
- Password reset via email

### 👥 User & Role Management
-Seeded Admin account
- Admin can create HR and Employee accounts
- HR can create Employee accounts (Employees cannot self-register)
- Role-specific  actions

### 📝 Employee Management
- HR and Admin can add, edit, and delete employee records and view all registered employees
- Employee profile information stored and retrieved from SQL Server using Entity Framework Core

### 📆 Leave Management System
- Employees can apply for leave with a specified type and date range
- HR/Admin can view all leave requests and approve or reject pending leave requests
- Leave status tracking (Pending, Approved, Rejected)
- Leave types include Paid, Unpaid, Sick, etc.

### ❗ Leave Request Conflict Handling
- Prevents overlapping or conflicting leave requests for the same employee
- System automatically checks if the new leave request overlaps with existing approved or pending leaves

### 📧 Email Notifications (via SMTP)
- Email alerts sent for account registration, password reset
- Configured using Gmail SMTP

### ⚙️ Architecture & Practices
- ASP.NET Core Web API with Clean Architecture using DTOs, Interfaces, and Service layer abstraction
- Entity Framework Core (Code First)
- JWT for stateless authentication
- Swagger integration for API testing

## 🏗️ Technologies Used

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- SMTP (Gmail) for emails
- Swagger / Swashbuckle


## 📁 Project Structure

```
Repos/
└── HR/
    ├── HR.sln
    └── HR/
        ├── Controllers/
        ├── Models/
        ├── DTOs/
        ├── Services/
        ├── Interfaces/
        ├── Data/
        ├── Services
        ├── Utilities
        └── appsettings.json (Dummy - pushed)
```

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone 
cd HR/HR
```

### 2. Configure the Database
- Update your real connection string in `appsettings.LocalBackup.json`
- Run the database migrations:
```bash
dotnet ef database update
```

### 3. Run the Project
```bash
dotnet run
```

Visit: `https://localhost:<your-port>/swagger/index.html`

## 🔐 Security Notes

- Sensitive credentials (SMTP, JWT key, DB connection string) are not stored in version-controlled `appsettings.json`
- Real secrets should go in `appsettings.LocalBackup.json`, which is excluded via `.gitignore`
- Consider using User Secrets or environment variables for production

## 🧪 API Documentation

- Swagger UI available at `https://localhost:<your-port>/swagger/index.html`
- You can test endpoints like `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/employee`, `POST /api/leave/apply`

## 📈 Future Enhancements

- Attendance and Work Hour Tracking
- Employee Performance Module
- Blazor/React Frontend for Dashboard
- Unit Testing and Integration Tests

