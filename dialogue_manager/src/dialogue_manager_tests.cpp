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

    HDialogueManager *dlgMgr;
    std::string dlgName = "A new dialogue";
};

TEST_F(DialogueManagerTest, AddDialogueAddsNewDialogue)
{
    auto dlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length());
    EXPECT_NE(dlg, nullptr);
}

TEST_F(DialogueManagerTest, AddedDialogueEqualsRetrievedDialogue)
{
    auto newDlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length());
    auto retDlg = dialogueFromName(dlgMgr, dlgName.c_str(), dlgName.length());

    EXPECT_EQ(newDlg, retDlg);
}

TEST_F(DialogueManagerTest, RemovedDialogueNoLongerRetrieved)
{
    auto newDlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length());
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
        dlg = addDialogue(dlgMgr, dlgName.c_str(), dlgName.length());
    }

    void TearDown() override
    {
        freeDialogueManager(dlgMgr);
    }

    HDialogueManager *dlgMgr;
    HDialogue *dlg;
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

TEST(MultipleDialogues, fileIO)
{
    auto dlgMgr = newDialogueManager();
    std::string dlg1Name = "Dialogue 1";
    std::string dlg1Entry1 = "Dialogue 1 Entry 1";
    std::string dlg1Entry2 = "Dialogue 1 Entry 2";
    std::string dlg1Entry3 = "Dialogue 1 Entry 3";
    std::string dlg1Choice1 = "Dialogue 1 Choice 1";
    std::string dlg1Choice2 = "Dialogue 1 Choice 2";
    std::string dlg1Choice3 = "Dialogue 1 Choice 3";
    std::string dlg1Participant1 = "Dialogue 1 Participant 1";
    std::string dlg1Participant2 = "Dialogue 1 Participant 2";
    std::string dlg1Participant3 = "Dialogue 1 Participant 3";
    std::string dlg2Name = "Dialogue 2";
    std::string dlg2Entry1 = "Dialogue 2 Entry 1";
    std::string dlg2Entry2 = "Dialogue 2 Entry 2";
    std::string dlg2Entry3 = "Dialogue 2 Entry 3";
    std::string dlg2Choice1 = "Dialogue 2 Choice 1";
    std::string dlg2Choice2 = "Dialogue 2 Choice 2";
    std::string dlg2Choice3 = "Dialogue 2 Choice 3";
    std::string dlg2Participant1 = "Dialogue 2 Participant 1";
    std::string dlg2Participant2 = "Dialogue 2 Participant 2";
    std::string dlg2Participant3 = "Dialogue 2 Participant 3";

    auto dlg1 = addDialogue(dlgMgr, dlg1Name.c_str(), dlg1Name.length());
    auto dlg2 = addDialogue(dlgMgr, dlg2Name.c_str(), dlg2Name.length());

    auto dlg1Part1 = addParticipant(dlg1, dlg1Participant1.c_str(), dlg1Participant1.length());
    auto dlg1Part2 = addParticipant(dlg1, dlg1Participant2.c_str(), dlg1Participant2.length());
    auto dlg1Part3 = addParticipant(dlg1, dlg1Participant3.c_str(), dlg1Participant3.length());
    auto dlg2Part1 = addParticipant(dlg2, dlg2Participant1.c_str(), dlg2Participant1.length());
    auto dlg2Part2 = addParticipant(dlg2, dlg2Participant2.c_str(), dlg2Participant2.length());
    auto dlg2Part3 = addParticipant(dlg2, dlg2Participant3.c_str(), dlg2Participant3.length());

    auto dlg1Entry1H = addDialogueEntry(dlg1, dlg1Part1, dlg1Entry1.c_str(), dlg1Entry1.length());
    auto dlg1Entry2H = addDialogueEntry(dlg1, dlg1Part2, dlg1Entry2.c_str(), dlg1Entry2.length());
    auto dlg1Entry3H = addDialogueEntry(dlg1, dlg1Part3, dlg1Entry3.c_str(), dlg1Entry3.length());
    auto dlg2Entry1H = addDialogueEntry(dlg2, dlg2Part1, dlg2Entry1.c_str(), dlg2Entry1.length());
    auto dlg2Entry2H = addDialogueEntry(dlg2, dlg2Part2, dlg2Entry2.c_str(), dlg2Entry2.length());
    auto dlg2Entry3H = addDialogueEntry(dlg2, dlg2Part3, dlg2Entry3.c_str(), dlg2Entry3.length());

    auto dlg1Choice1H = addChoice(dlg1, dlg1Entry1H, dlg1Choice1.c_str(), dlg1Choice1.length(), dlg1Entry2H);
    auto dlg1Choice2H = addChoice(dlg1, dlg1Entry2H, dlg1Choice2.c_str(), dlg1Choice2.length(), dlg1Entry3H);
    auto dlg1Choice3H = addChoice(dlg1, dlg1Entry3H, dlg1Choice3.c_str(), dlg1Choice3.length(), dlg1Entry1H);
    auto dlg2Choice1H = addChoice(dlg2, dlg2Entry1H, dlg2Choice1.c_str(), dlg2Choice1.length(), dlg2Entry2H);
    auto dlg2Choice2H = addChoice(dlg2, dlg2Entry2H, dlg2Choice2.c_str(), dlg2Choice2.length(), dlg2Entry3H);
    auto dlg2Choice3H = addChoice(dlg2, dlg2Entry3H, dlg2Choice3.c_str(), dlg2Choice3.length(), dlg2Entry1H);

    std::string dest = "test.json";
    ASSERT_TRUE(writeDialogues(dlgMgr, dest.c_str(), dest.length()));

    auto mgr = readDialogues(dest.c_str(), dest.length());
    ASSERT_NE(mgr, nullptr);

    auto numDlgs = numDialogues(mgr);
    EXPECT_EQ(numDlgs, 2);

    //Dialogue 1
    {
        auto dlg = dialogueFromIndex(mgr, 0);
        ASSERT_NE(dlg, nullptr);

        constexpr size_t bufSize = 1024;

        char strBuf[bufSize];
        dialogueName(dlg, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Name.data());

        //Participants
        auto numParts = numParticipants(dlg);
        EXPECT_EQ(numParts, 3);

        auto part1 = participantFromIndex(dlg, 0);
        participantName(part1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Participant1.c_str());

        auto part2 = participantFromIndex(dlg, 1);
        participantName(part2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Participant2.c_str());

        auto part3 = participantFromIndex(dlg, 2);
        participantName(part3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Participant3.c_str());

        //Entries
        auto numEntries = numDialogueEntries(dlg);
        EXPECT_EQ(numEntries, 3);

        auto entry1 = dialogueEntryFromIndex(dlg, 0);
        dialogueEntryContent(entry1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Entry1.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry1), part1);
        EXPECT_EQ(dialogueEntryNumChoices(entry1), 1);

        auto entry2 = dialogueEntryFromIndex(dlg, 1);
        dialogueEntryContent(entry2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Entry2.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry2), part2);
        EXPECT_EQ(dialogueEntryNumChoices(entry2), 1);

        auto entry3 = dialogueEntryFromIndex(dlg, 2);
        dialogueEntryContent(entry3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Entry3.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry3), part3);
        EXPECT_EQ(dialogueEntryNumChoices(entry3), 1);

        //Choices
        auto nChoices = numChoices(dlg);
        EXPECT_EQ(nChoices, 3);

        auto choice1 = choiceFromIndex(dlg, 0);
        choiceContent(choice1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Choice1.c_str());

        auto choice2 = choiceFromIndex(dlg, 1);
        choiceContent(choice2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Choice2.c_str());

        auto choice3 = choiceFromIndex(dlg, 2);
        choiceContent(choice3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg1Choice3.c_str());

        //Edges
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry1, 0), choice1);
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry2, 0), choice2);
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry3, 0), choice3);

        EXPECT_EQ(choiceSrcEntry(choice1), entry1);
        EXPECT_EQ(choiceSrcEntry(choice2), entry2);
        EXPECT_EQ(choiceSrcEntry(choice3), entry3);

        EXPECT_EQ(choiceDstEntry(choice1), entry2);
        EXPECT_EQ(choiceDstEntry(choice2), entry3);
        EXPECT_EQ(choiceDstEntry(choice3), entry1);
    }

    //Dialogue 2
    {
        auto dlg = dialogueFromIndex(mgr, 1);
        ASSERT_NE(dlg, nullptr);

        constexpr size_t bufSize = 1024;

        char strBuf[bufSize];
        dialogueName(dlg, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Name.data());

        //Participants
        auto numParts = numParticipants(dlg);
        EXPECT_EQ(numParts, 3);

        auto part1 = participantFromIndex(dlg, 0);
        participantName(part1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Participant1.c_str());

        auto part2 = participantFromIndex(dlg, 1);
        participantName(part2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Participant2.c_str());

        auto part3 = participantFromIndex(dlg, 2);
        participantName(part3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Participant3.c_str());

        //Entries
        auto numEntries = numDialogueEntries(dlg);
        EXPECT_EQ(numEntries, 3);

        auto entry1 = dialogueEntryFromIndex(dlg, 0);
        dialogueEntryContent(entry1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Entry1.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry1), part1);
        EXPECT_EQ(dialogueEntryNumChoices(entry1), 1);

        auto entry2 = dialogueEntryFromIndex(dlg, 1);
        dialogueEntryContent(entry2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Entry2.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry2), part2);
        EXPECT_EQ(dialogueEntryNumChoices(entry2), 1);

        auto entry3 = dialogueEntryFromIndex(dlg, 2);
        dialogueEntryContent(entry3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Entry3.c_str());
        EXPECT_EQ(dialogueEntryActiveParticipant(entry3), part3);
        EXPECT_EQ(dialogueEntryNumChoices(entry3), 1);

        //Choices
        auto nChoices = numChoices(dlg);
        EXPECT_EQ(nChoices, 3);

        auto choice1 = choiceFromIndex(dlg, 0);
        choiceContent(choice1, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Choice1.c_str());

        auto choice2 = choiceFromIndex(dlg, 1);
        choiceContent(choice2, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Choice2.c_str());

        auto choice3 = choiceFromIndex(dlg, 2);
        choiceContent(choice3, strBuf, bufSize);
        EXPECT_STREQ(strBuf, dlg2Choice3.c_str());

        //Edges
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry1, 0), choice1);
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry2, 0), choice2);
        EXPECT_EQ(dialogueEntryChoiceFromIndex(entry3, 0), choice3);

        EXPECT_EQ(choiceSrcEntry(choice1), entry1);
        EXPECT_EQ(choiceSrcEntry(choice2), entry2);
        EXPECT_EQ(choiceSrcEntry(choice3), entry3);

        EXPECT_EQ(choiceDstEntry(choice1), entry2);
        EXPECT_EQ(choiceDstEntry(choice2), entry3);
        EXPECT_EQ(choiceDstEntry(choice3), entry1);
    }
}

/////////////////////////////////////////////////////////////////////////////