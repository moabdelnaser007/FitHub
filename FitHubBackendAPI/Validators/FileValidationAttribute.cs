using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.Validators
{
    /// <summary>
    /// Validates uploaded IFormFile against allowed extensions, content types and max size.
    /// Usage: [FileValidation(AllowedExtensions = new[] { ".pdf" }, AllowedContentTypes = new[] { "application/pdf" }, MaxFileSizeInBytes = 5 * 1024 * 1024)]
    /// </summary>
    public class FileValidationAttribute : ValidationAttribute
    {
        public string[] AllowedExtensions { get; set; } = new string[0];
        public string[] AllowedContentTypes { get; set; } = new string[0];
        public long MaxFileSizeInBytes { get; set; } = 5 * 1024 * 1024; // default 5 MB

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
            {
                // If it's required, keep [Required] on property. Here we allow null (use Required separately).
                return ValidationResult.Success;
            }

            // Check file size
            if (file.Length > MaxFileSizeInBytes)
            {
                var maxMb = (MaxFileSizeInBytes / (1024 * 1024.0));
                return new ValidationResult($"File is too large. Maximum allowed size is {maxMb:0.##} MB.");
            }

            // Check content type if provided
            if (AllowedContentTypes != null && AllowedContentTypes.Length > 0)
            {
                if (!AllowedContentTypes.Contains(file.ContentType, System.StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult($"Invalid file type. Allowed content types: {string.Join(", ", AllowedContentTypes)}.");
                }
            }

            // Check extension if provided
            if (AllowedExtensions != null && AllowedExtensions.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"Invalid file extension. Allowed extensions: {string.Join(", ", AllowedExtensions)}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
