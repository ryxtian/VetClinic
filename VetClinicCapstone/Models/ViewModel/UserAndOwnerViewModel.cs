namespace VetClinicCapstone.Models.ViewModel
{
    public class UserAndOwnerViewModel
    {
        public OwnerTbl OwnerDetail { get; set; }
        public UserTbl UserDetail { get; set; }
        public UserChangePassViewModel UserChangePassViewModels { get; set; }
        public string VerificationCode { get; set; } // New field for verification code
        public bool IsVerified { get; set; } // New field for verification status

    }
    public class UserChangePassViewModel
    {
        public long User_ID { get; set; }
        public string Password { get; set; }
        public string CurrentPassword { get; set; } // Add this if not already present
        public string NewPassword { get; set; }     // Add this if not already present
        public string ConfirmPassword { get; set; } // Add this if not already present
    }
}
