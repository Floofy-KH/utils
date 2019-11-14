#include "dialogue_manager/dialogue_manager.hpp"

#include "gtest/gtest.h"

/////////////////////////////////////////////////////////////////////////////
// DialogueManager Tests
/////////////////////////////////////////////////////////////////////////////

class DialogueManagerTest : public ::testing::Test
{
protected:
    DialogueManager _dlgMgr;
};

TEST_F(DialogueManagerTest, AddDialogueAddsNewDialogue)
{
    auto dlg = _dlgMgr.addDialogue("A new dialogue", "Hello there");
    EXPECT_TRUE(dlg);
}

TEST_F(DialogueManagerTest, AddedDialogueEqualsRetrievedDialogue)
{
    auto newDlg = _dlgMgr.addDialogue("A new dialogue", "Hello there");
    auto retDlg = _dlgMgr.dialogue("A new dialogue");

    EXPECT_EQ(newDlg, retDlg);
}

TEST_F(DialogueManagerTest, RemovedDialogueNoLongerRetrieved)
{
    _dlgMgr.addDialogue("A new dialogue", "Hello there");
    _dlgMgr.removeDialogue("A new dialogue");
    auto retDlg = _dlgMgr.dialogue("A new dialogue");

    EXPECT_FALSE(retDlg);
}

/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Dialogue Tests
/////////////////////////////////////////////////////////////////////////////

class DialogueTest : public ::testing::Test
{
protected:
    void SetUp() override
    {
        _dlg = _dlgMgr.addDialogue(_dlgName, _dlgText).value();
    }

    DialogueManager _dlgMgr;
    Dialogue _dlg;
    std::string _dlgName = "A new dialogue";
    std::string _dlgText = "Hello there";
};

class DialogueTestWithParticipants : public DialogueTest
{
protected:
    void SetUp() override
    {
        _part1 = _dlg.addParticipant("Participant 1");
        _part2 = _dlg.addParticipant("Participant 2");
        _part3 = _dlg.addParticipant("Participant 3");
    }

    Participant _part1, _part2, _part3;
};

TEST_F(DialogueTest, DialogueStartsWithZeroParticipants)
{
    EXPECT_EQ(_dlg.numParticipants(), 0);
}

TEST_F(DialogueTest, DialogueStartsWithZeroEntries)
{
    EXPECT_EQ(_dlg.numDialogueEntries(), 0);
}

TEST_F(DialogueTest, DialogueStartsWithZeroChoices)
{
    EXPECT_EQ(_dlg.numChoices(), 0);
}

TEST_F(DialogueTest, AddParticipantIncrementsNumParticipants)
{
    _dlg.addParticipant("1");
    EXPECT_EQ(_dlg.numParticipants(), 1);

    _dlg.addParticipant("2");
    _dlg.addParticipant("3");
    EXPECT_EQ(_dlg.numParticipants(), 3);
}

TEST_F(DialogueTest, ParticipantReturnsTheCorrectParticipant)
{
    auto bob = _dlg.addParticipant("Bob");
    auto sue = _dlg.addParticipant("Sue");

    auto ret = _dlg.participant("bob");

    EXPECT_EQ(ret, bob);
    EXPECT_NE(ret, sue);
}

TEST_F(DialogueTest, RemoveParticipantDecrementsNumParticipants)
{
    _dlg.addParticipant("1");
    _dlg.removeParticipant("1");
    EXPECT_EQ(_dlg.numParticipants(), 0);

    _dlg.addParticipant("2");
    _dlg.addParticipant("3");
    _dlg.removeParticipant("2");
    EXPECT_EQ(_dlg.numParticipants(), 1);

    _dlg.addParticipant("4");
    _dlg.addParticipant("5");
    _dlg.addParticipant("6");
    _dlg.removeParticipant("4");
    EXPECT_EQ(_dlg.numParticipants(), 3);
}

TEST_F(DialogueTestWithParticipants, AddDialogueEntryIncrementsNumEntries)
{
    _dlg.addDialogueEntry(_part1, "1");
    EXPECT_EQ(_dlg.numDialogueEntries(), 1);

    _dlg.addDialogueEntry(_part2, "2");
    _dlg.addDialogueEntry(_part3, "3");
    EXPECT_EQ(_dlg.numDialogueEntries(), 3);
}

TEST_F(DialogueTestWithParticipants, DialogueEntryReturnsTheCorrectEntry)
{
    auto entry1 = _dlg.addDialogueEntry(_part1, "1");
    auto entry2 = _dlg.addDialogueEntry(_part2, "2");

    auto ret = _dlg.dialogueEntry(1);

    EXPECT_EQ(ret, entry1);
    EXPECT_NE(ret, entry2);
}

TEST_F(DialogueTestWithParticipants, RemoveDialogueEntryDecrementsNumEntries)
{
    _dlg.addDialogueEntry(_part1, "1");
    _dlg.removeDialogueEntry(1);
    EXPECT_EQ(_dlg.numDialogueEntries(), 0);

    _dlg.addDialogueEntry(_part1, "1");
    _dlg.addDialogueEntry(_part2, "2");
    _dlg.removeDialogueEntry(2);
    EXPECT_EQ(_dlg.numDialogueEntries(), 1);

    _dlg.addDialogueEntry(_part1, "1");
    _dlg.addDialogueEntry(_part3, "3");
    _dlg.removeDialogueEntry(3);
    EXPECT_EQ(_dlg.numDialogueEntries(), 2);
}

/////////////////////////////////////////////////////////////////////////////