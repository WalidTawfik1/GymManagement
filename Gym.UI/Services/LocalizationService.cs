using System.Collections.Generic;
using System.ComponentModel;

namespace Gym.UI.Services
{
    public interface ILocalizationService : INotifyPropertyChanged
    {
        string GetString(string key);
        void SetLanguage(string languageCode);
        string CurrentLanguage { get; }
        bool IsRightToLeft { get; }
        event EventHandler? LanguageChanged;
    }

    public class LocalizationService : ILocalizationService, INotifyPropertyChanged
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations;
        private string _currentLanguage = "ar";

        public string CurrentLanguage 
        { 
            get => _currentLanguage;
            private set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged(nameof(CurrentLanguage));
                    OnPropertyChanged(nameof(IsRightToLeft));
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsRightToLeft => _currentLanguage == "ar";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? LanguageChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LocalizationService()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["ar"] = new Dictionary<string, string>
                {
                    // General
                    ["AppTitle"] = "Mambela Gym",
                    ["Language"] = "اللغة:",
                    ["Arabic"] = "عربي",
                    ["English"] = "الإنجليزية",
                    ["Add"] = "إضافة",
                    ["Edit"] = "تعديل",
                    ["Delete"] = "حذف",
                    ["Save"] = "حفظ",
                    ["Cancel"] = "إلغاء",
                    ["Clear"] = "مسح",
                    ["Refresh"] = "تحديث",
                    ["Search"] = "بحث",
                    ["Actions"] = "الإجراءات",
                    ["Status"] = "الحالة",
                    ["Active"] = "نشط",
                    ["Inactive"] = "غير نشط",
                    ["Yes"] = "نعم",
                    ["No"] = "لا",
                    ["OK"] = "موافق",
                    
                    // Navigation
                    ["Trainees"] = "المتدربين",
                    ["Memberships"] = "العضويات", 
                    ["Visits"] = "الزيارات",
                    ["AdditionalServices"] = "الخدمات الإضافية",
                    ["ExpenseRevenue"] = "المصروفات والإيرادات",
                    ["TraineeManagementTitle"] = "إدارة المتدربين",
                    ["MembershipManagementTitle"] = "إدارة العضويات", 
                    ["VisitManagementTitle"] = "إدارة الزيارات",
                    ["AdditionalServiceManagementTitle"] = "إدارة الخدمات الإضافية",
                    ["ExpenseRevenueManagementTitle"] = "إدارة المصروفات والإيرادات",
                    ["TraineeManagementDesc"] = "إدارة معلومات أعضاء النادي والمتدربين",
                    ["MembershipManagementDesc"] = "إدارة الاشتراكات وخطط العضوية",
                    ["VisitManagementDesc"] = "تتبع زيارات النادي وسجلات الحضور",
                    ["AdditionalServiceManagementDesc"] = "إدارة الخدمات الإضافية مثل المشاية والميزان وInBody",
                    ["ExpenseRevenueManagementDesc"] = "إدارة مصروفات النادي وحساب الأرباح",
                    ["DashboardManagementTitle"] = "لوحة التحكم",
                    ["DashboardManagementDesc"] = "عرض الإحصائيات والتحليلات الشاملة للنادي",
                    ["Dashboard"] = "لوحة التحكم",
                    
                    // Dashboard Page
                    ["DashboardTitle"] = "لوحة التحكم",
                    ["DashboardSubtitle"] = "إحصائيات وتحليلات إدارة النادي",
                    ["LoadingDashboardData"] = "جاري تحميل بيانات لوحة التحكم...",
                    ["ActiveMembers"] = "الأعضاء النشطين",
                    ["VisitsThisMonth"] = "زيارات هذا الشهر",
                    ["EndingSoon"] = "ينتهي قريباً",
                    ["NetProfitThisMonth"] = "صافي الربح هذا الشهر",
                    ["TotalRevenue"] = "إجمالي الإيرادات",
                    ["TotalExpenses"] = "إجمالي المصروفات",
                    ["NewMembers"] = "أعضاء جدد",
                    ["TotalTrainees"] = "إجمالي المتدربين",
                    ["MembershipDistribution"] = "توزيع الاشتراكات",
                    ["MonthlyGrowth"] = "النمو الشهري",
                    ["RevenueGrowth"] = "نمو الإيرادات",
                    ["MemberGrowth"] = "نمو العضويات",
                    ["VisitGrowth"] = "نمو الزيارات",
                    ["UpcomingMembershipExpirations"] = "الاشتراكات المنتهية قريباً",
                    ["TraineeName"] = "اسم المتدرب",
                    ["EndDate"] = "تاريخ الانتهاء",
                    ["OneMonth"] = "شهر واحد",
                    ["ThreeMonths"] = "3 شهور",
                    ["TwelveSessions"] = "12 حصة",
                    
                    // Trainee Management
                    ["TraineeManagement"] = "إدارة المتدربين",
                    ["TraineesList"] = "قائمة المتدربين",
                    ["AddNewTrainee"] = "إضافة متدرب جديد",
                    ["EditTrainee"] = "تعديل المتدرب",
                    ["FullName"] = "الاسم الكامل",
                    ["PhoneNumber"] = "رقم الهاتف",
                    ["JoinDate"] = "تاريخ الانضمام",
                    ["AddTrainee"] = "إضافة متدرب",
                    ["UpdateTrainee"] = "تحديث المتدرب",
                    ["ClearForm"] = "مسح النموذج",
                    
                    // Membership Management
                    ["MembershipManagement"] = "إدارة العضويات",
                    ["MembershipsList"] = "قائمة العضويات",
                    ["AddNewMembership"] = "إضافة عضوية جديدة",
                    ["EditMembership"] = "تعديل العضوية",
                    ["SelectTrainee"] = "اختر المتدرب:",
                    ["MembershipType"] = "نوع العضوية:",
                    ["StartDate"] = "تاريخ البداية:",
                    ["EndDate"] = "تاريخ النهاية:",
                    ["RemainingSessions"] = "الحصص المتبقية:",
                    ["IsActive"] = "نشط",
                    ["AddMembership"] = "إضافة عضوية",
                    ["UpdateMembership"] = "تحديث العضوية",
                    ["Type"] = "النوع",
                    ["Start"] = "البداية",
                    ["End"] = "النهاية",
                    ["Sessions"] = "الحصص",
                    
                    // Visit Management
                    ["VisitManagement"] = "إدارة الزيارات",
                    ["RecentVisits"] = "الزيارات الأخيرة",
                    ["CheckInVisit"] = "تسجيل زيارة",
                    ["TraineeId"] = "رقم المتدرب:",
                    ["VisitDateTime"] = "تاريخ ووقت الزيارة:",
                    ["QuickCheckIn"] = "تسجيل دخول سريع (الآن)",
                    ["RecordVisit"] = "تسجيل زيارة",
                    ["VisitDate"] = "تاريخ الزيارة",
                    ["Trainee"] = "المتدرب",
                    ["LastUpdated"] = "آخر تحديث",
                    
                    // Additional Service Management
                    ["AdditionalServiceManagement"] = "إدارة الخدمات الإضافية",
                    ["AdditionalServicesList"] = "قائمة الخدمات الإضافية",
                    ["AddNewAdditionalService"] = "إضافة خدمة إضافية جديدة",
                    ["EditAdditionalService"] = "تعديل الخدمة الإضافية",
                    ["ServiceType"] = "نوع الخدمة:",
                    ["Duration"] = "المدة (بالدقائق):",
                    ["Price"] = "السعر:",
                    ["ServiceDate"] = "تاريخ الخدمة",
                    ["AddAdditionalService"] = "إضافة خدمة",
                    ["UpdateAdditionalService"] = "تحديث خدمة",
                    
                    // Expense & Revenue Management
                    ["ExpenseManagement"] = "إدارة المصروفات والإيرادات",
                    ["Month"] = "الشهر",
                    ["ExpenseType"] = "نوع المصروف",
                    ["Amount"] = "المبلغ", 
                    ["Description"] = "الوصف",
                    ["Optional"] = "اختياري",
                    ["AddExpense"] = "إضافة مصروف",
                    ["UpdateExpense"] = "تحديث مصروف",
                    ["DeleteExpense"] = "حذف مصروف",
                    ["TotalExpenses"] = "إجمالي المصروفات",
                    ["TotalRevenue"] = "إجمالي الإيرادات",
                    ["NetProfit"] = "صافي الربح",
                    
                    // Membership Types
                    ["مفتوح"] = "مفتوح",
                    ["12 حصة"] = "12 حصة",
                    ["3 شهور"] = "3 شهور",
                    ["شهر"] = "شهر",
                    
                    // Messages
                    ["SuccessTitle"] = "نجح",
                    ["ErrorTitle"] = "خطأ",
                    ["WarningTitle"] = "تحذير",
                    ["ConfirmTitle"] = "تأكيد",
                    ["TraineeAddedSuccess"] = "تم إضافة المتدرب بنجاح!",
                    ["TraineeUpdatedSuccess"] = "تم تحديث المتدرب بنجاح!",
                    ["TraineeDeletedSuccess"] = "تم حذف المتدرب بنجاح!",
                    ["MembershipAddedSuccess"] = "تم إضافة العضوية بنجاح!",
                    ["MembershipUpdatedSuccess"] = "تم تحديث العضوية بنجاح!",
                    ["MembershipDeletedSuccess"] = "تم حذف العضوية بنجاح!",
                    ["VisitRecordedSuccess"] = "تم تسجيل الزيارة بنجاح!",
                    ["VisitDeletedSuccess"] = "تم حذف سجل الزيارة بنجاح!",
                    ["FillRequiredFields"] = "يرجى ملء جميع الحقول المطلوبة.",
                    ["ConfirmDelete"] = "هل أنت متأكد من أنك تريد حذف {0}؟",
                    ["ConfirmDeleteMembership"] = "هل أنت متأكد من أنك تريد حذف هذه العضوية؟",
                    ["ConfirmDeleteVisit"] = "هل أنت متأكد من أنك تريد حذف سجل الزيارة هذا؟",
                    ["SelectTraineeForCheckIn"] = "يرجى اختيار متدرب لتسجيل الدخول السريع.",
                    ["SelectTraineeAndType"] = "يرجى اختيار متدرب ونوع العضوية.",
                    ["ErrorLoadingTrainees"] = "خطأ في تحميل المتدربين: {0}",
                    ["ErrorLoadingMemberships"] = "خطأ في تحميل العضويات: {0}",
                    ["ErrorLoadingVisits"] = "خطأ في تحميل الزيارات: {0}",
                    ["ErrorAddingTrainee"] = "خطأ في إضافة المتدرب: {0}",
                    ["ErrorUpdatingTrainee"] = "خطأ في تحديث المتدرب: {0}",
                    ["ErrorDeletingTrainee"] = "خطأ في حذف المتدرب: {0}",
                    ["ErrorAddingMembership"] = "خطأ في إضافة العضوية: {0}",
                    ["ErrorUpdatingMembership"] = "خطأ في تحديث العضوية: {0}",
                    ["ErrorDeletingMembership"] = "خطأ في حذف العضوية: {0}",
                    ["ErrorRecordingVisit"] = "خطأ في تسجيل الزيارة: {0}",
                    ["ErrorDeletingVisit"] = "خطأ في حذف الزيارة: {0}",
                    ["FailedAddTrainee"] = "فشل في إضافة المتدرب.",
                    ["FailedUpdateTrainee"] = "فشل في تحديث المتدرب.",
                    ["FailedAddMembership"] = "فشل في إضافة العضوية.",
                    ["FailedUpdateMembership"] = "فشل في تحديث العضوية.",
                    ["PleaseEnterTraineeId"] = "يرجى إدخال رقم المتدرب",
                    ["ValidationErrorTitle"] = "خطأ في التحقق",
                    ["EnterValidTraineeId"] = "يرجى إدخال رقم متدرب صالح.",
                    ["EnterTraineeIdAndType"] = "يرجى إدخال رقم المتدرب واختيار نوع العضوية.",
                    ["TraineeNotFound"] = "المتدرب غير موجود. يرجى التحقق من رقم المتدرب.",
                    ["FailedAddMembershipGeneric"] = "فشل في إضافة العضوية.",
                    ["FailedUpdateMembershipGeneric"] = "فشل في تحديث العضوية.",
                    ["MembershipUpdatedSuccess"] = "تم تحديث العضوية بنجاح!",
                    ["MembershipAddedSuccessOpen"] = "تم إضافة العضوية بنجاح (مفتوح)",
                    ["MembershipAddedSuccessLimited"] = "تم إضافة العضوية بنجاح (12 حصة)",
                    ["MembershipAddedSuccess3Months"] = "تم إضافة العضوية بنجاح (3 شهور)",
                    ["MembershipAddedSuccess1Month"] = "تم إضافة العضوية بنجاح (شهر)",
                    ["MembershipAddedSuccessGeneric"] = "تم إضافة العضوية بنجاح",
                    ["EnterAllRequiredFields"] = "يرجى ملء جميع الحقول المطلوبة.",
                    ["EnterTraineeIdToSearch"] = "أدخل رقم المتدرب للبحث",
                    ["InvalidTraineeNumber"] = "رقم متدرب غير صالح",
                    ["NoActiveMembership"] = "لا توجد عضوية نشطة لهذا المتدرب",
                    ["SearchResultTitle"] = "نتيجة البحث",
                    ["NotificationTitle"] = "تنبيه",
                    ["DeleteMembershipConfirmTitle"] = "تأكيد", // uses ConfirmTitle existing
                    ["EnterValidTraineeIdShort"] = "يرجى إدخال رقم متدرب صالح"
                    , ["EnterTraineeNameToSearch"] = "أدخل اسم المتدرب للبحث"
                    , ["EnterTraineeName"] = "ادخل اسم المتدرب"
                    , ["NoTraineesFound"] = "لا توجد نتائج"
                },
                ["en"] = new Dictionary<string, string>
                {
                    // General
                    ["AppTitle"] = "Mambela Gym",
                    ["Language"] = "Language:",
                    ["Arabic"] = "Arabic",
                    ["English"] = "English",
                    ["Add"] = "Add",
                    ["Edit"] = "Edit",
                    ["Delete"] = "Delete",
                    ["Save"] = "Save",
                    ["Cancel"] = "Cancel",
                    ["Clear"] = "Clear",
                    ["Refresh"] = "Refresh",
                    ["Search"] = "Search",
                    ["Actions"] = "Actions",
                    ["Status"] = "Status",
                    ["Active"] = "Active",
                    ["Inactive"] = "Inactive",
                    ["Yes"] = "Yes",
                    ["No"] = "No",
                    ["OK"] = "OK",
                    
                    // Navigation
                    ["Trainees"] = "Trainees",
                    ["Memberships"] = "Memberships",
                    ["Visits"] = "Visits",
                    ["AdditionalServices"] = "Additional Services",
                    ["ExpenseRevenue"] = "Expenses & Revenue",
                    ["TraineeManagementTitle"] = "Trainee Management",
                    ["MembershipManagementTitle"] = "Membership Management", 
                    ["VisitManagementTitle"] = "Visit Management",
                    ["AdditionalServiceManagementTitle"] = "Additional Service Management",
                    ["ExpenseRevenueManagementTitle"] = "Expense & Revenue Management",
                    ["TraineeManagementDesc"] = "Manage gym members and trainee information",
                    ["MembershipManagementDesc"] = "Handle subscriptions and membership plans",
                    ["VisitManagementDesc"] = "Track gym visits and attendance records",
                    ["AdditionalServiceManagementDesc"] = "Manage additional services like treadmill, scale and InBody",
                    ["ExpenseRevenueManagementDesc"] = "Manage gym expenses and calculate profits",
                    ["DashboardManagementTitle"] = "Dashboard",
                    ["DashboardManagementDesc"] = "View comprehensive gym statistics and analytics",
                    ["Dashboard"] = "Dashboard",
                    
                    // Dashboard Page
                    ["DashboardTitle"] = "Dashboard",
                    ["DashboardSubtitle"] = "Gym Management Insights & Analytics",
                    ["LoadingDashboardData"] = "Loading dashboard data...",
                    ["ActiveMembers"] = "Active Members",
                    ["VisitsThisMonth"] = "Visits This Month",
                    ["EndingSoon"] = "Ending Soon",
                    ["NetProfitThisMonth"] = "Net Profit This Month",
                    ["TotalRevenue"] = "Total Revenue",
                    ["TotalExpenses"] = "Total Expenses",
                    ["NewMembers"] = "New Members",
                    ["TotalTrainees"] = "Total Trainees",
                    ["MembershipDistribution"] = "Membership Distribution",
                    ["MonthlyGrowth"] = "Monthly Growth",
                    ["RevenueGrowth"] = "Revenue Growth",
                    ["MemberGrowth"] = "Member Growth",
                    ["VisitGrowth"] = "Visit Growth",
                    ["UpcomingMembershipExpirations"] = "Upcoming Membership Expirations",
                    ["TraineeName"] = "Trainee Name",
                    ["EndDate"] = "End Date",
                    ["OneMonth"] = "1 Month",
                    ["ThreeMonths"] = "3 Months",
                    ["TwelveSessions"] = "12 Sessions",
                    
                    // Trainee Management
                    ["TraineeManagement"] = "Trainee Management",
                    ["TraineesList"] = "Trainees List",
                    ["AddNewTrainee"] = "Add New Trainee",
                    ["EditTrainee"] = "Edit Trainee",
                    ["FullName"] = "Full Name",
                    ["PhoneNumber"] = "Phone Number",
                    ["JoinDate"] = "Join Date",
                    ["AddTrainee"] = "Add Trainee",
                    ["UpdateTrainee"] = "Update Trainee",
                    ["ClearForm"] = "Clear Form",
                    
                    // Membership Management
                    ["MembershipManagement"] = "Membership Management",
                    ["MembershipsList"] = "Memberships List",
                    ["AddNewMembership"] = "Add New Membership",
                    ["EditMembership"] = "Edit Membership",
                    ["SelectTrainee"] = "Select Trainee:",
                    ["MembershipType"] = "Membership Type:",
                    ["StartDate"] = "Start Date:",
                    ["EndDate"] = "End Date:",
                    ["RemainingSessions"] = "Remaining Sessions:",
                    ["IsActive"] = "Is Active",
                    ["AddMembership"] = "Add Membership",
                    ["UpdateMembership"] = "Update Membership",
                    ["Type"] = "Type",
                    ["Start"] = "Start",
                    ["End"] = "End",
                    ["Sessions"] = "Sessions",
                    
                    // Visit Management
                    ["VisitManagement"] = "Visit Management",
                    ["RecentVisits"] = "Recent Visits",
                    ["CheckInVisit"] = "Check-in Visit",
                    ["TraineeId"] = "Trainee ID:",
                    ["VisitDateTime"] = "Visit Date/Time:",
                    ["QuickCheckIn"] = "Quick Check-in (Now)",
                    ["RecordVisit"] = "Record Visit",
                    ["VisitDate"] = "Visit Date",
                    ["Trainee"] = "Trainee",
                    ["LastUpdated"] = "Last Updated",
                    
                    // Additional Service Management
                    ["AdditionalServiceManagement"] = "Additional Service Management",
                    ["AdditionalServicesList"] = "Additional Services List",
                    ["AddNewAdditionalService"] = "Add New Additional Service",
                    ["EditAdditionalService"] = "Edit Additional Service",
                    ["ServiceType"] = "Service Type:",
                    ["Duration"] = "Duration (minutes):",
                    ["Price"] = "Price:",
                    ["ServiceDate"] = "Service Date",
                    ["AddAdditionalService"] = "Add Service",
                    ["UpdateAdditionalService"] = "Update Service",
                    
                    // Expense & Revenue Management
                    ["ExpenseManagement"] = "Expense & Revenue Management",
                    ["Month"] = "Month",
                    ["ExpenseType"] = "Expense Type",
                    ["Amount"] = "Amount",
                    ["Description"] = "Description", 
                    ["Optional"] = "Optional",
                    ["AddExpense"] = "Add Expense",
                    ["UpdateExpense"] = "Update Expense",
                    ["DeleteExpense"] = "Delete Expense",
                    ["TotalExpenses"] = "Total Expenses",
                    ["TotalRevenue"] = "Total Revenue",
                    ["NetProfit"] = "Net Profit",
                    
                    // Membership Types
                    ["مفتوح"] = "Open",
                    ["12 حصة"] = "12 Sessions",
                    ["3 شهور"] = "3 Months", 
                    ["شهر"] = "1 Month",
                    
                    // Messages
                    ["SuccessTitle"] = "Success",
                    ["ErrorTitle"] = "Error",
                    ["WarningTitle"] = "Warning",
                    ["ConfirmTitle"] = "Confirm",
                    ["TraineeAddedSuccess"] = "Trainee added successfully!",
                    ["TraineeUpdatedSuccess"] = "Trainee updated successfully!",
                    ["TraineeDeletedSuccess"] = "Trainee deleted successfully!",
                    ["MembershipAddedSuccess"] = "Membership added successfully!",
                    ["MembershipUpdatedSuccess"] = "Membership updated successfully!",
                    ["MembershipDeletedSuccess"] = "Membership deleted successfully!",
                    ["VisitRecordedSuccess"] = "Visit recorded successfully!",
                    ["VisitDeletedSuccess"] = "Visit record deleted successfully!",
                    ["FillRequiredFields"] = "Please fill in all required fields.",
                    ["ConfirmDelete"] = "Are you sure you want to delete {0}?",
                    ["ConfirmDeleteMembership"] = "Are you sure you want to delete this membership?",
                    ["ConfirmDeleteVisit"] = "Are you sure you want to delete this visit record?",
                    ["SelectTraineeForCheckIn"] = "Please select a trainee for quick check-in.",
                    ["SelectTraineeAndType"] = "Please select a trainee and membership type.",
                    ["ErrorLoadingTrainees"] = "Error loading trainees: {0}",
                    ["ErrorLoadingMemberships"] = "Error loading memberships: {0}",
                    ["ErrorLoadingVisits"] = "Error loading visits: {0}",
                    ["ErrorAddingTrainee"] = "Error adding trainee: {0}",
                    ["ErrorUpdatingTrainee"] = "Error updating trainee: {0}",
                    ["ErrorDeletingTrainee"] = "Error deleting trainee: {0}",
                    ["ErrorAddingMembership"] = "Error adding membership: {0}",
                    ["ErrorUpdatingMembership"] = "Error updating membership: {0}",
                    ["ErrorDeletingMembership"] = "Error deleting membership: {0}",
                    ["ErrorRecordingVisit"] = "Error recording visit: {0}",
                    ["ErrorDeletingVisit"] = "Error deleting visit: {0}",
                    ["FailedAddTrainee"] = "Failed to add trainee.",
                    ["FailedUpdateTrainee"] = "Failed to update trainee.",
                    ["FailedAddMembership"] = "Failed to add membership.",
                    ["FailedUpdateMembership"] = "Failed to update membership."
                    , ["PleaseEnterTraineeId"] = "Please enter trainee ID"
                    , ["ValidationErrorTitle"] = "Validation Error"
                    , ["EnterValidTraineeId"] = "Please enter a valid trainee ID number."
                    , ["EnterTraineeIdAndType"] = "Please enter trainee ID and select membership type."
                    , ["TraineeNotFound"] = "Trainee not found. Please check the trainee ID."
                    , ["FailedAddMembershipGeneric"] = "Failed to add membership."
                    , ["FailedUpdateMembershipGeneric"] = "Failed to update membership."
                    , ["MembershipUpdatedSuccess"] = "Membership updated successfully!"
                    , ["MembershipAddedSuccessOpen"] = "Membership added successfully (Open)."
                    , ["MembershipAddedSuccessLimited"] = "Membership added successfully (12 Sessions)."
                    , ["MembershipAddedSuccess3Months"] = "Membership added successfully (3 Months)."
                    , ["MembershipAddedSuccess1Month"] = "Membership added successfully (1 Month)."
                    , ["MembershipAddedSuccessGeneric"] = "Membership added successfully."
                    , ["EnterAllRequiredFields"] = "Please fill in all required fields."
                    , ["EnterTraineeIdToSearch"] = "Enter trainee ID to search"
                    , ["InvalidTraineeNumber"] = "Invalid trainee number"
                    , ["NoActiveMembership"] = "No active membership for this trainee"
                    , ["SearchResultTitle"] = "Search Result"
                    , ["NotificationTitle"] = "Notice"
                    , ["EnterValidTraineeIdShort"] = "Please enter a valid trainee ID."
                    , ["EnterTraineeNameToSearch"] = "Enter trainee name to search"
                    , ["EnterTraineeName"] = "Enter trainee name"
                    , ["NoTraineesFound"] = "No trainees found."
                }
            };
        }

        public string GetString(string key)
        {
            if (_translations.TryGetValue(CurrentLanguage, out var languageDict) &&
                languageDict.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback to English if Arabic translation not found
            if (CurrentLanguage != "en" && _translations.TryGetValue("en", out var englishDict) &&
                englishDict.TryGetValue(key, out var englishValue))
            {
                return englishValue;
            }

            return key; // Return key as fallback
        }

        public void SetLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode))
            {
                CurrentLanguage = languageCode;
            }
        }
    }
}
