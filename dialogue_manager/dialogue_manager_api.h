#pragma once

#define dlgmgr_handle unsigned long
#define dlgmgr_result unsigned long
#define dlgmgr_size unsigned long

#define EXPORT __declspec(dllexport)

#if __cplusplus
extern "C"{
#endif

EXPORT dlgmgr_handle beginNewDialogueManager();
EXPORT dlgmgr_result resumePreviousDialogueManager();
EXPORT dlgmgr_result endDialogueManager( dlgmgr_handle mgr );

EXPORT dlgmgr_handle addDialogue(dlgmgr_handle mgr);
EXPORT void removeDialogue(dlgmgr_handle mgr);
EXPORT dlgmgr_handle dialogue(dlgmgr_handle mgr, const char* name, dlgmgr_size size);

EXPORT dlgmgr_handle addParticipant(dlgmgr_handle dialogue);
EXPORT dlgmgr_size numParticipants(dlgmgr_handle dialogue);
EXPORT dlgmgr_handle participantFromIndex(dlgmgr_handle dialogue, dlgmgr_size index);
EXPORT dlgmgr_handle participantFromName(dlgmgr_handle dialogue, const char* name, dlgmgr_size size);
EXPORT void removeParticipant(dlgmgr_handle dialogue, const char* name, dlgmgr_size size);

EXPORT dlgmgr_handle addDialogueEntry(dlgmgr_handle dialogue);
EXPORT dlgmgr_size numDialogueEntries(dlgmgr_handle dialogue);
EXPORT dlgmgr_handle dialogueEntryFromIndex(dlgmgr_handle dialogue, dlgmgr_size index);
EXPORT void removeDialogueEntry(dlgmgr_handle dialogue, dlgmgr_size index);

EXPORT dlgmgr_handle addChoice(dlgmgr_handle dialogue, 
                    dlgmgr_handle dialogueEntry, 
                    const char* name, 
                    dlgmgr_size size, 
                    dlgmgr_handle destDialogueEntry);
EXPORT dlgmgr_size numChoices();
EXPORT dlgmgr_handle choiceFromIndex(dlgmgr_size index);
EXPORT dlgmgr_handle choiceFromName(const char* name, dlgmgr_size size);
EXPORT void removeChoice(const char* name, dlgmgr_size size);

#if __cplusplus
}
#endif