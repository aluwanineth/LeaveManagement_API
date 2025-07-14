
# Leave Management System

A comprehensive leave management system built with **Clean Architecture** using .NET 9, Entity Framework Core, and JWT authentication. The system enables employees to request leave and managers to approve/reject requests based on organizational hierarchy.

## 🏗️ Architecture Overview

The solution follows **Clean Architecture** principles with clear separation of concerns:

```
LeaveManagement/
├── LeaveManagement.API/              # Web API Layer (Controllers, Program.cs)
├── LeaveManagement.Application/      # Business Logic (CQRS, DTOs, Validators)
├── LeaveManagement.Domain/           # Core Business Entities and Enums
├── LeaveManagement.Identity/         # Identity & Authentication (JWT, Users)
├── LeaveManagement.Persistence/      # Data Access (DbContext, Repositories)
├── LeaveManagement.Tests/            # Unit Tests (NUnit, NSubstitute)
└── LeaveManagement.sln              # Solution File
```

## 🚀 Key Features

### Business Features
- ✅ **Employee Management** - CRUD operations for employees
- ✅ **Leave Request System** - Create, view, edit, and cancel leave requests
- ✅ **Self-Service Cancellation** - Employees can cancel their own pending requests
- ✅ **Overlap Prevention** - Prevents duplicate/overlapping leave requests
- ✅ **Approval Workflow** - Manager-based approval system
- ✅ **Role-Based Access** - CEO, Manager, TeamLead, Employee roles
- ✅ **Business Days Validation** - South African public holidays and business day calculations
- ✅ **Audit Trail** - Track record updates with user and timestamp

### Technical Features
- ✅ **Clean Architecture** - Domain-driven design with dependency inversion
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- ✅ **Repository Pattern** - Generic and specific repositories
- ✅ **JWT Authentication** - Secure token-based authentication
- ✅ **Entity Framework Core** - Code-first with SQL Server
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Request validation with overlap checking
- ✅ **Swagger Documentation** - Interactive API documentation
- ✅ **Unit Testing** - Comprehensive test coverage

## 🛠️ Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **Framework** | .NET Core | 9.0 |
| **Database** | SQL Server | 2022+ |
| **ORM** | Entity Framework Core | 9.0.7 |
| **Authentication** | JWT Bearer Tokens | 8.12.1 |
| **Identity** | ASP.NET Core Identity | 9.0.7 |
| **API Documentation** | Swagger/OpenAPI | 6.6.2 |
| **Validation** | FluentValidation | 11.8.0 |
| **Mapping** | AutoMapper | 15.0.1 |
| **Mediator** | MediatR | 13.0.0 |
| **Testing** | NUnit | 4.6.0 |
| **Testing** | NSubstitute | 5.3.0 |
| **Logging** | Serilog | 8.0.1 |

## 📋 Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** - LocalDB, Express, or Full version
- **Visual Studio 2022** or **VS Code** (optional)
- **Git** for version control

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/aluwanineth/LeaveManagement_API.git
cd LeaveManagement_API
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Update Connection Strings
Edit `LeaveManagement.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LeaveManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JWTSettings": {
    "Key": "C1CF4B7DC4C4175B6618DE4F55CA4",
    "Issuer": "LeaveManagementAPI",
    "Audience": "LeaveManagementAPIUsers",
    "DurationInMinutes": 60
  }
}
```

### 4. Run Database Migrations
```bash
# Create and apply ApplicationDbContext migration
dotnet ef migrations add InitialCreate_Application --project LeaveManagement.Persistence --startup-project LeaveManagement.API --context ApplicationDbContext
dotnet ef database update --project LeaveManagement.Persistence --startup-project LeaveManagement.API --context ApplicationDbContext

# Create and apply IdentityContext migration
dotnet ef migrations add InitialCreate_Identity --project LeaveManagement.Identity --startup-project LeaveManagement.API --context IdentityContext
dotnet ef database update --project LeaveManagement.Identity --startup-project LeaveManagement.API --context IdentityContext
```

### 5. Run the Application
```bash
dotnet run --project LeaveManagement.API
```

The API will be available at:
- **HTTPS**: `https://localhost:7221`
- **Swagger UI**: `https://localhost:7221/swagger`

## 📊 Database Schema

### Core Entities

#### Employee
- **EmployeeId** (PK) - Auto-increment ID
- **EmployeeNumber** - Unique employee identifier
- **FullName** - Employee's full name
- **Email** - Unique email address
- **CellphoneNumber** - Optional phone number
- **EmployeeType** - CEO, Manager, TeamLead, Employee
- **ManagerId** (FK) - Reference to manager
- **Audit Fields** - LastRecordUpdateDate, LastRecordUpdateUserid

#### LeaveRequest
- **LeaveRequestId** (PK) - Auto-increment ID
- **EmployeeId** (FK) - Reference to employee
- **StartDate** - Leave start date
- **EndDate** - Leave end date
- **LeaveType** - Annual, Sick, Personal, Maternity, Paternity
- **Status** - Pending, Approved, Rejected, Cancelled
- **Comments** - Employee comments
- **ApprovedById** (FK) - Reference to approver
- **ApprovedDate** - Approval timestamp
- **ApprovalComments** - Manager comments
- **Audit Fields** - LastRecordUpdateDate, LastRecordUpdateUserid

### Identity Schema
- **Users** - ASP.NET Identity users linked to employees
- **Roles** - CEO, Manager, TeamLead, Employee
- **UserRoles** - User-role associations
- **UserLogins, UserTokens** - Authentication data

## 🔐 Authentication & Authorization

### User Registration & Login
1. **Registration** - Users register using existing employee email addresses
2. **Role Assignment** - Automatic role assignment based on EmployeeType
3. **JWT Generation** - Token includes user claims and employee information

### Role-Based Access Control

| Role | Permissions |
|------|-------------|
| **CEO** | Full access to all operations |
| **Manager** | Manage employees, approve subordinate leave requests |
| **TeamLead** | Approve team member leave requests |
| **Employee** | Create, view, and cancel own leave requests |

### API Security
- **JWT Bearer Authentication** - All endpoints except auth require valid tokens
- **Role-based Authorization** - Endpoints restricted by user roles
- **Claim-based Access** - Employee-specific data access

## 📡 API Endpoints

### Authentication Endpoints
```http
POST /api/account/register          # Register new user
POST /api/account/authenticate      # Login user
```

### Employee Endpoints
```http
GET    /api/v1/employee            # Get all employees (CEO/Manager only)
POST   /api/v1/employee            # Create employee (CEO/Manager only)
```

### Leave Request Endpoints
```http
GET    /api/v1/leaverequest/employee/{id}     # Get employee's leave requests
POST   /api/v1/leaverequest                   # Create leave request
PUT    /api/v1/leaverequest/approve           # Approve/reject leave request (Managers)
PUT    /api/v1/leaverequest/reject            # Reject leave request (Managers)
DELETE /api/v1/leaverequest/{id}/cancel       # Cancel own pending leave request (Employees)
GET    /api/v1/leaverequest/pending           # Get pending approvals (Managers)
GET    /api/v1/leaverequest/approvals         # Get approval history (Managers)
```

### Business Days Endpoints
```http
GET    /api/v1/businessdays/public-holidays/{year}     # Get public holidays for year
GET    /api/v1/businessdays/is-business-day/{date}     # Check if date is business day
POST   /api/v1/businessdays/validate-date-range        # Validate date range for leave
POST   /api/v1/businessdays/business-days-count        # Count business days in range
POST   /api/v1/businessdays/non-business-days          # Get non-business days in range
GET    /api/v1/businessdays/next-business-day/{date}   # Get next business day
GET    /api/v1/businessdays/previous-business-day/{date} # Get previous business day
```

## 🔧 Key Business Rules

### Leave Request Creation
- **Overlap Prevention**: System prevents creating requests with overlapping dates
- **Future Dates Only**: Leave requests cannot be created for past dates
- **Business Day Validation**: Optional validation for business days
- **Maximum Comments**: Comments limited to 500 characters

### Leave Request Cancellation
- **Self-Service**: Employees can cancel their own pending requests
- **Pending Only**: Only requests with "Pending" status can be cancelled
- **No Manager Approval**: Cancellation is immediate without manager intervention
- **Audit Trail**: Cancelled requests are marked as "Cancelled" (not deleted)
- **Status Update**: Request status changes to "Cancelled" with automatic comment

### Leave Request Approval/Rejection
- **Manager Authorization**: Only managers can approve/reject subordinate requests
- **Pending Only**: Only pending requests can be approved/rejected
- **Comments Required**: Approval/rejection comments are mandatory
- **Status Tracking**: Full audit trail of approval decisions

### Business Days (South African Context)
- **Public Holidays**: Automatic handling of South African public holidays
- **Weekend Exclusion**: Saturdays and Sundays are non-business days
- **Holiday Observation**: Sunday holidays moved to Monday
- **Date Range Validation**: Comprehensive business day calculations

### Request/Response Examples

#### Authentication Request
```json
POST /api/account/authenticate
{
  "email": "lindajenkins@acme.com",
  "password": "Password123!"
}
```

#### Create Leave Request
```json
POST /api/v1/leaverequest
{
  "employeeId": 1,
  "startDate": "2025-08-15",
  "endDate": "2025-08-19",
  "leaveType": 1,
  "comments": "Family vacation"
}
```

#### Cancel Leave Request
```http
DELETE /api/v1/leaverequest/19/cancel
Authorization: Bearer {jwt-token}
```

## 🧪 Testing

### Running Unit Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

```

### Test Coverage Areas
- **Controller Tests** - API endpoint testing
- **Handler Tests** - Business logic testing with CQRS commands/queries
- **Repository Tests** - Data access testing
- **Validator Tests** - Input validation and overlap detection testing
- **Service Tests** - Business day calculations and authorization testing
- **Integration Tests** - End-to-end workflow testing

### Test Categories
- **Unit Tests** - Individual component testing
- **Performance Tests** - Large dataset and concurrent operation testing
- **Edge Case Tests** - Boundary conditions and error scenarios
- **Security Tests** - Authorization and data access validation
- **Business Logic Tests** - Complex workflow and rule validation

## 🔧 Configuration

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LeaveManagementDb;Trusted_Connection=true"
  },
  "JWTSettings": {
    "Key": "YourSecretKey",
    "Issuer": "LeaveManagementAPI",
    "Audience": "LeaveManagementAPIUsers",
    "DurationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
