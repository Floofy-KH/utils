#pragma once

#include "dialoguemanager_export.h"

#include <string>


/////////////////////////////////////////////////////////////////////////////
//Forward Decls 
class DialogueManager;
class DialogueManagerImpl;
class Dialogue;
class DialogueImpl;
class Choice;
class ChoiceImpl;
class DialogueEntry;
class DialogueEntryImpl;
class Participant;
class ParticipantImpl;
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueManager
class DIALOGUEMANAGER_EXPORT DialogueManager
{
public:    
    DialogueManager();
    ~DialogueManager();
    Dialogue addDialogue(std::string name, std::string greeting);
    Dialogue dialogue(const std::string &name);
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
    Dialogue();

    Participant addParticipant(std::string name);
    size_t numParticipants() const;
    Participant participant(size_t index) const;
    Participant participant(const std::string& name) const;
    void removeParticipant(const std::string& name);

    DialogueEntry addDialogueEntry(Participant activeParticipant, std::string entry);
    size_t numDialogueEntries() const;
    DialogueEntry dialogueEntry(size_t index) const;
    void removeDialogueEntry(size_t index);

    Choice addChoice(DialogueEntry dialogueEntry, std::string choiceStr, DialogueEntry dst);
    size_t numChoices() const;
    Choice choice(size_t index) const;
    Choice choice(const std::string& name) const;
    void removeChoice(const std::string& name);

    bool operator ==(const Dialogue& other) const;
    bool operator !=(const Dialogue& other) const;
    Dialogue& operator =(const Dialogue& other);
    operator bool() const;

private:
    Dialogue(DialogueImpl& impl);
    DialogueImpl* _impl = nullptr;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Participant
class DIALOGUEMANAGER_EXPORT Participant
{
    friend class Dialogue;
public:
    Participant();

    bool operator ==(const Participant& other) const;
    bool operator !=(const Participant& other) const;
private:
    explicit Participant(ParticipantImpl& name);

    ParticipantImpl* _impl = nullptr;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueEntry
class DIALOGUEMANAGER_EXPORT DialogueEntry
{
    friend class Dialogue;
public:
    DialogueEntry();

    bool operator ==(const DialogueEntry& other) const;
    bool operator !=(const DialogueEntry& other) const;

private:
    explicit DialogueEntry(DialogueEntryImpl& entry);
    DialogueEntryImpl *_impl = nullptr;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Choice
class DIALOGUEMANAGER_EXPORT Choice
{
    friend class Dialogue;
public:
    Choice() = default;
    
    bool operator ==(const Choice& other) const;
    bool operator !=(const Choice& other) const;
private:
    explicit Choice(ChoiceImpl& impl);

    ChoiceImpl *_impl = nullptr;
};
/////////////////////////////////////////////////////////////////////////////

