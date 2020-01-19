#include "dialogue_manager/dialogue_manager_api.h"

#include "dialogue_manager.hpp"

#define SUCCESS 1;
#define FAILURE 2;

#define CAST_OPERATIONS(PtrTC, PtrTCpp)          \
    inline PtrTC *cast(PtrTCpp *ptr)             \
    {                                            \
        return reinterpret_cast<PtrTC *>(ptr);   \
    }                                            \
    inline PtrTCpp *cast(PtrTC *ptr)             \
    {                                            \
        return reinterpret_cast<PtrTCpp *>(ptr); \
    }

using namespace floofy;

namespace
{
CAST_OPERATIONS(HDialogueManager, DialogueManager);
CAST_OPERATIONS(HDialogue, Dialogue);
CAST_OPERATIONS(HParticipant, Participant);
CAST_OPERATIONS(HDialogueEntry, DialogueEntry);
CAST_OPERATIONS(HChoice, Choice);

void returnString(const std::string &dst, char *buf, dlgmgr_size bufSize)
{
    if (!buf || bufSize < 1)
        return;
    auto length = dst.copy(buf, bufSize);
    buf[length] = '\0';
}

void setString(std::string &str, char *buf, dlgmgr_size bufSize)
{
    str.assign(buf, bufSize);
}
} // namespace

extern "C"
{
    HDialogueManager *newDialogueManager()
    {
        return reinterpret_cast<HDialogueManager *>(new DialogueManager);
    }

    dlgmgr_result freeDialogueManager(HDialogueManager *mgr)
    {
        delete cast(mgr);
        return SUCCESS;
    }

    dlgmgr_result writeDialogues(HDialogueManager *mgr, const char *filePath, dlgmgr_size filePathSize)
    {
        return cast(mgr)->writeToFile(std::string(filePath, filePathSize));
    }

    HDialogueManager *readDialogues(const char *filePath, dlgmgr_size filePathSize)
    {
        return cast(DialogueManager::readFromFile(std::string(filePath, filePathSize)));
    }

    HDialogue *addDialogue(HDialogueManager *mgr, const char *name, dlgmgr_size nameSize)
    {
        auto cppMgr = cast(mgr);
        return cast(cppMgr->addDialogue(std::string(name, nameSize)));
    }

    void removeDialogue(HDialogueManager *mgr, const char *name, dlgmgr_size nameSize)
    {
        auto cppMgr = cast(mgr);
        cppMgr->removeDialogue(std::string(name, nameSize));
    }

    dlgmgr_size numDialogues(HDialogueManager *mgr)
    {
        auto cppMgr = cast(mgr);
        return cppMgr->numDialogues();
    }

    HDialogue *dialogueFromName(HDialogueManager *mgr, const char *name, dlgmgr_size size)
    {
        auto cppMgr = cast(mgr);
        return cast(cppMgr->dialogue(std::string(name, size)));
    }

    HDialogue *dialogueFromIndex(HDialogueManager *mgr, dlgmgr_size index)
    {
        auto cppMgr = cast(mgr);
        return cast(cppMgr->dialogue(index));
    }

    HParticipant *addParticipant(HDialogue *dialogue, const char *name, dlgmgr_size nameSize)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addParticipant(std::string(name, nameSize)));
    }

    dlgmgr_size numParticipants(HDialogue *dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numParticipants();
    }

    HParticipant *participantFromIndex(HDialogue *dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->participant(index));
    }

    HParticipant *participantFromName(HDialogue *dialogue, const char *name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->participant(std::string(name, size)));
    }

    void removeParticipant(HDialogue *dialogue, const char *name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeParticipant(std::string(name, size));
    }

    HDialogueEntry *addDialogueEntry(HDialogue *dialogue, HParticipant *part, const char *name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addDialogueEntry(cast(part), std::string(name, size)));
    }

    dlgmgr_size numDialogueEntries(HDialogue *dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numDialogueEntries();
    }

    HDialogueEntry *dialogueEntryFromIndex(HDialogue *dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->dialogueEntry(index));
    }

    void removeDialogueEntry(HDialogue *dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeDialogueEntry(index);
    }

    HChoice *addChoiceWithDest(HDialogue *dialogue,
                               HDialogueEntry *dialogueEntry,
                               const char *name,
                               dlgmgr_size size,
                               HDialogueEntry *destDialogueEntry)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addChoice(cast(dialogueEntry), std::string(name, size), cast(destDialogueEntry)));
    }

    HChoice *addChoice(HDialogue *dialogue,
                       HDialogueEntry *dialogueEntry,
                       const char *name,
                       dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addChoice(cast(dialogueEntry), std::string(name, size)));
    }

    dlgmgr_size numChoices(HDialogue *dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numChoices();
    }

    HChoice *choiceFromIndex(HDialogue *dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->choice(index));
    }

    HChoice *choiceFromName(HDialogue *dialogue, const char *name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->choice(std::string(name, size)));
    }

    void removeChoice(HDialogue *dialogue, const char *name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeChoice(std::string(name, size));
    }

    void dialogueName(HDialogue *dialogue, char *name, dlgmgr_size bufferSize)
    {
        auto cppDlg = cast(dialogue);
        returnString(cppDlg->name, name, bufferSize);
    }

    void setDialogueName(HDialogue *dialogue, char *name, dlgmgr_size bufferSize)
    {
        auto cppDlg = cast(dialogue);
        setString(cppDlg->name, name, bufferSize);
    }

    void participantName(HParticipant *participant, char *name, dlgmgr_size bufferSize)
    {
        auto cppPart = cast(participant);
        returnString(cppPart->name, name, bufferSize);
    }

    void setParticipantName(HParticipant *participant, char *name, dlgmgr_size bufferSize)
    {
        auto cppPart = cast(participant);
        setString(cppPart->name, name, bufferSize);
    }

    void dialogueEntryContent(HDialogueEntry *entry, char *content, dlgmgr_result bufferSize)
    {
        auto cppEntry = cast(entry);
        returnString(cppEntry->entry, content, bufferSize);
    }

    void setDialogueEntryContent(HDialogueEntry *entry, char *content, dlgmgr_result bufferSize)
    {
        auto cppEntry = cast(entry);
        setString(cppEntry->entry, content, bufferSize);
    }

    dlgmgr_size dialogueEntryNumChoices(HDialogueEntry *entry)
    {
        auto cppEntry = cast(entry);
        return cppEntry->choices.size();
    }

    HChoice *dialogueEntryChoiceFromIndex(HDialogueEntry *entry, dlgmgr_size index)
    {
        auto cppEntry = cast(entry);
        return cast(cppEntry->choices[index]);
    }

    HParticipant *dialogueEntryActiveParticipant(HDialogueEntry *entry)
    {
        auto cppEntry = cast(entry);
        return cast(cppEntry->activeParticipant);
    }

    void choiceContent(HChoice *choice, char *content, dlgmgr_size bufferSize)
    {
        auto cppChoice = cast(choice);
        returnString(cppChoice->choice, content, bufferSize);
    }

    void setChoiceContent(HChoice *choice, char *content, dlgmgr_size bufferSize)
    {
        auto cppChoice = cast(choice);
        setString(cppChoice->choice, content, bufferSize);
    }

    HDialogueEntry *choiceSrcEntry(HChoice *choice)
    {
        auto cppChoice = cast(choice);
        return cast(cppChoice->src);
    }

    HDialogueEntry *choiceDstEntry(HChoice *choice)
    {
        auto cppChoice = cast(choice);
        return cast(cppChoice->dst);
    }

    void setChoiceDstEntry(HChoice *choice, HDialogueEntry *entry)
    {
        auto cppChoice = cast(choice);
        cppChoice->dst = cast(entry);
    }
}