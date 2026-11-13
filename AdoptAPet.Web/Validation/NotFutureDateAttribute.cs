
namespace AdoptAPet.Web.Validation
{
    using System.ComponentModel.DataAnnotations;

    public sealed class NotFutureDateAttribute : ValidationAttribute
    {
        public NotFutureDateAttribute()
        {
            ErrorMessage = "Date of birth cannot be in the future.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is not DateTime date)
            {
                return false;
            }

            return date.Date <= DateTime.UtcNow.Date;
        }
    }
}
