#pragma once

#include <string>
#include <vector>
#include <unordered_map>

namespace floofy
{
/////////////////////////////////////////////////////////////////////////////
//Forward Decls
class DialogueManager;
using DialogueManagerPtr = DialogueManager *;
class Dialogue;
using DialoguePtr = Dialogue *;
class Choice;
using ChoicePtr = Choice *;
class DialogueEntry;
using DialogueEntryPtr = DialogueEntry *;
class Participant;
using ParticipantPtr = Participant *;
/////////////////////////////////////////////////////////////////////////////

struct ID
{
    explicit ID(size_t id) : _id(id) {}

    ID operator+(size_t rhs) { return ID{_id + rhs}; }
    ID operator-(size_t rhs) { return ID{_id - rhs}; }
    ID operator++(int) { return ID{_id++}; }
    ID operator--(int) { return ID{_id--}; }
    ID &operator++()
    {
        ++_id;
        return *this;
    }
    ID &operator--()
    {
        --_id;
        return *this;
    }

    bool operator>=(const ID &rhs) const { return _id >= rhs._id; }
    bool operator<=(const ID &rhs) const { return _id <= rhs._id; }
    bool operator<(const ID &rhs) const { return _id < rhs._id; }
    bool operator>(const ID &rhs) const { return _id > rhs._id; }
    bool operator==(const ID &rhs) const { return _id == rhs._id; }

    size_t _id;
};

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

    ChoicePtr addChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst);
    ChoicePtr addChoice(DialogueEntryPtr src, std::string choiceStr);
    size_t numChoices() const;
    ChoicePtr choice(size_t index) const;
    ChoicePtr choice(const std::string &name) const;
    ChoicePtr choice(ID id) const;
    void removeChoice(const std::string &name);

    std::string name;
    std::vector<ParticipantPtr> participants;
    std::vector<ChoicePtr> choices;
    std::vector<DialogueEntryPtr> entries;
    ID _nextParticipantId = ID{1};
    ID _nextChoiceId = ID{1};
    ID _nextEntryId = ID{1};

private:
    ParticipantPtr addParticipant(std::string name, ID id);
    DialogueEntryPtr addDialogueEntry(ParticipantPtr activeParticipant, std::string entry, ID id);
    ChoicePtr addChoice(DialogueEntryPtr dialogueEntry, std::string choiceStr, DialogueEntryPtr dst, ID id);
    ChoicePtr addChoice(DialogueEntryPtr dialogueEntry, std::string choiceStr, ID id);
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
    std::vector<ChoicePtr> choices;
    ParticipantPtr activeParticipant;
};
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Choice
class Choice
{
public:
    Choice(ID id, DialogueEntryPtr src, std::string choice, DialogueEntryPtr dst)
        : id(id), src(std::move(src)), choice(std::move(choice)), dst(std::move(dst))
    {
    }
    Choice(ID id, DialogueEntryPtr src, std::string choice)
        : id(id), src(std::move(src)), choice(std::move(choice)), dst(nullptr)
    {
    }

    bool operator==(const Choice &other) const;
    bool operator!=(const Choice &other) const;

    ID id;
    std::string choice;
    DialogueEntryPtr src, dst;
};
/////////////////////////////////////////////////////////////////////////////
} // namespace floofy