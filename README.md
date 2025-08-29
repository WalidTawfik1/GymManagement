# Gym Management System UI

A comprehensive WPF application for managing gym operations including trainee management, membership tracking, and visit recording.

## Features

### 1. Trainee Management
- **Add New Trainees**: Register new gym members with full name and phone number
- **Edit Trainee Information**: Update existing trainee details
- **Delete Trainees**: Soft delete functionality (maintains data integrity)
- **View All Trainees**: List view with search and filter capabilities

### 2. Membership Management
- **Create Memberships**: Assign memberships to trainees
- **Multiple Membership Types**: 
  - Monthly, 3 Months, 6 Months, Yearly
  - Session-based (10, 20, 30 sessions)
- **Membership Status Tracking**: Active/Inactive status management
- **Session Management**: Track remaining sessions for session-based memberships
- **Edit/Delete Memberships**: Full CRUD operations

### 3. Visit Management
- **Quick Check-in**: One-click check-in for trainees
- **Visit Recording**: Manual visit entry with custom date/time
- **Visit History**: View all gym visits with trainee information
- **Automatic Session Decrement**: Sessions automatically decrease on check-in

## Architecture

### MVVM Pattern
The application follows the Model-View-ViewModel (MVVM) pattern:

- **Models**: Located in `Gym.Core/Models/`
- **ViewModels**: Located in `Gym.UI/ViewModels/`
- **Views**: Located in `Gym.UI/Views/`

### Key Components

#### ViewModels
- `MainViewModel`: Main application coordinator
- `TraineeViewModel`: Handles trainee operations
- `MembershipViewModel`: Manages membership operations
- `VisitViewModel`: Controls visit recording and display
- `BaseViewModel`: Common functionality for all ViewModels

#### Views
- `TraineeView`: Trainee management interface
- `MembershipView`: Membership management interface
- `VisitView`: Visit recording and history interface

### Data Layer
- **Entity Framework Core**: ORM for database operations
- **Repository Pattern**: Clean separation of data access logic
- **Unit of Work**: Coordinated database operations
- **AutoMapper**: Object-to-object mapping

## UI Features

### Modern Design
- **Color-coded Sections**: Each module has distinct colors
  - Trainees: Blue theme (#2C3E50)
  - Memberships: Purple theme (#8E44AD)
  - Visits: Orange theme (#E67E22)

### User Experience
- **Intuitive Navigation**: Tab-style navigation between modules
- **Real-time Updates**: Data refreshes automatically after operations
- **Progress Indicators**: Loading states for async operations
- **Confirmation Dialogs**: Safe delete operations with user confirmation
- **Form Validation**: Client-side validation with clear error messages

### Data Grid Features
- **Sortable Columns**: Click column headers to sort
- **Action Buttons**: Edit/Delete buttons in each row
- **Responsive Layout**: Adapts to window size changes

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Database Setup
1. Copy `.env.example` to `.env`
2. Update the connection string in `.env` file:
   ```
   MambelaDatabase=Data Source=.;Initial Catalog=GymManagementDB;Integrated Security=True;TrustServerCertificate=True
   ```

### Running the Application
1. Open terminal in the project root
2. Run database migrations:
   ```powershell
   dotnet ef database update --project Gym.Infrastructure --startup-project Gym.UI
   ```
3. Start the application:
   ```powershell
   cd Gym.UI
   dotnet run
   ```

## Dependencies

### NuGet Packages
- **CommunityToolkit.Mvvm**: MVVM framework
- **AutoMapper**: Object mapping
- **Microsoft.EntityFrameworkCore**: ORM
- **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider
- **Microsoft.Extensions.Hosting**: Dependency injection
- **DotNetEnv**: Environment variable management

## Project Structure

```
Gym.UI/
├── Views/                  # XAML Views
│   ├── TraineeView.xaml
│   ├── MembershipView.xaml
│   └── VisitView.xaml
├── ViewModels/            # ViewModels
│   ├── BaseViewModel.cs
│   ├── MainViewModel.cs
│   ├── TraineeViewModel.cs
│   ├── MembershipViewModel.cs
│   └── VisitViewModel.cs
├── Mapping/               # AutoMapper Profiles
│   ├── TraineeMapping.cs
│   └── MembershipMapping.cs
├── Converters.cs          # Value Converters
├── MainWindow.xaml        # Main Application Window
└── App.xaml.cs            # Application Startup & DI Configuration
```

## Key Business Rules

### Membership Rules
- Session-based memberships automatically decrease session count on check-in
- Expired memberships are automatically deactivated
- Users cannot check-in without an active membership
- Only one check-in per trainee per day

### Data Integrity
- Soft delete implementation preserves historical data
- Foreign key relationships maintained
- Audit trail with UpdatedAt timestamps

## Future Enhancements

### Planned Features
- **Reports Module**: Generate membership and visit reports
- **Staff Management**: Staff login and time tracking
- **Payment Tracking**: Membership payment history
- **Equipment Management**: Gym equipment tracking
- **Class Scheduling**: Group fitness class management
- **Member Photos**: Profile pictures for trainees
- **Barcode/QR Integration**: Quick check-in via scanning
- **Dashboard**: Analytics and key metrics display

### Technical Improvements
- **Search and Filtering**: Advanced search across all modules
- **Export Functionality**: Excel/PDF export capabilities
- **Theme System**: Dark/Light theme switching
- **Localization**: Multi-language support
- **Mobile App**: Companion mobile application
- **API Integration**: REST API for external integrations

## Support

For issues and feature requests, please refer to the project repository or contact the development team.
