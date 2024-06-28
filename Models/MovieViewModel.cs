using System.ComponentModel.DataAnnotations;

namespace WebAppMovieTop.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Director is required.")]
        [StringLength(50, ErrorMessage = "Director cannot be longer than 50 characters.")]
        public string Director { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(30, ErrorMessage = "Genre cannot be longer than 30 characters.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1888, 2100, ErrorMessage = "Year must be between 1888 and 2100.")]
        public int Year { get; set; }

        [FileExtensions(Extensions = "jpg,jpeg,png", ErrorMessage = "Please upload a valid image file.")]
        public IFormFile Poster { get; set; }

        public string PosterPath { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }
    }

    public class FileExtensionsAttribute : ValidationAttribute
    {
        public string Extensions { get; set; } = "png,jpg,jpeg";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).Substring(1).ToLower();
                if (!Extensions.Split(',').Contains(extension))
                {
                    return new ValidationResult(ErrorMessage ?? "File extension is not allowed.");
                }
            }
            return ValidationResult.Success;
        }
    }
}