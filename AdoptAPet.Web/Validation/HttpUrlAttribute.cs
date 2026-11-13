
namespace AdoptAPet.Web.Validation
{
    using System.ComponentModel.DataAnnotations;

    public sealed class HttpUrlAttribute : ValidationAttribute
    {
        public HttpUrlAttribute()
        {
            ErrorMessage = "Image URL must be a valid HTTP or HTTPS URL.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is not string url)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return true;
            }

            return Uri.TryCreate(url.Trim(), UriKind.Absolute,out Uri? uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
