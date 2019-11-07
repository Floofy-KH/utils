#include "dialogue_manager/dialogue_manager.hpp"

#include "gtest/gtest.h"

class DialogueManagerTest : public ::testing::Test
{
protected:
    DialogueManager _dlgMgr;
};

TEST_F(DialogueManagerTest, AddDialogueAddsNewDialogue)
{
    DialogueHandle dlg = _dlgMgr.addDialogue("A new dialogue", "Hello there");
    EXPECT_TRUE(dlg);
}

TEST_F(DialogueManagerTest, AddedDialogueEqualsRetrievedDialogue)
{
    DialogueHandle newDlg = _dlgMgr.addDialogue("A new dialogue", "Hello there");
    DialogueHandle retDlg = _dlgMgr.dialogue("A new dialogue");

    EXPECT_EQ(newDlg, retDlg);
}