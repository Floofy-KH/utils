#include "dialogue_manager/dialogue_manager_api.h"

#include "gtest/gtest.h"

/////////////////////////////////////////////////////////////////////////////
// DialogueManager Tests

class DialogueManagerTest : public ::testing::Test
{
protected:
    void SetUp()
    {
        dlgMgr = newDialogueManager();
    }

    void TearDown()
    {
        freeDialogueManager(dlgMgr);
    }

    HDialogueManager* dlgMgr; 
    std::string dlgName = "A new dialogue";
    std::string dlgContents = "Hello there";
};

TEST_F(DialogueManagerTest, AddDialogueAddsNewDialogue)
{
    auto dlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length(), dlgContents.c_str(), dlgContents.length());
    EXPECT_NE(dlg, nullptr);
}

TEST_F(DialogueManagerTest, AddedDialogueEqualsRetrievedDialogue)
{
    auto newDlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length(), dlgContents.c_str(), dlgContents.length());
    auto retDlg = dialogueFromName(dlgMgr, dlgName.c_str(), dlgName.length());

    EXPECT_EQ(newDlg, retDlg);
}

TEST_F(DialogueManagerTest, RemovedDialogueNoLongerRetrieved)
{
    auto newDlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length(), dlgContents.c_str(), dlgContents.length());
    removeDialogue(dlgMgr, dlgName.c_str(), dlgName.length());
    auto retDlg = dialogueFromName(dlgMgr, dlgName.c_str(), dlgName.length());

    EXPECT_EQ(retDlg, nullptr);
}

/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Dialogue Tests

class DialogueTest : public ::testing::Test
{
protected:
    void SetUp() override
    {
        dlgMgr = newDialogueManager();
        dlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length(), dlgContents.c_str(), dlgContents.length());
    }

    void TearDown() override
    {
        freeDialogueManager(dlgMgr);
    }

    HDialogueManager* dlgMgr; 
    HDialogue* dlg;
    std::string dlgName = "A new dialogue";
    std::string dlgContents = "Hello there";
};

class DialogueTestWithParticipants : public DialogueTest
{
protected:
    void SetUp() override
    {
        DialogueTest::SetUp();

        part1 = addParticipant(dlg, part1Name.c_str(), part1Name.length());
        part2 = addParticipant(dlg, part2Name.c_str(), part2Name.length());
        part3 = addParticipant(dlg, part3Name.c_str(), part3Name.length());
    }

    HParticipant *part1, *part2, *part3;
    std::string part1Name = "Participant 1";
    std::string part2Name = "Participant 2";
    std::string part3Name = "Participant 3";
};

TEST_F(DialogueTest, DialogueStartsWithZeroParticipants)
{
    EXPECT_EQ(numParticipants(dlg), 0);
}

TEST_F(DialogueTest, DialogueStartsWithZeroEntries)
{
    EXPECT_EQ(numDialogueEntries(dlg), 0);
}

TEST_F(DialogueTest, DialogueStartsWithZeroChoices)
{
    EXPECT_EQ(numChoices(dlg), 0);
}

TEST_F(DialogueTest, AddParticipantIncrementsNumParticipants)
{
    addParticipant(dlg, "1", 1);
    EXPECT_EQ(numParticipants(dlg), 1);

    addParticipant(dlg, "2", 1);
    addParticipant(dlg, "3", 1);
    EXPECT_EQ(numParticipants(dlg), 3);
}

TEST_F(DialogueTest, ParticipantReturnsTheCorrectParticipant)
{
    auto bob = addParticipant(dlg, "Bob", 3);
    auto sue = addParticipant(dlg, "Sue", 3);

    auto ret = participantFromName(dlg, "Bob", 3);

    EXPECT_EQ(ret, bob);
    EXPECT_NE(ret, sue);
}

TEST_F(DialogueTest, RemoveParticipantDecrementsNumParticipants)
{
    addParticipant(dlg, "1", 1);
    removeParticipant(dlg, "1", 1);
    EXPECT_EQ(numParticipants(dlg), 0);

    addParticipant(dlg, "2", 1);
    addParticipant(dlg, "3", 1);
    removeParticipant(dlg, "2", 1);
    EXPECT_EQ(numParticipants(dlg), 1);

    addParticipant(dlg, "4", 1);
    addParticipant(dlg, "5", 1);
    addParticipant(dlg, "6", 1);
    removeParticipant(dlg, "4", 1);
    EXPECT_EQ(numParticipants(dlg), 3);
}

TEST_F(DialogueTestWithParticipants, AddDialogueEntryIncrementsNumEntries)
{
    addDialogueEntry(dlg, part1, "1", 1);
    EXPECT_EQ(numDialogueEntries(dlg), 1);

    addDialogueEntry(dlg, part2, "2", 1);
    addDialogueEntry(dlg, part3, "3", 1);
    EXPECT_EQ(numDialogueEntries(dlg), 3);
}

TEST_F(DialogueTestWithParticipants, DialogueEntryReturnsTheCorrectEntry)
{
    auto entry1 = addDialogueEntry(dlg, part1, "1", 1);
    auto entry2 = addDialogueEntry(dlg, part2, "2", 1);

    auto ret = dialogueEntryFromIndex(dlg, 0);

    EXPECT_EQ(ret, entry1);
    EXPECT_NE(ret, entry2);
}

TEST_F(DialogueTestWithParticipants, RemoveDialogueEntryDecrementsNumEntries)
{
    addDialogueEntry(dlg, part1, "1", 1);
    removeDialogueEntry(dlg, 0);
    EXPECT_EQ(numDialogueEntries(dlg), 0);

    addDialogueEntry(dlg, part1, "1", 1);
    addDialogueEntry(dlg, part2, "2", 1);
    removeDialogueEntry(dlg, 1);
    EXPECT_EQ(numDialogueEntries(dlg), 1);

    addDialogueEntry(dlg, part1, "1", 1);
    addDialogueEntry(dlg, part3, "3", 1);
    removeDialogueEntry(dlg, 2);
    EXPECT_EQ(numDialogueEntries(dlg), 2);
}

/////////////////////////////////////////////////////////////////////////////