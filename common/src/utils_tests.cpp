#include "common/guid.hpp"

#include "gtest/gtest.h"

/////////////////////////////////////////////////////////////////////////////
// Guid Tests

class GuidTest : public ::testing::Test
{
protected:
  void SetUp()
  {
  }

  void TearDown()
  {
  }

  std::string validGuidString = "47D655C2-5A0B-4830-AD70-6E22E9A2A820";
};

TEST_F(GuidTest, DefaultConstructorGeneratesValidGuid)
{
  floofy::Guid newGuid {};
  EXPECT_TRUE(newGuid.isValid());
}

TEST_F(GuidTest, StringConstructorGeneratesValidGuidWithWithString)
{
  floofy::Guid newGuid {validGuidString};
  EXPECT_TRUE(newGuid.isValid());
}

TEST_F(GuidTest, ToStringReturnsIdenticalStringAsConstructedWith)
{
  floofy::Guid newGuid {validGuidString};
  std::string newGuidString = newGuid.toString();
  EXPECT_EQ(newGuidString, validGuidString);
}

TEST_F(GuidTest, CopyConstructorCreatesEqualGuids)
{
  floofy::Guid newGuid {validGuidString};
  floofy::Guid copyGuid {newGuid};
  EXPECT_EQ(newGuid, copyGuid);
  EXPECT_EQ(newGuid.toString(), copyGuid.toString());
}

/////////////////////////////////////////////////////////////////////////////