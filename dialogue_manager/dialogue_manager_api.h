#pragma once

struct HDialogueManager;
struct HDialogue;
struct HParticipant;
struct HDialogueEntry;
struct HChoice;

#define dlgmgr_result unsigned long
#define dlgmgr_size unsigned long

#define EXPORT __declspec(dllexport)

#if __cplusplus
extern "C"
{
#endif

    EXPORT HDialogueManager *newDialogueManager();
    EXPORT dlgmgr_result freeDialogueManager(HDialogueManager *mgr);
    EXPORT dlgmgr_result writeDialogues(HDialogueManager *mgr, const char *filePath, dlgmgr_size filePathSize);
    EXPORT HDialogueManager *readDialogues(const char *filePath, dlgmgr_size filePathSize);

    EXPORT HDialogue *addDialogue(HDialogueManager *mgr, const char *name, dlgmgr_size nameSize);
    EXPORT void removeDialogue(HDialogueManager *mgr, const char *name, dlgmgr_size nameSize);
    EXPORT dlgmgr_size numDialogues(HDialogueManager *mgr);
    EXPORT HDialogue *dialogueFromName(HDialogueManager *mgr, const char *name, dlgmgr_size size);
    EXPORT HDialogue *dialogueFromIndex(HDialogueManager *mgr, dlgmgr_size index);

    EXPORT HParticipant *addParticipant(HDialogue *dialogue, const char *name, dlgmgr_size nameSize);
    EXPORT dlgmgr_size numParticipants(HDialogue *dialogue);
    EXPORT HParticipant *participantFromIndex(HDialogue *dialogue, dlgmgr_size index);
    EXPORT HParticipant *participantFromName(HDialogue *dialogue, const char *name, dlgmgr_size size);
    EXPORT void removeParticipant(HDialogue *dialogue, const char *name, dlgmgr_size size);

    EXPORT HDialogueEntry *addDialogueEntry(HDialogue *dialogue, HParticipant *part, const char *name, dlgmgr_size size);
    EXPORT dlgmgr_size numDialogueEntries(HDialogue *dialogue);
    EXPORT HDialogueEntry *dialogueEntryFromIndex(HDialogue *dialogue, dlgmgr_size index);
    EXPORT void removeDialogueEntry(HDialogue *dialogue, dlgmgr_size index);

    EXPORT HChoice *addChoice(HDialogue *dialogue,
                              HDialogueEntry *dialogueEntry,
                              const char *name,
                              dlgmgr_size size,
                              HDialogueEntry *destDialogueEntry);
    EXPORT dlgmgr_size numChoices(HDialogue *dialogue);
    EXPORT HChoice *choiceFromIndex(HDialogue *dialogue, dlgmgr_size index);
    EXPORT HChoice *choiceFromName(HDialogue *dialogue, const char *name, dlgmgr_size size);
    EXPORT void removeChoice(HDialogue *dialogue, const char *name, dlgmgr_size size);

    EXPORT void getDialogueName(HDialogue *dialogue, char *name, dlgmgr_size bufferSize);

#if __cplusplus
}
#endif