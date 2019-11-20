#include "dialogue_manager/dialogue_manager_api.h"

extern "C"
{
    HDialogueManager* newDialogueManager()
    {
        return 0;
    }

    dlgmgr_result freeDialogueManager(HDialogueManager* mgr)
    {
        return 0;
    }

    HDialogue* addDialogue(HDialogueManager* mgr, const char* name, dlgmgr_size nameSize, const char* contents, dlgmgr_size contentsSize)
    {
        return 0;
    }

    void removeDialogue(HDialogueManager* mgr, const char* name, dlgmgr_size nameSize)
    {

    }

    HDialogue* dialogueFromName(HDialogueManager* mgr, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    HParticipant* addParticipant(HDialogue* dialogue, const char* name, dlgmgr_size nameSize)
    {
        return 0;
    }

    dlgmgr_size numParticipants(HDialogue* dialogue)
    {
        return 0;
    }

    HParticipant* participantFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        return 0;
    }

    HParticipant* participantFromName(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    void removeParticipant(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {

    }

    HDialogueEntry* addDialogueEntry(HDialogue* dialogue, HParticipant* part, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    dlgmgr_size numDialogueEntries(HDialogue* dialogue)
    {
        return 0;
    }

    HDialogueEntry* dialogueEntryFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        return 0;
    }

    void removeDialogueEntry(HDialogue* dialogue, dlgmgr_size index)
    {

    }

    HChoice* addChoice(HDialogue* dialogue, 
                            HDialogueEntry* dialogueEntry, 
                            const char* name, 
                            dlgmgr_size size, 
                            HDialogueEntry* destDialogueEntry)
    {
        return 0;
    }

    dlgmgr_size numChoices(HDialogue* dialogue)
    {
        return 0;
    }

    HChoice* choiceFromIndex(HDialogue* dialogue, dlgmgr_size index)
    {
        return 0;
    }

    HChoice* choiceFromName(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    void removeChoice(HDialogue* dialogue, const char* name, dlgmgr_size size)
    {

    }
}