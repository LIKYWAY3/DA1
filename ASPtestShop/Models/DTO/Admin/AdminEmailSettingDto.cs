namespace ASPtestShop.Models.DTO.Admin
{
    public class AdminEmailSettingDto
    {
        public string SenderName { get; set; } = "FuuFishing Support";
        public string SenderEmail { get; set; } = "";
        public string SenderPassword { get; set; } = "";
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public bool IsConfigured { get; set; } = false;
    }

    public class TestEmailRequestDto
    {
        public string RecipientEmail { get; set; } = "";
    }
}
