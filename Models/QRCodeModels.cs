namespace SpaAdmin.Models
{
    public class QRCodeResponse
    {
        public string QRCodeId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string QRCodeImageBase64 { get; set; } = string.Empty;
        public string QRCodeUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
} 