namespace FamilyScheduler.Models
{
    // Error Model: Here by default.
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}