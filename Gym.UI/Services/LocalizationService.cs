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
                    ["AppTitle"] = "نظام إدارة الصالة الرياضية",
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
                    
                    // Membership Types
                    ["مفتوح"] = "مفتوح",
                    ["محدود"] = "12 حصة",
                    
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
                    ["FailedUpdateMembership"] = "فشل في تحديث العضوية."
                },
                ["en"] = new Dictionary<string, string>
                {
                    // General
                    ["AppTitle"] = "Gym Management System",
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
                    
                    // Membership Types
                    ["مفتوح"] = "Open",
                    ["محدود"] = "12 Sessions",
                    
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
