#pragma once

#include "dialoguemanager_export.h"

#include <memory>
#include <string>
#include <vector>

class Choice;
class DialogueEntry;
class Dialogue;
class Participant;
class DialogueManager;

using ChoiceHandle = Choice*;
using DialogueHandle = Dialogue*;
using DialogueEntryHandle = DialogueEntry*;
using ParticipantHandle = Participant*;

class Choice
{
public:
    Choice(const Choice&) = delete;

    void endsDialogue();
    DialogueHandle leadsToNewDialogue(std::string dialogue);
    DialogueHandle leadsToDialogue(DialogueHandle dialogue);
private:
    explicit Choice(std::string option, DialogueManager& manager);

    DialogueManager& _mgr;
};

class DialogueEntry
{
public:
};

class Dialogue
{
public:
    Dialogue(const Dialogue&) = delete;

    ParticipantHandle addParticipant(std::string name);
    DialogueEntryHandle addDialogueEntry(ParticipantHandle activeParticipant, std::string entry);
    ChoiceHandle addChoice(DialogueEntryHandle dialogueEntry, std::string choice);
    ~Dialogue();
private:
    explicit Dialogue(std::string greeting, DialogueManager& manager);

    DialogueManager& _mgr;
    std::vector<Participant> _participants;
    std::vector<std::string> _dialogueEntries;
    std::vector<std::string> _entries;
};

class Participant
{
public:
    Participant(const Participant&) = delete;
private:
    explicit Participant(std::string name);
};

class DIALOGUEMANAGER_EXPORT DialogueManager
{
public:    
    DialogueHandle addDialogue(std::string name, std::string greeting);
    DialogueHandle dialogue(const std::string &name);
    void removeDialogue(const std::string &name);
private:
};