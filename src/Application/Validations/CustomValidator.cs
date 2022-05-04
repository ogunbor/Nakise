using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public static class CustomValidator
    {
        public static IRuleBuilderOptions<T, IFormFile> NotMoreThan4MB<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
        {
            return ruleBuilder.Must(x => x.Length <= 4000000);
        }

        public static IRuleBuilderOptions<T, IFormFile> CheckFileFormat<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
        {
            var fileFormat = ".png, .jpeg, .jpg, .gif";

            return ruleBuilder.Must(x => fileFormat.Contains(Path.GetExtension(x.FileName).ToLower()));
        }

        public static IRuleBuilder<T, ICollection<FormFieldInputDto>> ValidateFieldIndex<T>(this IRuleBuilder<T, ICollection<FormFieldInputDto>> ruleBuilder, string errorMessage)
        {
            return ruleBuilder.Must(x => x.Select(x => x.Index).Count() == x.Select(x => x.Index).Distinct().Count())
                .WithMessage(errorMessage);
        }
    }
}
