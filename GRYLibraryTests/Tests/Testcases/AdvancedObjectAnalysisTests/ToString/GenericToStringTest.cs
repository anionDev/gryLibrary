﻿using GRYLibrary.Tests.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRYLibrary.Tests.Testcases.AdvancedObjectAnalysisTests.ToString
{
    [TestClass]
    public class GenericToStringTest
    {
        [Ignore]
        [TestMethod]
        public void SimpleDataStructureTestObjectToString()
        {
            // arrange
            SimpleDataStructure1 testObject = SimpleDataStructure1.GetRandom();
            string expectedString = @"{ (ObjectId: 2, Type: GRYLibrary.TestData.TestTypes.SimpleDataStructure.SimpleDataStructure1) 
  Property1: 
  [
    { (ObjectId: 1, Type: GRYLibrary.TestData.TestTypes.SimpleDataStructure.SimpleDataStructure3) 
      Property4: 
      (Type: String, Value: ""Property4_e7df34db-bb6f-4a11-8c6d-66bccafbd041"")
      Property5: 
      [
        { (ObjectId: 5, Type: GRYLibrary.TestData.TestTypes.SimpleDataStructure.SimpleDataStructure2) 
          Guid: 
          (Type: Guid, Value: ""a54f4945-e928-4296-bf9b-e9ae16b35744"")
        },
        { (ObjectId: 5, Type: GRYLibrary.TestData.TestTypes.SimpleDataStructure.SimpleDataStructure2) 
          Guid: 
          (Type: Guid, Value: ""1735ece2-942f-4380-aec4-27aaa4021ed5"")
        }
      ]
    }
  ]
  Property2: 
  { (ObjectId: 5, Type: GRYLibrary.TestData.TestTypes.SimpleDataStructure.SimpleDataStructure2) 
    Guid: 
    (Type: Guid, Value: ""3735ece2-942f-4380-aec4-27aaa4021ed5"")
  }
  Property3: 
  (Type: Int32, Value: ""21"")
}";

            // act
            string actualString = testObject.ToString();

            // assert
            Assert.AreEqual(expectedString, actualString);
        }

    }
}
