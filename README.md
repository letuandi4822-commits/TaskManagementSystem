
# Task Management System

A web-based task management system built with ASP.NET Core MVC.
## Demo Accounts

### Admin

Email: admin@gmail.com

Password: 123

### User

Email: User@gmail.com

Password: 123456
## Features

- User Authentication
- Role-based Authorization (Admin/User)
- Project Management
- Task Assignment
- Email Notification
- File Upload
- Dashboard with Charts
- Activity Logs
- Notification Center

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap 5
- Chart.js
- MailKit

## Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server LocalDB

## Setup

1. Clone the repository

```bash
git clone https://github.com/letuandi4822-commits/TaskManagementSystem.git
```

2. Open `TaskManagementSystem.csproj` in Visual Studio 2022.

3. Update the database:

```powershell
Update-Database
```

4. Configure Email Settings (optional).

5. Press **F5** or click **Start** to run the application.

## Email Configuration (Optional)

To enable email notifications, update the `EmailSettings` section in `appsettings.json`.

```json
"EmailSettings": {
  "Email": "your-email@gmail.com",
  "Password": "your-app-password",
  "Host": "smtp.gmail.com",
  "Port": 587
}
