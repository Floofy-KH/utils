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
    DialoguePtr dialogue(const std::string &name);
    void removeDialogue(const std::string &name);
    
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
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Participant
class Participant
{
public:
    Participant(std::string name) : name(std::move(name)) {}

    std::string name;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueEntry
class DialogueEntry
{
public:
    DialogueEntry(std::string entry, ParticipantPtr participant) 
    : entry(std::move(entry))
    , activeParticipant(std::move(activeParticipant))
    {}

    bool operator ==(const DialogueEntry& other) const;
    bool operator !=(const DialogueEntry& other) const;

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
    Choice(std::string choice, DialogueEntryPtr result) 
    : choice(std::move(choice))
    , result(std::move(result))
    {}

    bool operator ==(const Choice& other) const;
    bool operator !=(const Choice& other) const;

    std::string choice;
    DialogueEntryPtr result;
};
/////////////////////////////////////////////////////////////////////////////

