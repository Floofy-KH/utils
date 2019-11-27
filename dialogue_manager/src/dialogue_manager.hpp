#pragma once

#include <string>
#include <vector>


/////////////////////////////////////////////////////////////////////////////
//Forward Decls 
class DialogueManager;
using DialogueManagerPtr = DialogueManager*;
class Dialogue;
using DialoguePtr = Dialogue*;
class Choice;
using ChoicePtr = Choice*;
class DialogueEntry;
using DialogueEntryPtr = DialogueEntry*;
class Participant;
using ParticipantPtr = Participant*;
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueManager
class DialogueManager
{
public:    
    DialogueManager() = default;
    ~DialogueManager() = default;

    DialoguePtr addDialogue(std::string name, std::string greeting);
    DialoguePtr dialogue(const std::string &name) const;
    void removeDialogue(const std::string &name);
    
    bool writeToFile(const std::string& filePath) const;

    static DialogueManagerPtr readFromFile(const std::string& filePath);
    
    std::vector<DialoguePtr> dialogues;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Dialogue
class Dialogue
{
public:
    Dialogue(std::string name) : name(std::move(name)) 
    {}

    ParticipantPtr addParticipant(std::string name);
    size_t numParticipants() const;
    ParticipantPtr participant(size_t index) const;
    ParticipantPtr participant(const std::string& name) const;
    void removeParticipant(const std::string& name);

    DialogueEntryPtr addDialogueEntry(ParticipantPtr activeParticipant, std::string entry);
    size_t numDialogueEntries() const;
    DialogueEntryPtr dialogueEntry(size_t index) const;
    void removeDialogueEntry(size_t index);

    ChoicePtr addChoice(DialogueEntryPtr dialogueEntry, std::string choiceStr, DialogueEntryPtr dst);
    size_t numChoices() const;
    ChoicePtr choice(size_t index) const;
    ChoicePtr choice(const std::string& name) const;
    void removeChoice(const std::string& name);

    std::string name;
    std::vector<ParticipantPtr> participants;
    std::vector<ChoicePtr> choices;
    std::vector<DialogueEntryPtr> entries;
    size_t _nextParticipantId = 1;
    size_t _nextChoiceId = 1;
    size_t _nextEntryId = 1;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Participant
class Participant
{
public:
    Participant(size_t id, std::string name) : id(id), name(std::move(name)) {}

    size_t id;
    std::string name;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueEntry
class DialogueEntry
{
public:
    DialogueEntry(size_t id, std::string entry, ParticipantPtr participant) 
    : id(id)
    , entry(std::move(entry))
    , activeParticipant(std::move(activeParticipant))
    {}

    bool operator ==(const DialogueEntry& other) const;
    bool operator !=(const DialogueEntry& other) const;

    size_t id;
    std::string entry;
    std::vector<ChoicePtr> choices;
    ParticipantPtr activeParticipant;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Choice
class Choice
{
public:        
    Choice(size_t id, std::string choice, DialogueEntryPtr result) 
    : id(id)
    , choice(std::move(choice))
    , result(std::move(result))
    {}

    bool operator ==(const Choice& other) const;
    bool operator !=(const Choice& other) const;

    size_t id;
    std::string choice;
    DialogueEntryPtr result;
};
/////////////////////////////////////////////////////////////////////////////

