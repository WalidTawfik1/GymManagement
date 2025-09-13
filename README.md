# Gym Management System UI

A comprehensive WPF application for managing gym operations including trainee management, membership tracking, visit recording, financial management, and business analytics.

## Features

### 1. Trainee Management
- **Add New Trainees**: Register new gym members with full name and phone number
- **Edit Trainee Information**: Update existing trainee details
- **Delete Trainees**: Soft delete functionality (maintains data integrity)
- **View All Trainees**: List view with search and filter capabilities
- **Search Functionality**: Find trainees by name with real-time filtering

### 2. Membership Management
- **Create Memberships**: Assign memberships to trainees
- **Multiple Membership Types**: 
  - Monthly, 3 Months
  - Session-based (1 and 12 sessions)
- **Membership Status Tracking**: Active/Inactive status management
- **Session Management**: Track remaining sessions for session-based memberships
- **Edit/Delete Memberships**: Full CRUD operations
- **Pricing Support**: Set and track membership prices

### 3. Visit Management
- **Quick Check-in**: One-click check-in for trainees
- **Visit Recording**: Manual visit entry with custom date/time
- **Visit History**: View all gym visits with trainee information
- **Automatic Session Decrement**: Sessions automatically decrease on check-in

### 4. Additional Services Management ⭐ *NEW*
- **Service Types**: Manage services like Treadmill, Scale, and InBody
- **Service Tracking**: Record service usage with duration and pricing
- **Revenue Generation**: Additional services contribute to total revenue
- **Service History**: Track all additional services provided to trainees

### 5. Financial Management ⭐ *NEW*
- **Expense Tracking**: Record and categorize gym expenses
- **Revenue Calculation**: Automatic calculation from memberships and services
- **Monthly Financial Analysis**: View income, expenses, and profit by month
- **Net Profit Tracking**: Real-time profit/loss calculations
- **Financial Details**: Drill-down view of all financial transactions
- **Multi-Month Comparison**: Compare financial performance across months

### 6. Dashboard & Analytics ⭐ *NEW*
- **Key Performance Indicators**: 
  - Total active members
  - Visits this month
  - Memberships ending soon
  - Net profit tracking
- **Growth Metrics**: Month-over-month growth analysis
- **Membership Distribution**: Visual breakdown of membership types
- **Upcoming Expirations**: Alert system for expiring memberships
- **Revenue vs Expenses**: Financial performance overview

### 7. Advanced Reporting System ⭐ *NEW*
- **PDF Report Generation**: Professional PDF reports using QuestPDF
- **Dashboard Reports**: Comprehensive gym statistics and analytics
- **Financial Reports**: Detailed monthly financial statements
- **Automatic Report Organization**: Reports saved to Documents/GymReports
- **Arabic Report Support**: Fully localized reports in Arabic
- **Export Functionality**: One-click export with automatic folder opening

### 8. Multi-Language Support ⭐ *NEW*
- **Bilingual Interface**: Full Arabic and English language support
- **Real-time Language Switching**: Change language without restart
- **Localized Content**: All text, messages, and reports fully translated
- **Cultural Adaptation**: Right-to-left text support for Arabic

## Architecture

### MVVM Pattern
The application follows the Model-View-ViewModel (MVVM) pattern:

- **Models**: Located in `Gym.Core/Models/`
- **ViewModels**: Located in `Gym.UI/ViewModels/`
- **Views**: Located in `Gym.UI/Views/`

### Key Components

#### ViewModels
- `MainViewModel`: Main application coordinator and navigation hub
- `MainMenuViewModel`: Main menu with navigation cards
- `TraineeViewModel`: Handles trainee operations
- `MembershipViewModel`: Manages membership operations
- `VisitViewModel`: Controls visit recording and display
- `AdditionalServiceViewModel`: Manages additional services ⭐ *NEW*
- `ExpenseRevenueViewModel`: Financial management and tracking ⭐ *NEW*
- `FinancialDetailsViewModel`: Detailed financial transaction view ⭐ *NEW*
- `DashboardViewModel`: Analytics and key performance indicators ⭐ *NEW*
- `BaseViewModel`: Common functionality and localization support

#### Views
- `MainMenuView`: Modern card-based navigation interface ⭐ *NEW*
- `TraineeView`: Trainee management interface
- `MembershipView`: Membership management interface
- `VisitView`: Visit recording and history interface
- `AdditionalServiceView`: Additional services management ⭐ *NEW*
- `ExpenseRevenueView`: Financial management dashboard ⭐ *NEW*
- `FinancialDetailsView`: Detailed financial analysis ⭐ *NEW*
- `DashboardView`: Analytics and business intelligence ⭐ *NEW*

### Data Layer
- **Entity Framework Core**: ORM for database operations
- **Repository Pattern**: Clean separation of data access logic
- **Unit of Work**: Coordinated database operations
- **AutoMapper**: Object-to-object mapping

### Services Layer ⭐ *NEW*
- **Report Service**: PDF report generation and data export
- **Localization Service**: Multi-language support system
- **Dialog Service**: Centralized UI dialog management
- **Dashboard Repository**: Specialized analytics data access

## UI Features

### Modern Design ⭐ *ENHANCED*
- **Card-based Navigation**: Modern main menu with visual navigation cards
- **Color-coded Sections**: Each module has distinct visual identity
  - Trainees: Blue theme (#2C3E50)
  - Memberships: Purple theme (#8E44AD)
  - Visits: Orange theme (#E67E22)
  - Additional Services: Green theme
  - Financial Management: Red/Gold theme
  - Dashboard: Multi-color analytics theme
- **Consistent Styling**: Unified modern UI components across all views
- **Responsive Icons**: Visual indicators for each management section

### User Experience ⭐ *ENHANCED*
- **Dynamic Navigation**: Smart navigation system with breadcrumb support
- **Real-time Updates**: Data refreshes automatically after operations
- **Progress Indicators**: Loading states for async operations with visual feedback
- **Smart Dialogs**: Modern confirmation dialogs with contextual messages
- **Form Validation**: Client-side validation with clear error messages
- **Language Toggle**: Seamless language switching in the main interface
- **Auto-refresh**: Intelligent data refresh when navigating between views

### Data Grid Features ⭐ *ENHANCED*
- **Sortable Columns**: Click column headers to sort
- **Action Buttons**: Edit/Delete buttons in each row with modern styling
- **Responsive Layout**: Adapts to window size changes
- **Search Integration**: Built-in search functionality within data grids
- **Contextual Menus**: Right-click actions for enhanced productivity

### Financial Dashboard ⭐ *NEW*
- **Real-time Metrics**: Live financial KPIs and performance indicators
- **Visual Analytics**: Charts and graphs for data visualization
- **Monthly Comparisons**: Side-by-side month comparison views
- **Profit/Loss Indicators**: Color-coded financial health indicators
- **Export Integration**: One-click report generation from any view

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Configuration Setup ⭐ *ENHANCED*
1. **Environment Configuration**: 
   ```bash
   # Copy environment template
   cp .env.example .env
   
   # Copy application settings templates
   cp Gym.UI/appsettings.example.json Gym.UI/appsettings.json
   cp Gym.UI/appsettings.Development.example.json Gym.UI/appsettings.Development.json
   ```

2. **Update Connection Strings**: Edit the copied files with your database details
   - For development: Use LocalDB connection string in `.env`
   - For production: Use production database in `appsettings.Production.json`

3. **Security Note**: Configuration files with actual connection strings are automatically excluded from Git tracking

### Database Setup
```powershell
# Run database migrations
dotnet ef database update --project Gym.Infrastructure --startup-project Gym.UI
```

### Running the Application
```powershell
# Navigate to UI project
cd Gym.UI

# Run the application
dotnet run
```

### Building for Release
```powershell
# Build release version
dotnet build -c Release

# Publish self-contained executable
dotnet publish Gym.UI -c Release --self-contained true -r win-x64
```

## Dependencies

### NuGet Packages
- **CommunityToolkit.Mvvm**: MVVM framework with relay commands
- **AutoMapper**: Object mapping for DTOs and entities
- **Microsoft.EntityFrameworkCore**: ORM framework
- **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider
- **Microsoft.Extensions.Hosting**: Dependency injection container
- **Microsoft.Extensions.Configuration.Json**: JSON configuration support ⭐ *NEW*
- **DotNetEnv**: Environment variable management
- **QuestPDF**: Professional PDF report generation ⭐ *NEW*
- **LiveCharts.Wpf**: Charts and data visualization ⭐ *NEW*

## Project Structure

```
Gym.UI/
├── Views/                     # XAML Views
│   ├── MainMenuView.xaml      # Modern navigation interface ⭐ *NEW*
│   ├── TraineeView.xaml
│   ├── MembershipView.xaml
│   ├── VisitView.xaml
│   ├── AdditionalServiceView.xaml    ⭐ *NEW*
│   ├── ExpenseRevenueView.xaml       ⭐ *NEW*
│   ├── FinancialDetailsView.xaml     ⭐ *NEW*
│   ├── DashboardView.xaml            ⭐ *NEW*
│   └── Dialogs/              # Modern dialog system ⭐ *NEW*
│       ├── ModernDialogWindow.xaml
│       └── ModernDialogViewModel.cs
├── ViewModels/               # ViewModels
│   ├── BaseViewModel.cs      # Enhanced with localization
│   ├── MainViewModel.cs      # Navigation coordinator
│   ├── MainMenuViewModel.cs           ⭐ *NEW*
│   ├── TraineeViewModel.cs
│   ├── MembershipViewModel.cs
│   ├── VisitViewModel.cs
│   ├── AdditionalServiceViewModel.cs  ⭐ *NEW*
│   ├── ExpenseRevenueViewModel.cs     ⭐ *NEW*
│   ├── FinancialDetailsViewModel.cs   ⭐ *NEW*
│   └── DashboardViewModel.cs          ⭐ *NEW*
├── Services/                 # Application Services ⭐ *NEW*
│   ├── LocalizationService.cs        # Multi-language support
│   └── Dialogs/
│       ├── IDialogService.cs
│       └── DialogService.cs
├── Mapping/                  # AutoMapper Profiles
│   ├── TraineeMapping.cs
│   ├── MembershipMapping.cs
│   ├── ExpenseMapping.cs              ⭐ *NEW*
│   ├── VisitMapping.cs                ⭐ *NEW*
│   └── AdditionalServiceMapping.cs    ⭐ *NEW*
├── Resources/                # UI Resources ⭐ *NEW*
│   ├── ModernStyles.xaml     # Consistent styling
│   └── [Various Icons]       # Navigation and UI icons
├── Converters.cs             # Enhanced value converters
├── MainWindow.xaml           # Main Application Window
└── App.xaml.cs              # Enhanced startup with configuration
```

## Key Business Rules

### Membership Rules
- Session-based memberships automatically decrease session count on check-in
- Expired memberships are automatically deactivated  
- Users cannot check-in without an active membership
- Only one check-in per trainee per day
- Membership pricing is tracked and contributes to revenue calculations ⭐ *NEW*

### Financial Rules ⭐ *NEW*
- Revenue is automatically calculated from memberships and additional services
- Expenses are categorized and tracked by month
- Net profit calculations are updated in real-time
- Financial reports include detailed breakdowns of all transactions
- All financial data supports multi-currency formatting

### Data Integrity ⭐ *ENHANCED*
- Soft delete implementation preserves historical data
- Foreign key relationships maintained across all entities
- Audit trail with UpdatedAt timestamps on all operations
- Referential integrity between trainees, memberships, visits, and services
- Automatic data validation and consistency checks

## Recent Updates ⭐ *COMPLETED*

### ✅ Implemented Features (v2.0)
- **✅ Reports Module**: Professional PDF reports with QuestPDF
- **✅ Dashboard**: Complete analytics and KPI display  
- **✅ Financial Management**: Full expense/revenue tracking system
- **✅ Export Functionality**: PDF export with automatic organization
- **✅ Localization**: Complete Arabic/English multi-language support
- **✅ Advanced Search**: Search functionality across all modules
- **✅ Modern UI**: Card-based navigation and consistent styling
- **✅ Enhanced Data Models**: Additional services and financial entities

## Future Enhancements

### Planned Features (v3.0)
- **Staff Management**: Staff login and time tracking
- **Payment Tracking**: Detailed membership payment history
- **Equipment Management**: Gym equipment tracking and maintenance
- **Class Scheduling**: Group fitness class management
- **Member Photos**: Profile pictures for trainees
- **Barcode/QR Integration**: Quick check-in via scanning
- **Subscription Management**: Recurring payment handling
- **Mobile Notifications**: SMS/Email notifications for expiring memberships

### Technical Improvements
- **Theme System**: Dark/Light theme switching
- **Mobile App**: Companion mobile application
- **API Integration**: REST API for external integrations
- **Cloud Sync**: Cloud backup and multi-location sync
- **Advanced Analytics**: Predictive analytics and business intelligence
- **Integration APIs**: Third-party fitness app integrations

## Support

For issues and feature requests, please refer to the project repository or contact the development team.
