using FocusFinderApp.Models;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FocusFinderApp.Tests
{
    public class UserModelTests
    {

        private List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }
        
        [Test]
        public void User_Missing_Fields_Should_Return_Error()
        {
            var user = new User();

            var results = ValidateModel(user);

            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.MemberNames.Contains("Username")));
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.MemberNames.Contains("Email")));
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.MemberNames.Contains("Password")));

        }
    }
}