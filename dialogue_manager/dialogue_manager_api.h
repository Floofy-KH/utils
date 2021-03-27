#pragma once

#include "common/defines_api.h"

struct HDialogueManager;
struct HDialogue;
struct HParticipant;
struct HDialogueEntry;
struct HDialogueChoice;
struct HGuid;

#if __cplusplus
extern "C"
{
#endif

  EXPORT HDialogueManager *newDialogueManager();
  EXPORT _result_t freeDialogueManager(HDialogueManager *mgr);
  EXPORT _result_t writeDialogues(HDialogueManager *mgr, const char *filePath, _size_t filePathSize);
  EXPORT HDialogueManager *readDialoguesFromFile(const char *filePath, _size_t filePathSize);
  EXPORT HDialogueManager *readDialoguesFromContents(const char *contents, _size_t contentsPathSize);

  EXPORT HDialogue *addNewDialogue(HDialogueManager *mgr, const char *name, _size_t nameSize);
  EXPORT bool addExistingDialogue(HDialogueManager *mgr, HDialogue *dlg);
  EXPORT void removeDialogue(HDialogueManager *mgr, const char *name, _size_t nameSize);
  EXPORT _size_t numDialogues(HDialogueManager *mgr);
  EXPORT HDialogue *dialogueFromName(HDialogueManager *mgr, const char *name, _size_t size);
  EXPORT HDialogue *dialogueFromIndex(HDialogueManager *mgr, _size_t index);
  EXPORT void freeDialogue(HDialogue *dlg);

  EXPORT HParticipant *addParticipant(HDialogue *dialogue, const char *name, _size_t nameSize);
  EXPORT _size_t numParticipants(HDialogue *dialogue);
  EXPORT HParticipant *participantFromIndex(HDialogue *dialogue, _size_t index);
  EXPORT HParticipant *participantFromName(HDialogue *dialogue, const char *name, _size_t size);
  EXPORT void removeParticipant(HDialogue *dialogue, const char *name, _size_t size);

  EXPORT HDialogueEntry *addDialogueEntry(HDialogue *dialogue, HParticipant *part, const char *name, _size_t size);
  EXPORT _size_t numDialogueEntries(HDialogue *dialogue);
  EXPORT HDialogueEntry *dialogueEntryFromIndex(HDialogue *dialogue, _size_t index);
  EXPORT void removeDialogueEntry(HDialogue *dialogue, _size_t index);
  EXPORT void removeDialogueEntryPtr(HDialogue *dialogue, HDialogueEntry *entry);

  EXPORT HDialogueChoice *addDialogueChoiceWithDest(HDialogue *dialogue,
    HDialogueEntry *dialogueEntry,
    const char *name,
    _size_t size,
    HDialogueEntry *destDialogueEntry);

  EXPORT HDialogueChoice *addDialogueChoice(HDialogue *dialogue,
    HDialogueEntry *dialogueEntry,
    const char *name,
    _size_t size);

  EXPORT _size_t numDialogueChoices(HDialogue *dialogue);
  EXPORT HDialogueChoice *dialogueChoiceFromIndex(HDialogue *dialogue, _size_t index);
  EXPORT void removeDialogueChoice(HDialogue *dialogue, HDialogueChoice *choice);

  EXPORT void dialogueName(HDialogue *dialogue, char *name, _size_t bufferSize);
  EXPORT void setDialogueName(HDialogue *dialogue, char *name, _size_t bufferSize);

  EXPORT void participantName(HParticipant *participant, char *name, _size_t bufferSize);
  EXPORT void setParticipantName(HParticipant *participant, char *name, _size_t bufferSize);

  EXPORT void dialogueEntryContent(HDialogueEntry *entry, char *content, _size_t bufferSize);
  EXPORT void setDialogueEntryContent(HDialogueEntry *entry, char *content, _size_t bufferSize);
  EXPORT _size_t dialogueEntryNumDialogueChoices(HDialogueEntry *entry);
  EXPORT HDialogueChoice *dialogueEntryDialogueChoiceFromIndex(HDialogueEntry *entry, _size_t index);
  EXPORT HParticipant *dialogueEntryActiveParticipant(HDialogueEntry *entry);
  EXPORT void setDialogueEntryActiveParticipant(HDialogueEntry *entry, HParticipant *participant);
  EXPORT double dialogueEntryPositionX(HDialogueEntry *entry);
  EXPORT double dialogueEntryPositionY(HDialogueEntry *entry);
  EXPORT void setDialogueEntryPosition(HDialogueEntry *entry, double x, double y);
  EXPORT int dialogueEntryLReaction(HDialogueEntry *entry);
  EXPORT void setDialogueEntryLReaction(HDialogueEntry *entry, int reaction);
  EXPORT int dialogueEntryRReaction(HDialogueEntry *entry);
  EXPORT void setDialogueEntryRReaction(HDialogueEntry *entry, int reaction);

  EXPORT void dialogueChoiceContent(HDialogueChoice *choice, char *content, _size_t bufferSize);
  EXPORT void setDialogueChoiceContent(HDialogueChoice *choice, char *content, _size_t bufferSize);
  EXPORT HDialogueEntry *dialogueChoiceSrcEntry(HDialogueChoice *choice);
  EXPORT HDialogueEntry *dialogueChoiceDstEntry(HDialogueChoice *choice);
  EXPORT void setDialogueChoiceDstEntry(HDialogueChoice *choice, HDialogueEntry *entry);
  EXPORT void assignDialogueChoiceGuid(HDialogueChoice *choice);
  EXPORT bool dialogueChoiceGuidAssigned(HDialogueChoice *choice);
  EXPORT HGuid *dialogueChoiceGuid(HDialogueChoice *choice);

  EXPORT bool guidsAreEqual(HGuid *lhs, HGuid *rhs);
  EXPORT void guidToString(HGuid *guid, char *content, _size_t bufferSize);
  EXPORT HGuid *guidFromString(const char *content, _size_t bufferSize);
  EXPORT bool guidIsValid(HGuid *guid);

#if __cplusplus
}
#endif