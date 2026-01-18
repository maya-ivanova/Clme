using Clme.Data.Entities;
using Clme.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Clme.Tests.Entities
{
    public class ProductTests
    {
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
                Brand = new Brand { Name = "Samsung", Country = "Korea", LogoUrl = "url" },
                Category = new Category { Name = "Wall Mounted" },
                Model = "WindFree",
                Price = 1200.00m,
                Btu = 12000,
                EnergyClass = EnergyClass.A___,
                IsAvailableAtStore = true
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(400)]    // Below Range (Limit is 500)
        [InlineData(11000)]  // Above Range (Limit is 10000)
        public void Product_PriceOutsideRange_ShouldHaveValidationError(decimal invalidPrice)
        {
            // Arrange
            var product = new Product
            {
                Brand = new Brand { Name = "LG", Country = "Korea", LogoUrl = "url" },
                Category = new Category { Name = "Wall Mounted" },
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
        public void Product_ModelExceedsMaxLength_ShouldHaveValidationError()
        {
            // Arrange - Note: We moved MaxLength to Model, Brand is now a class
            var product = new Product
            {
                Brand = new Brand { Name = "Daikin", Country = "Japan", LogoUrl = "url" },
                Category = new Category { Name = "Wall" },
                Model = new string('A', 101), // Limit is 100
                Price = 1000m,
                Btu = 9000
            };

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Model"));
        }

        [Fact]
        public void Product_RequiredFieldsMissing_ShouldHaveValidationErrors()
        {
            // Arrange
            // Brand and Category are navigation properties and required by the compiler (null!)
            // but the Validator checks for [Required] on simple types like Model.
            var product = new Product();

            // Act
            var results = ValidateModel(product);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Model"));
        }
    }
}