#include "choice_manager/choice_manager_api.h"

#include "gtest/gtest.h"

/////////////////////////////////////////////////////////////////////////////
// ChoiceManager Tests

class ChoiceManagerTest : public ::testing::Test
{
protected:
  void SetUp()
  {
    choiceMgr = newChoiceManager();
  }

  void TearDown()
  {
    freeChoiceManager(choiceMgr);
  }

  HChoiceManager *choiceMgr;
  std::string choiceName = "A new choice";
};

TEST_F(ChoiceManagerTest, AddNewChoiceReturnsValidPtrOnSuccess)
{
  auto choice = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  EXPECT_NE(choice, nullptr);
}

TEST_F(ChoiceManagerTest, AddNewChoiceReturnsNullPtrOnDuplicateEntry)
{
  auto choice = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  auto choice2 = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  EXPECT_EQ(choice2, nullptr);
}

TEST_F(ChoiceManagerTest, AddExistingChoiceReturnsTrueOnSuccess)
{
  auto choice = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  removeChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  auto res = addExistingChoice(choiceMgr, choice);
  EXPECT_TRUE(res);
}

TEST_F(ChoiceManagerTest, AddExistingChoiceReturnsFalseOnDuplicateEntry)
{
  auto choice = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  auto res = addExistingChoice(choiceMgr, choice);
  EXPECT_FALSE(res);
}

TEST_F(ChoiceManagerTest, AddedChoiceEqualsRetrievedChoice)
{
  auto newDlg = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  auto retDlg = choiceFromName(choiceMgr, choiceName.c_str(), choiceName.length());

  EXPECT_EQ(newDlg, retDlg);
}

TEST_F(ChoiceManagerTest, RemovedChoiceNoLongerRetrieved)
{
  auto newDlg = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  removeChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  auto retDlg = choiceFromName(choiceMgr, choiceName.c_str(), choiceName.length());

  EXPECT_EQ(retDlg, nullptr);
}

/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Choice Tests

class ChoiceTest : public ::testing::Test
{
protected:
  void SetUp() override
  {
    choiceMgr = newChoiceManager();
    choice = addNewChoice(choiceMgr, choiceName.c_str(), choiceName.length());
  }

  void TearDown() override
  {
    freeChoiceManager(choiceMgr);
  }

  HChoiceManager *choiceMgr;
  HChoice *choice;
  std::string choiceName = "A new choice";
  std::string choiceContents = "Hello there";
};



TEST(MultipleChoices, fileIO)
{
  auto choiceMgr = newChoiceManager();
  std::string choice1Name = "Choice 1";
  std::string choice2Name = "Choice 2";

  auto choice1 = addNewChoice(choiceMgr, choice1Name.c_str(), choice1Name.length());
  auto choice2 = addNewChoice(choiceMgr, choice2Name.c_str(), choice2Name.length());

  std::string dest = "test.json";
  ASSERT_TRUE(writeChoices(choiceMgr, dest.c_str(), dest.length()));

  auto mgr = readChoicesFromFile(dest.c_str(), dest.length());
  ASSERT_NE(mgr, nullptr);

  auto numDlgs = numChoices(mgr);
  EXPECT_EQ(numDlgs, 2);

  //Choice 1
  {
    auto choice = choiceFromIndex(mgr, 0);
    ASSERT_NE(choice, nullptr);

    _size_t bufSize{};

    char **strBuf = new char*();
    choiceName(choice, strBuf, &bufSize);
    EXPECT_STREQ(*strBuf, choice1Name.data());    
  }

  //Choice 2
  {
    auto choice = choiceFromIndex(mgr, 1);
    ASSERT_NE(choice, nullptr);

    _size_t bufSize{};

    char **strBuf = new char*();
    choiceName(choice, strBuf, &bufSize);
    EXPECT_STREQ(*strBuf, choice2Name.data());
  }
}

/////////////////////////////////////////////////////////////////////////////