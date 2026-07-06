# LendAHand — Employee Task Management System

A full-stack Task Management System built for the LendAHand India Full Stack Developer assignment.

**Tech Stack:**
- Backend: ASP.NET Core Web API (.NET 8), Entity Framework Core, SQL Server
- Frontend: React (Vite), React Router
- Auth: JWT (Access Token + Refresh Token + Remember Me)

## Features

- JWT-based authentication (Register, Login, Refresh Token, Logout, Remember Me)
- Role-based dashboards (Admin / Employee)
- Employee management (Add, Edit, Delete, Search, Sort, Pagination) — Admin only
- Task management (Create, Update, Delete, View) with business rules:
  - Due date cannot be earlier than start date
  - Completed tasks cannot be edited
  - Employees see only their own tasks; Admins see all tasks
- Notifications — triggered on task assignment, task completion, and tasks due within 1 day
- File upload — PDF/JPG/PNG attachments on tasks (max 5MB)
- Reports — Completed / Pending / Employee-wise, exportable to Excel and CSV
- Validation & centralized error handling (FluentValidation + global exception middleware)
- API versioning + Swagger documentation

## Project Structure

```
LendAHandIndia/
├── LendAHand.API              → Web API project (controllers, startup, Swagger, JWT config)
├── LendAHand.Application       → Business logic, services, DTOs, validators
├── LendAHand.Domain            → Entities, enums, exceptions
├── LendAHand.Infrastructure     → EF Core DbContext, repositories, migrations
├── frontend/lendahand-frontend → React (Vite) frontend
├── schema.sql                  → Full database schema script (EF Core migrations)
└── LendAHandIndia.slnx          → Visual Studio solution file
```

## Prerequisites

- .NET 8 SDK
- Node.js (v18+) and npm
- SQL Server (LocalDB, Express, or full instance)
- Visual Studio 2022 (recommended) or VS Code

## Backend Setup

1. Open `LendAHandIndia.slnx` in Visual Studio.

2. Update the connection string in `LendAHand.API/appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LendAHandDB;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

3. Apply database migrations — in Package Manager Console:
   ```powershell
   Update-Database -Project LendAHand.Infrastructure -StartupProject LendAHand.API
   ```
   (Alternatively, run `schema.sql` directly against your SQL Server instance using SSMS.)

4. Run the API (`LendAHand.API` as startup project):
   ```
   dotnet run --project LendAHand.API
   ```
   The API will be available at `https://localhost:7263` (Swagger UI at `/swagger`).

5. **Important:** The API uses a self-signed HTTPS dev certificate. The first time you connect from the frontend, open `https://localhost:7263/swagger` directly in your browser once and accept the certificate warning — otherwise the frontend's API calls may be silently blocked.

## Frontend Setup

1. Navigate to the frontend folder:
   ```
   cd frontend/lendahand-frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Confirm the API URL in `.env` matches your backend:
   ```
   VITE_API_URL=https://localhost:7263/api/v1
   ```

4. Run the dev server:
   ```
   npm run dev
   ```
   The app will be available at `http://localhost:5173`.

## Using the Application

1. **Register an Admin account** first (`/register`, select Role = Admin).
2. Log in as Admin → go to **Employees** → **Add employee** to onboard your first employee (this creates both their login and their employee profile).
3. Log in as that Employee to see their dashboard and assigned tasks.
4. As Admin, create tasks and assign them to employees from the **Tasks** page.
5. Employees can update task status; Admins can edit/delete tasks and view **Reports**.

> Note: Registering directly via `/register` as "Employee" only creates a login — it does **not** create an employee profile. Employee profiles (with Department/Designation) must be created by an Admin via the Employees page for that user's Employee Dashboard and task assignment to work correctly.

## API Endpoints (summary)

| Endpoint | Method | Access |
|---|---|---|
| `/api/v1/auth/register` | POST | Public |
| `/api/v1/auth/login` | POST | Public |
| `/api/v1/auth/refresh-token` | POST | Public |
| `/api/v1/auth/logout` | POST | Authenticated |
| `/api/v1/dashboard/admin` | GET | Admin |
| `/api/v1/dashboard/employee` | GET | Employee |
| `/api/v1/employees` | GET/POST/PUT/DELETE | Admin |
| `/api/v1/tasks` | GET/POST/PUT/DELETE | Authenticated (role-scoped) |
| `/api/v1/tasks/{id}/files` | GET/POST | Authenticated |
| `/api/v1/tasks/files/{fileId}` | DELETE | Authenticated |
| `/api/v1/notifications` | GET | Authenticated |
| `/api/v1/notifications/{id}/read` | PUT | Authenticated |
| `/api/v1/reports/*` | GET | Admin |

Full interactive documentation is available via Swagger at `https://localhost:7263/swagger` when the API is running.

## Architecture

See `docs/architecture-diagram.png` for the system flow diagram (Frontend ⇄ API ⇄ Database, plus the JWT auth flow).

## Author

Laxmi Sirture — Full Stack Developer Assignment Submission, LendAHand India.
