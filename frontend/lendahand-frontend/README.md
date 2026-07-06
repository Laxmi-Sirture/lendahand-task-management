# LendAHand — Frontend (React + Vite)

Task management system UI. Talks to the LendAHand .NET backend API.

## Setup

1. Install dependencies:
   ```
   npm install
   ```

2. Point the app at your backend — edit `.env`:
   ```
   VITE_API_URL=https://localhost:7263/api/v1
   ```
   (Change the port/host if your backend runs elsewhere.)

3. Run the dev server:
   ```
   npm run dev
   ```
   App opens at `http://localhost:5173`.

## Notes

- If your backend uses a self-signed HTTPS certificate (default `dotnet run` dev cert), your browser may block requests until you open the API URL directly once (e.g. `https://localhost:7263/swagger`) and accept the certificate warning.
- Roles: register as `Admin` or `Employee`. Admins manage employees and all tasks; Employees see and update only their own assigned tasks.
- First-time note: registering a user only creates a login (`Users` table). An **Employee dashboard** will only work once an Admin also creates a matching employee record via **Employees → Add employee** (this creates both the login and the employee profile together).

## Pages

- `/login`, `/register` — auth
- `/admin` — admin dashboard (stats)
- `/employee` — employee dashboard (stats)
- `/employees` — employee CRUD (Admin only)
- `/tasks` — task list; full CRUD for Admin, status updates for Employee
- `/notifications` — view + mark as read
