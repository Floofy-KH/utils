#include "dialogue_manager/dialogue_manager.hpp"

#include <algorithm>
#include <vector>

/////////////////////////////////////////////////////////////////////////////
//Private Impls

class ParticipantImpl
{
public:
    ParticipantImpl(std::string name) : name(std::move(name)) {}
    std::string name;
};

class ChoiceImpl
{
public:
    ChoiceImpl(std::string choice, DialogueEntry result) 
    : choice(std::move(choice))
    , result(std::move(result))
    {}

    std::string choice;
    DialogueEntry result;
};

class DialogueEntryImpl
{
public:
    DialogueEntryImpl(std::string entry, Participant participant) 
    : entry(std::move(entry))
    , activeParticipant(std::move(activeParticipant))
    {}

    std::string entry;
    std::vector<Choice> choices;
    Participant activeParticipant;
};

class DialogueImpl
{
public:
    DialogueImpl(std::string name) : name(std::move(name)) {}
    std::string name;
    std::vector<ParticipantImpl> participants;
    std::vector<ChoiceImpl> choices;
    std::vector<DialogueEntryImpl> entries;
};

class DialogueManagerImpl
{
public:
    std::vector<DialogueImpl> dialogues;
};

/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//DialogueManager

DialogueManager::DialogueManager() 
{
    _impl = new DialogueManagerImpl;
}

DialogueManager::~DialogueManager()
{
    delete _impl;
}

Dialogue DialogueManager::addDialogue(std::string name, std::string greeting)
{
    return Dialogue(_impl->dialogues.emplace_back(name));
}

Dialogue DialogueManager::dialogue(const std::string &name)
{
    for(auto& dialogue : _impl->dialogues)
    {
        if(dialogue.name == name)
        {
            return Dialogue(dialogue);
        }
    }

    return {};
}

void DialogueManager::removeDialogue(const std::string &name)
{
    const auto pred = [&name](const DialogueImpl& other) 
    {
        return name == other.name;
    };

    _impl->dialogues.erase(std::remove_if(_impl->dialogues.begin(), _impl->dialogues.end(), pred));
}

/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//Dialogue

Dialogue::Dialogue(DialogueImpl& impl) : _impl(&impl)
{

}

Participant Dialogue::addParticipant(std::string name)
{
    return Participant(_impl->participants.emplace_back(name));
}

size_t Dialogue::numParticipants() const
{
    return _impl->participants.size();
}

Participant Dialogue::participant(size_t index) const
{
    return Participant(_impl->participants.at(index));
}

Participant Dialogue::participant(const std::string& name) const
{
    const auto pred = [&name](const ParticipantImpl& other) 
    {
        return name == other.name;
    };

    auto findParticipant = std::find_if(_impl->participants.cbegin(), _impl->participants.cend(), pred);

    if(findParticipant == _impl->participants.end())
    {
        return {};
    }
    else
    {
        return Participant(const_cast<ParticipantImpl&>(*findParticipant));
    }
}

void Dialogue::removeParticipant(const std::string& name)
{
    const auto pred = [&name](const ParticipantImpl& other) 
    {
        return name == other.name;
    };

    _impl->participants.erase(std::remove_if(_impl->participants.begin(), _impl->participants.end(), pred));
}

DialogueEntry Dialogue::addDialogueEntry(Participant activeParticipant, std::string entry)
{
    return DialogueEntry(_impl->entries.emplace_back(entry, activeParticipant));
}

size_t Dialogue::numDialogueEntries() const
{
    return _impl->entries.size();
}

DialogueEntry Dialogue::dialogueEntry(size_t index) const
{
    return DialogueEntry(_impl->entries.at(index));
}

void Dialogue::removeDialogueEntry(size_t index)
{
    _impl->entries.erase(_impl->entries.begin() + index);
}

Choice Dialogue::addChoice(DialogueEntry src, std::string choiceStr, DialogueEntry dst)
{
    auto& choiceImpl = _impl->choices.emplace_back(choiceStr, dst);
    Choice choice{choiceImpl};
    src._impl->choices.push_back(choice);

    return choice;
}

size_t Dialogue::numChoices() const
{
    return _impl->choices.size();
}

Choice Dialogue::choice(size_t index) const
{
    return Choice(_impl->choices.at(index));
}

Choice Dialogue::choice(const std::string& name) const
{
    const auto pred = [&name](const ChoiceImpl& other) 
    {
        return name == other.choice;
    };

    auto findChoice = std::find_if(_impl->choices.begin(), _impl->choices.end(), pred);
    if(findChoice == _impl->choices.end())
    {
        return {};
    }
    else
    {
        return Choice(*findChoice);
    }    
}

void Dialogue::removeChoice(const std::string& name)
{
    const auto pred = [&name](const ChoiceImpl& other) 
    {
        return name == other.choice;
    };

    _impl->choices.erase(std::remove_if(_impl->choices.begin(), _impl->choices.end(), pred));
}

bool Dialogue::operator ==(const Dialogue& other) const
{
    return _impl == other._impl;
}

bool Dialogue::operator !=(const Dialogue& other) const
{
    return !((*this) == other);
}

Dialogue& Dialogue::operator =(const Dialogue& other)
{
    _impl = other._impl;
    return *this;
}
Dialogue::operator bool() const
{
    return _impl != nullptr;
}

/////////////////////////////////////////////////////////////////////////////



/////////////////////////////////////////////////////////////////////////////
//Participant

/////////////////////////////////////////////////////////////////////////////



/////////////////////////////////////////////////////////////////////////////
//DialogueEntry

/////////////////////////////////////////////////////////////////////////////



/////////////////////////////////////////////////////////////////////////////
//Choice

/////////////////////////////////////////////////////////////////////////////
