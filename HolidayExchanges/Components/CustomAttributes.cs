using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace HolidayExchanges.Components
{
    // TODO: implementing this (http://www.macaalay.com/2014/02/25/unobtrusive-client-and-server-side-not-equal-to-validation-in-mvc-using-custom-data-annotations/)
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of {1}.";

        public string OtherProperty { get; set; }

        public UnlikeAttribute(string otherProperty)
            : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
                throw new ArgumentNullException("otherProperty");

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name) => string.Format(DefaultErrorMessage, name, OtherProperty);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance
                    .GetType()
                    .GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty
                    .GetValue(validationContext.ObjectInstance, null);

                if (value.Equals(otherPropertyValue))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "unlike",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

            rule.ValidationParameters.Add("otherproperty", OtherProperty);

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidBirthDate : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime _dateBirth = Convert.ToDateTime(value);
            if (_dateBirth < DateTime.Now)
                return ValidationResult.Success;
            else
            {
                return new ValidationResult("The date must be before the current date.");
            }
        }

        public override string FormatErrorMessage(string name) => string.Format("{0} is not a valid date, must be before today's date.", name);

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "validbirthdate",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcludeChar : ValidationAttribute, IClientValidatable
    {
        private readonly string _chars;

        public ExcludeChar(string chars)
            : base("{0} contains invalid character.")
        {
            _chars = chars;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                for (int i = 0; i < _chars.Length; i++)
                {
                    var valueAsString = value.ToString();
                    if (valueAsString.Contains(_chars[i]))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name) => string.Format("{0} has an invalid character.", name);

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "excludechar",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidExchangeDate : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime _dateExchange = Convert.ToDateTime(value);
            if (_dateExchange > DateTime.Now)
                return ValidationResult.Success;
            else
            {
                return new ValidationResult("The date must be after the current date.");
            }
        }

        public override string FormatErrorMessage(string name) => string.Format("{0} is not a valid date, must be after today's date.", name);

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "validexchangedate",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

            yield return rule;
        }
    }
}