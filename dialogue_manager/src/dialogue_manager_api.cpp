#include "dialogue_manager/dialogue_manager_api.h"

extern "C"
{
    dlgmgr_handle beginNewDialogueManager()
    {
        return 0;
    }

    dlgmgr_result resumePreviousDialogueManager()
    {
        return 0;
    }

    dlgmgr_result endDialogueManager(dlgmgr_handle mgr)
    {
        return 0;
    }

    dlgmgr_handle addDialogue(dlgmgr_handle mgr)
    {
        return 0;
    }

    void removeDialogue(dlgmgr_handle mgr)
    {

    }

    dlgmgr_handle dialogue(dlgmgr_handle mgr, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    dlgmgr_handle addParticipant(dlgmgr_handle dialogue)
    {
        return 0;
    }

    dlgmgr_size numParticipants(dlgmgr_handle dialogue)
    {
        return 0;
    }

    dlgmgr_handle participantFromIndex(dlgmgr_handle dialogue, dlgmgr_size index)
    {
        return 0;
    }

    dlgmgr_handle participantFromName(dlgmgr_handle dialogue, const char* name, dlgmgr_size size)
    {
        return 0;
    }

    void removeParticipant(dlgmgr_handle dialogue, const char* name, dlgmgr_size size)
    {

    }

    dlgmgr_handle addDialogueEntry(dlgmgr_handle dialogue)
    {
        return 0;
    }

    dlgmgr_size numDialogueEntries(dlgmgr_handle dialogue)
    {
        return 0;
    }

    dlgmgr_handle dialogueEntryFromIndex(dlgmgr_handle dialogue, dlgmgr_size index)
    {
        return 0;
    }

    void removeDialogueEntry(dlgmgr_handle dialogue, dlgmgr_size index)
    {

    }

    dlgmgr_handle addChoice(dlgmgr_handle dialogue, 
                            dlgmgr_handle dialogueEntry, 
                            const char* name, 
                            dlgmgr_size size, 
                            dlgmgr_handle destDialogueEntry)
    {
        return 0;
    }

    dlgmgr_size numChoices()
    {
        return 0;
    }

    dlgmgr_handle choiceFromIndex(dlgmgr_size index)
    {
        return 0;
    }

    dlgmgr_handle choiceFromName(const char* name, dlgmgr_size size)
    {
        return 0;
    }

    void removeChoice(const char* name, dlgmgr_size size)
    {

    }
}