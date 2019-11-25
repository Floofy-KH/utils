#include "dialogue_manager/dialogue_manager_api.h"

#include "dialogue_manager.hpp"

#define SUCCESS 1;
#define FAILURE 2;

#define CAST_OPERATIONS(PtrTC, PtrTCpp) \
inline PtrTC* cast(PtrTCpp* ptr) \
{ \
    return reinterpret_cast<PtrTC*>(ptr); \
} \
inline PtrTCpp* cast(PtrTC* ptr) \
{ \
    return reinterpret_cast<PtrTCpp*>(ptr); \
} \

namespace
{
    CAST_OPERATIONS(HDialogueManager, DialogueManager);
    CAST_OPERATIONS(HDialogue, Dialogue);
    CAST_OPERATIONS(HParticipant, Participant);
    CAST_OPERATIONS(HDialogueEntry, DialogueEntry);
    CAST_OPERATIONS(HChoice, Choice);
}

extern "C"
{
    HDialogueManager* newDialogueManager()
    {
        return reinterpret_cast<HDialogueManager*>(new DialogueManager);
    }

    dlgmgr_result freeDialogueManager(HDialogueManager* mgr)
    {
        delete cast(mgr);
        return SUCCESS;
    }

    HDialogue* addDialogue(HDialogueManager* mgr, const char* name, dlgmgr_size nameSize, const char* contents, dlgmgr_size contentsSize)
    {
        auto cppMgr = cast(mgr);
        return cast(cppMgr->addDialogue(std::string(name, nameSize), std::string(contents, contentsSize)));
    }

    void removeDialogue(HDialogueManager* mgr, const char* name, dlgmgr_size nameSize)
    {
        auto cppMgr = cast(mgr);
        cppMgr->removeDialogue(std::string(name, nameSize));
    }

    HDialogue* dialogueFromName(HDialogueManager* mgr, const char* name, dlgmgr_size size)
    {
        auto cppMgr = cast(mgr);
        return cast(cppMgr->dialogue(std::string(name, size)));
    }

    HParticipant* addParticipant(HDialogue* dialogue, const char* name, dlgmgr_size nameSize)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addParticipant(std::string(name, nameSize)));
    }

    dlgmgr_size numParticipants(HDialogue* dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numParticipants();
    }

    HParticipant* participantFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->participant(index));
    }

    HParticipant* participantFromName(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->participant(std::string(name, size)));
    }

    void removeParticipant(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeParticipant(std::string(name, size));
    }

    HDialogueEntry* addDialogueEntry(HDialogue* dialogue, HParticipant* part, const char* name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addDialogueEntry(cast(part), std::string(name, size)));
    }

    dlgmgr_size numDialogueEntries(HDialogue* dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numDialogueEntries();
    }

    HDialogueEntry* dialogueEntryFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->dialogueEntry(index));
    }

    void removeDialogueEntry(HDialogue* dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeDialogueEntry(index);
    }

    HChoice* addChoice(HDialogue* dialogue, 
                            HDialogueEntry* dialogueEntry, 
                            const char* name, 
                            dlgmgr_size size, 
                            HDialogueEntry* destDialogueEntry)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->addChoice(cast(dialogueEntry), std::string(name, size), cast(destDialogueEntry)));
    }

    dlgmgr_size numChoices(HDialogue* dialogue)
    {
        auto cppDlg = cast(dialogue);
        return cppDlg->numChoices();
    }

    HChoice* choiceFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->choice(index));
    }

    HChoice* choiceFromName(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        return cast(cppDlg->choice(std::string(name, size)));
    }

    void removeChoice(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        auto cppDlg = cast(dialogue);
        cppDlg->removeChoice(std::string(name, size));
    }
}