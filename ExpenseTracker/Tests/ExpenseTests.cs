//namespace ExpenseTracker.Tests
//{
//    using System;
//    using System.Collections.Generic;
//    using System.ComponentModel.DataAnnotations;
//    using NUnit.Framework;
//    using ExpenseTracker;

//    namespace ExpenseTracker.Tests
//    {
//        [TestFixture]
//        public class ExpenseTests
//        {
//            private static bool ValidateModel(object model, out IList<ValidationResult> results)
//            {
//                var context = new ValidationContext(model);
//                results = new List<ValidationResult>();
//                return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
//            }

//            //[Test]
//            //public void Expense_Defaults_Are_Set_Correctly()
//            //{
//            //    var expense = new Expense();

//            //    Assert.IsNotNull(expense);
//            //    Assert.AreNotEqual(Guid.Empty, expense.Id, "Id should be generated");
//            //    Assert.AreEqual(DateOnly.FromDateTime(DateTime.UtcNow), expense.Date, "Date should default to current UTC date");
//            //    Assert.AreEqual("USD", expense.Currency, "Default currency should be USD");
//            //    Assert.IsFalse(expense.IsBusiness, "IsBusiness should default to false");
//            //    Assert.IsNull(expense.UpdatedAt, "UpdatedAt should be null by default");

//            //    var ageSeconds = (DateTime.UtcNow - expense.CreatedAt).TotalSeconds;
//            //    Assert.LessOrEqual(ageSeconds, 5, "CreatedAt should be very recent (within 5 seconds)");
//            //}

//            //[Test]
//            //public void Expense_Validation_Fails_For_Negative_Amount()
//            //{
//            //    var expense = new Expense
//            //    {
//            //        Amount = -10m
//            //    };

//            //    var valid = ValidateModel(expense, out var results);

//            //    Assert.IsFalse(valid, "Validation should fail for negative Amount");
//            //    Assert.IsNotEmpty(results);
//            //    Assert.IsTrue(results.Any(r => r.ErrorMessage != null && r.ErrorMessage.Contains("between")), "Expected Range validation error");
//            //}

//            [Test]
//            public void Expense_Validation_Succeeds_For_Valid_Amount()
//            {
//                var expense = new Expense
//                {
//                    Amount = 123.45m
//                };

//                var valid = ValidateModel(expense, out var results);

//                Assert.IsTrue(valid, "Validation should succeed for non-negative Amount");
//                Assert.IsEmpty(results);
//            }

//            [Test]
//            public void Category_Defaults_Are_Set_Correctly()
//            {
//                var category = new Category();

//                Assert.IsNotNull(category);
//                Assert.AreEqual(0, category.Id, "Default Id for int should be 0");
//                Assert.AreEqual(string.Empty, category.Name, "Default Name should be empty string");
//                Assert.IsNotNull(category.Expenses, "Expenses collection should be initialized");
//                Assert.IsEmpty(category.Expenses, "Expenses collection should be empty by default");
//            }
//        }
//    }
//}
