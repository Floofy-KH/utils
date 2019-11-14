#pragma once

#include "dialoguemanager_export.h"

#include <string>
#include <optional>


/////////////////////////////////////////////////////////////////////////////
//Forward Decls 
class DialogueManager;
class DialogueManagerImpl;
class Dialogue;
class DialogueImpl;
class Choice;
class DialogueEntry;
class Participant;
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueManager
class DIALOGUEMANAGER_EXPORT DialogueManager
{
public:    
    DialogueManager();
    ~DialogueManager();
    std::optional<Dialogue> addDialogue(std::string name, std::string greeting);
    std::optional<Dialogue> dialogue(const std::string &name);
    void removeDialogue(const std::string &name);
private:
    DialogueManagerImpl* _impl;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Dialogue
class DIALOGUEMANAGER_EXPORT Dialogue
{
public:
    friend class DialogueManager;
    Dialogue() = default;
    ~Dialogue() = default;

    Participant addParticipant(std::string name);
    size_t numParticipants() const;
    Participant participant(size_t index) const;
    Participant participant(const std::string& name) const;
    void removeParticipant(const std::string& name);

    DialogueEntry addDialogueEntry(Participant activeParticipant, std::string entry);
    size_t numDialogueEntries() const;
    DialogueEntry dialogueEntry(size_t index) const;
    void removeDialogueEntry(size_t index);

    Choice addChoice(DialogueEntry dialogueEntry, std::string choice);
    size_t numChoices() const;
    Choice choice(size_t index) const;
    Choice choice(const std::string& name) const;
    void removeChoice(const std::string& name);

    bool operator ==(const Dialogue& other) const;
    bool operator !=(const Dialogue& other) const;
    Dialogue& operator =(const Dialogue& other);

private:
    Dialogue(DialogueImpl& impl);
    DialogueImpl* _impl = nullptr;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Participant
class DIALOGUEMANAGER_EXPORT Participant
{
public:
    Participant() = default;

    bool operator ==(const Participant& other) const;
    bool operator !=(const Participant& other) const;
private:
    explicit Participant(std::string name);
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueEntry
class DIALOGUEMANAGER_EXPORT DialogueEntry
{
public:
    DialogueEntry() = default;

    bool operator ==(const DialogueEntry& other) const;
    bool operator !=(const DialogueEntry& other) const;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Choice
class DIALOGUEMANAGER_EXPORT Choice
{
public:
    Choice() = default;

    void endsDialogue();
    Dialogue leadsToNewDialogue(std::string dialogue);
    Dialogue leadsToDialogue(Dialogue dialogue);

    bool operator ==(const Choice& other) const;
    bool operator !=(const Choice& other) const;
private:
    explicit Choice(std::string option, DialogueManager& manager);

    DialogueManager& _mgr;
};
/////////////////////////////////////////////////////////////////////////////

