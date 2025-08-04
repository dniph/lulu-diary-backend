using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Attributes
{
    public class ReactionTypeValidationAttribute : ValidationAttribute
    {
        private static readonly string[] ValidReactionTypes = { "like", "love", "hate" };

        public override bool IsValid(object? value)
        {
            if (value is not string reactionType)
            {
                return false;
            }

            return ValidReactionTypes.Contains(reactionType.ToLower());
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The field {name} must be one of: {string.Join(", ", ValidReactionTypes)}.";
        }
    }
}
