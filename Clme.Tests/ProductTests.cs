using Clme.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Clme.Tests.Entities
{
    public class ProductTests
    {
        // Helper method to simulate validation
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Product_ValidData_ShouldPassValidation()
        {
            // Arrange
            var product = new Product
            {
                Brand = "Samsung",
                Model = "WindFree",
                Price = 1200.00m,
                Btu = 12000,
                Description = "High efficiency AC"
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(400)]    // Below Range
        [InlineData(11000)]  // Above Range
        public void Product_PriceOutsideRange_ShouldHaveValidationError(decimal invalidPrice)
        {
            // Arrange
            var product = new Product
            {
                Brand = "LG",
                Model = "ArtCool",
                Price = invalidPrice,
                Btu = 12000
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void Product_BrandExceedsMaxLength_ShouldHaveValidationError()
        {
            // Arrange
            var product = new Product
            {
                Brand = new string('A', 101), // 101 chars (Limit is 100)
                Model = "Standard",
                Price = 1000m,
                Btu = 9000
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void Product_RequiredFieldsMissing_ShouldHaveValidationErrors()
        {
            // Arrange
            var product = new Product(); // Brand and Model are null/missing

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Brand"));
            Assert.Contains(results, r => r.MemberNames.Contains("Model"));
        }
    }
}