namespace Backoffice.Models
{
    public class Modal
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PrimaryButtonText { get; set; }
        public string? SuccessButtonText { get; set; }
        public string? DangerButtonText { get; set; }
        public string SecondaryButtonText { get; set; } = "Close";
        public bool IsStaticBackdrop { get; set; } = false;
    }
}
