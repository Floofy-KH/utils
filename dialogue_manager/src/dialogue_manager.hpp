#pragma once

#include "common/id.hpp"

#include <string>
#include <vector>
#include <unordered_map>

namespace floofy
{
  /////////////////////////////////////////////////////////////////////////////
  //Forward Decls
  class DialogueManager;
  using DialogueManagerPtr = DialogueManager * ;
  class Dialogue;
  using DialoguePtr = Dialogue * ;
  class DialogueChoice;
  using DialogueChoicePtr = DialogueChoice * ;
  class DialogueEntry;
  using DialogueEntryPtr = DialogueEntry * ;
  class Participant;
  using ParticipantPtr = Participant * ;
  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////
  //DialogueManager
  class DialogueManager
  {
  public:
    DialogueManager() = default;
    ~DialogueManager() = default;

    DialoguePtr addDialogue(std::string name);
    bool addDialogue(DialoguePtr dlg);
    DialoguePtr dialogue(const std::string &name) const;
    DialoguePtr dialogue(size_t index) const;
    DialoguePtr removeDialogue(const std::string &name);
    size_t numDialogues() const;

    bool writeToFile(const std::string &filePath) const;

    static DialogueManagerPtr readFromFile(const std::string &filePath);
    static DialogueManagerPtr readContents(const std::string &contents);
    static DialogueManagerPtr readStream(std::istream& stream);

    std::vector<DialoguePtr> dialogues;
  };
  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////
  //Dialogue
  class Dialogue
  {
    friend class DialogueManager;

  public:
    Dialogue(std::string name) : name(std::move(name))
    {
    }

    ParticipantPtr addParticipant(std::string name);
    size_t numParticipants() const;
    ParticipantPtr participant(size_t index) const;
    ParticipantPtr participant(const std::string &name) const;
    ParticipantPtr participant(ID id) const;
    void removeParticipant(const std::string &name);

    DialogueEntryPtr addDialogueEntry(ParticipantPtr activeParticipant, std::string entry);
    size_t numDialogueEntries() const;
    DialogueEntryPtr dialogueEntry(size_t index) const;
    DialogueEntryPtr dialogueEntry(ID id) const;
    void removeDialogueEntry(size_t index);

    DialogueChoicePtr addDialogueChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst);
    DialogueChoicePtr addDialogueChoice(DialogueEntryPtr src, std::string choiceStr);
    size_t numDialogueChoices() const;
    DialogueChoicePtr choice(size_t index) const;
    DialogueChoicePtr choice(const std::string &name) const;
    DialogueChoicePtr choice(ID id) const;
    void removeDialogueChoice(const std::string &name);

    std::string name;
    std::vector<ParticipantPtr> participants;
    std::vector<DialogueChoicePtr> choices;
    std::vector<DialogueEntryPtr> entries;
    ID _nextParticipantId = ID{ 1 };
    ID _nextDialogueChoiceId = ID{ 1 };
    ID _nextEntryId = ID{ 1 };

  private:
    ParticipantPtr addParticipant(std::string name, ID id);
    DialogueEntryPtr addDialogueEntry(ParticipantPtr activeParticipant, std::string entry, ID id);
    DialogueChoicePtr addDialogueChoice(DialogueEntryPtr dialogueEntry, std::string choiceStr, DialogueEntryPtr dst, ID id);
    DialogueChoicePtr addDialogueChoice(DialogueEntryPtr dialogueEntry, std::string choiceStr, ID id);
  };
  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////
  //Participant
  class Participant
  {
  public:
    Participant(ID id, std::string name) : id(id), name(std::move(name)) {}

    ID id;
    std::string name;
  };
  /////////////////////////////////////////////////////////////////////////////


  /////////////////////////////////////////////////////////////////////////////
  //eReaction
  enum class eReaction : int // Should donate money every time this is misspelled without the 'a'. 
  {
    None,
    Happy,
    Sad,
    Angry,
    Surprised
  }; // REMEMBER TO UPDATE DIALOGUEMANAGER.CS

  /////////////////////////////////////////////////////////////////////////////


  /////////////////////////////////////////////////////////////////////////////
  //DialogueEntry
  class DialogueEntry
  {
  public:
    DialogueEntry(ID id, std::string entry, ParticipantPtr participant)
      : id(id), entry(std::move(entry)), activeParticipant(std::move(participant))
    {
    }

    bool operator==(const DialogueEntry &other) const;
    bool operator!=(const DialogueEntry &other) const;

    ID id;
    std::string entry;
    std::vector<DialogueChoicePtr> choices;
    ParticipantPtr activeParticipant;
    struct Vector2
    {
      double x = 0, y = 0;
    } viewPosition;
    eReaction lReaction = eReaction::None;
    eReaction rReaction = eReaction::None;
  };
  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////
  //DialogueChoice
  class DialogueChoice
  {
  public:
    DialogueChoice(ID id, DialogueEntryPtr src, std::string choice, DialogueEntryPtr dst)
      : id(id), src(std::move(src)), choice(std::move(choice)), dst(std::move(dst))
    {
    }
    DialogueChoice(ID id, DialogueEntryPtr src, std::string choice)
      : id(id), src(std::move(src)), choice(std::move(choice)), dst(nullptr)
    {
    }

    bool operator==(const DialogueChoice &other) const;
    bool operator!=(const DialogueChoice &other) const;

    ID id;
    std::string choice;
    DialogueEntryPtr src, dst;
  };
  /////////////////////////////////////////////////////////////////////////////
} // namespace floofy