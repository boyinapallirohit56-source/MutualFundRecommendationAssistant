# Mutual Fund Recommendation Assistant

An AI-powered web application that helps users understand their investment profile and recommends suitable mutual fund allocations based on their risk appetite.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular |
| Backend | ASP.NET Core Web API (.NET 8) |
| Database | SQL Server (LocalDB) |
| Cloud | AWS (EC2, RDS) |
| AI | OpenAI API |

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Node.js (for Angular frontend)

### Backend Setup

```bash
cd src/MutualFundAPI
dotnet restore
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`).

Swagger UI available at: `http://localhost:5000/swagger`

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/v1/auth/register | Register new user |
| POST | /api/v1/auth/login | Login |
| GET | /api/v1/users/profile | Get user profile |
| POST | /api/v1/users/profile | Save/update profile |
| GET | /api/v1/risk-assessment/questions | Get questionnaire |
| POST | /api/v1/risk-assessment/submit | Submit answers |
| GET | /api/v1/risk-assessment/latest | Get latest assessment |
| POST | /api/v1/recommendations/generate | Generate recommendation |
| GET | /api/v1/recommendations/latest | Get latest recommendation |

## Project Structure

```
src/MutualFundAPI/
├── Controllers/          # API endpoints
├── Models/
│   ├── Entities/         # Database entities
│   └── DTOs/             # Data transfer objects
├── Services/             # Business logic
├── Data/
│   ├── AppDbContext.cs   # EF Core context
│   └── Seeders/          # Database seed data
├── Program.cs            # App configuration
└── appsettings.json      # Configuration
```
