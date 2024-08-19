using System.ComponentModel.DataAnnotations;

namespace CommonLib.Attributes
{
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateTime date = Convert.ToDateTime(value);
            return (date >= DateTime.Now) ? ValidationResult.Success : new ValidationResult("Invalid date");
        }
    }
}
