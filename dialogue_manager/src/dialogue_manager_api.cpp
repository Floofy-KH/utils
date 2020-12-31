#include "dialogue_manager/dialogue_manager_api.h"

#include "dialogue_manager.hpp"
#include "common/defines.hpp"
#include "common/guid.hpp"

using namespace floofy;

namespace
{
  CAST_OPERATIONS(HDialogueManager, DialogueManager);
  CAST_OPERATIONS(HDialogue, Dialogue);
  CAST_OPERATIONS(HParticipant, Participant);
  CAST_OPERATIONS(HDialogueEntry, DialogueEntry);
  CAST_OPERATIONS(HDialogueChoice, DialogueChoice);
  CAST_OPERATIONS(HGuid, Guid);

  void returnString(const std::string &dst, char *buf, _size_t bufSize)
  {
    if (!buf || bufSize < 1)
      return;
    auto length = dst.copy(buf, bufSize);
    buf[length] = '\0';
  }

  void setString(std::string &str, char *buf, _size_t bufSize)
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

  _result_t freeDialogueManager(HDialogueManager *mgr)
  {
    delete cast(mgr);
    return SUCCESS;
  }

  _result_t writeDialogues(HDialogueManager *mgr, const char *filePath, _size_t filePathSize)
  {
    return cast(mgr)->writeToFile(std::string(filePath, filePathSize));
  }

  HDialogueManager *readDialoguesFromFile(const char *filePath, _size_t filePathSize)
  {
    return cast(DialogueManager::readFromFile(std::string(filePath, filePathSize)));
  }

  HDialogueManager *readDialoguesFromContents(const char *contents, _size_t contentsPathSize)
  {
    return cast(DialogueManager::readContents(std::string(contents, contentsPathSize)));
  }

  HDialogue *addNewDialogue(HDialogueManager *mgr, const char *name, _size_t nameSize)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->addDialogue(std::string(name, nameSize)));
  }

  bool addExistingDialogue(HDialogueManager *mgr, HDialogue *dlg)
  {
    auto cppMgr = cast(mgr);
    return cppMgr->addDialogue(cast(dlg));
  }

  void removeDialogue(HDialogueManager *mgr, const char *name, _size_t nameSize)
  {
    auto cppMgr = cast(mgr);
    cppMgr->removeDialogue(std::string(name, nameSize));
  }

  void freeDialogue(HDialogue *dlg)
  {
    delete cast(dlg);
  }

  _size_t numDialogues(HDialogueManager *mgr)
  {
    auto cppMgr = cast(mgr);
    return cppMgr->numDialogues();
  }

  HDialogue *dialogueFromName(HDialogueManager *mgr, const char *name, _size_t size)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->dialogue(std::string(name, size)));
  }

  HDialogue *dialogueFromIndex(HDialogueManager *mgr, _size_t index)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->dialogue(index));
  }

  HParticipant *addParticipant(HDialogue *dialogue, const char *name, _size_t nameSize)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->addParticipant(std::string(name, nameSize)));
  }

  _size_t numParticipants(HDialogue *dialogue)
  {
    auto cppDlg = cast(dialogue);
    return cppDlg->numParticipants();
  }

  HParticipant *participantFromIndex(HDialogue *dialogue, _size_t index)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->participant(index));
  }

  HParticipant *participantFromName(HDialogue *dialogue, const char *name, _size_t size)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->participant(std::string(name, size)));
  }

  void removeParticipant(HDialogue *dialogue, const char *name, _size_t size)
  {
    auto cppDlg = cast(dialogue);
    cppDlg->removeParticipant(std::string(name, size));
  }

  HDialogueEntry *addDialogueEntry(HDialogue *dialogue, HParticipant *part, const char *name, _size_t size)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->addDialogueEntry(cast(part), std::string(name, size)));
  }

  _size_t numDialogueEntries(HDialogue *dialogue)
  {
    auto cppDlg = cast(dialogue);
    return cppDlg->numDialogueEntries();
  }

  HDialogueEntry *dialogueEntryFromIndex(HDialogue *dialogue, _size_t index)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->dialogueEntry(index));
  }

  void removeDialogueEntry(HDialogue *dialogue, _size_t index)
  {
    auto cppDlg = cast(dialogue);
    cppDlg->removeDialogueEntry(index);
  }

  HDialogueChoice *addDialogueChoiceWithDest(HDialogue *dialogue,
    HDialogueEntry *dialogueEntry,
    const char *name,
    _size_t size,
    HDialogueEntry *destDialogueEntry)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->addDialogueChoice(cast(dialogueEntry), std::string(name, size), cast(destDialogueEntry)));
  }

  HDialogueChoice *addDialogueChoice(HDialogue *dialogue,
    HDialogueEntry *dialogueEntry,
    const char *name,
    _size_t size)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->addDialogueChoice(cast(dialogueEntry), std::string(name, size)));
  }

  _size_t numDialogueChoices(HDialogue *dialogue)
  {
    auto cppDlg = cast(dialogue);
    return cppDlg->numDialogueChoices();
  }

  HDialogueChoice *dialogueChoiceFromIndex(HDialogue *dialogue, _size_t index)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->choice(index));
  }

  HDialogueChoice *dialogueChoiceFromName(HDialogue *dialogue, const char *name, _size_t size)
  {
    auto cppDlg = cast(dialogue);
    return cast(cppDlg->choice(std::string(name, size)));
  }

  void removeDialogueChoice(HDialogue *dialogue, const char *name, _size_t size)
  {
    auto cppDlg = cast(dialogue);
    cppDlg->removeDialogueChoice(std::string(name, size));
  }

  void dialogueName(HDialogue *dialogue, char *name, _size_t bufferSize)
  {
    auto cppDlg = cast(dialogue);
    returnString(cppDlg->name, name, bufferSize);
  }

  void setDialogueName(HDialogue *dialogue, char *name, _size_t bufferSize)
  {
    auto cppDlg = cast(dialogue);
    setString(cppDlg->name, name, bufferSize);
  }

  void participantName(HParticipant *participant, char *name, _size_t bufferSize)
  {
    auto cppPart = cast(participant);
    returnString(cppPart->name, name, bufferSize);
  }

  void setParticipantName(HParticipant *participant, char *name, _size_t bufferSize)
  {
    auto cppPart = cast(participant);
    setString(cppPart->name, name, bufferSize);
  }

  void dialogueEntryContent(HDialogueEntry *entry, char *content, _result_t bufferSize)
  {
    auto cppEntry = cast(entry);
    returnString(cppEntry->entry, content, bufferSize);
  }

  void setDialogueEntryContent(HDialogueEntry *entry, char *content, _result_t bufferSize)
  {
    auto cppEntry = cast(entry);
    setString(cppEntry->entry, content, bufferSize);
  }

  _size_t dialogueEntryNumDialogueChoices(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return cppEntry->choices.size();
  }

  HDialogueChoice *dialogueEntryDialogueChoiceFromIndex(HDialogueEntry *entry, _size_t index)
  {
    auto cppEntry = cast(entry);
    return cast(cppEntry->choices[index]);
  }

  HParticipant *dialogueEntryActiveParticipant(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return cast(cppEntry->activeParticipant);
  }

  void setDialogueEntryActiveParticipant(HDialogueEntry *entry, HParticipant *participant)
  {
    auto cppEntry = cast(entry);
    cppEntry->activeParticipant = cast(participant);
  }

  double dialogueEntryPositionX(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return cppEntry->viewPosition.x;
  }

  double dialogueEntryPositionY(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return cppEntry->viewPosition.y;
  }

  void setDialogueEntryPosition(HDialogueEntry *entry, double x, double y)
  {
    auto cppEntry = cast(entry);
    cppEntry->viewPosition = { x, y };
  }

  int dialogueEntryLReaction(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return static_cast<int>(cppEntry->lReaction);
  }

  void setDialogueEntryLReaction(HDialogueEntry *entry, int reaction)
  {
    auto cppEntry = cast(entry);
    cppEntry->lReaction = static_cast<floofy::eReaction>(reaction);
  }

  int dialogueEntryRReaction(HDialogueEntry *entry)
  {
    auto cppEntry = cast(entry);
    return static_cast<int>(cppEntry->rReaction);
  }

  void setDialogueEntryRReaction(HDialogueEntry *entry, int reaction)
  {
    auto cppEntry = cast(entry);
    cppEntry->rReaction = static_cast<floofy::eReaction>(reaction);
  }

  void dialogueChoiceContent(HDialogueChoice *choice, char *content, _size_t bufferSize)
  {
    auto cppDialogueChoice = cast(choice);
    returnString(cppDialogueChoice->choice, content, bufferSize);
  }

  void setDialogueChoiceContent(HDialogueChoice *choice, char *content, _size_t bufferSize)
  {
    auto cppDialogueChoice = cast(choice);
    setString(cppDialogueChoice->choice, content, bufferSize);
  }

  HDialogueEntry *dialogueChoiceSrcEntry(HDialogueChoice *choice)
  {
    auto cppDialogueChoice = cast(choice);
    return cast(cppDialogueChoice->src);
  }

  HDialogueEntry *dialogueChoiceDstEntry(HDialogueChoice *choice)
  {
    auto cppDialogueChoice = cast(choice);
    return cast(cppDialogueChoice->dst);
  }

  void setDialogueChoiceDstEntry(HDialogueChoice *choice, HDialogueEntry *entry)
  {
    auto cppDialogueChoice = cast(choice);
    cppDialogueChoice->dst = cast(entry);
  }

  void assignDialogueChoiceGuid(HDialogueChoice *choice)
  {
    auto cppDialogueChoice = cast(choice);
    cppDialogueChoice->guidAssigned = true;
  }

  bool dialogueChoiceGuidAssigned(HDialogueChoice *choice)
  {
    auto cppDialogueChoice = cast(choice);
    return cppDialogueChoice->guidAssigned;
  }

  EXPORT HGuid *dialogueChoiceGuid(HDialogueChoice *choice)
  {
    auto cppDialogueChoice = cast(choice);
    return cast(&cppDialogueChoice->guid);
  }

  EXPORT bool guidsAreEqual(HGuid *lhs, HGuid *rhs)
  {
    auto cppGuidLhs = cast(lhs);
    auto cppGuidRhs = cast(rhs);
    return (*cppGuidLhs)==(*cppGuidRhs);
  }

  EXPORT void guidToString(HGuid *guid, char *content, _size_t bufferSize)
  {
    auto cppGuid = cast(guid);
    returnString(cppGuid->toString(), content, bufferSize);
  }

}