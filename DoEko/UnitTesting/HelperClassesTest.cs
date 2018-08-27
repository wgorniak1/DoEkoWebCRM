using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoEko.Controllers.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HelperClassesTesting
{
    public enum TestEnum
    {
        [Display(Name = "Jeden")]
        one = 1,
        [Display(Name = "Dwa")]
        two,
        [Display(Name = "Trzy")]
        three
    }
    [TestClass]
    public class HelperClassesTest
    {
        [TestMethod]
        public void EnumHelperTest()
        {

            Dictionary<int, string> expectedResult = new Dictionary<int, string>();
            expectedResult.Add(1, "Jeden");
            expectedResult.Add(2, "Dwa");
            expectedResult.Add(3, "Trzy");

            var result = EnumHelper.GetKeyValuePairs(typeof(TestEnum));
            
            CollectionAssert.AreEqual(expectedResult, result, "Kolekcje są różne");
                       
        }
    }
}
