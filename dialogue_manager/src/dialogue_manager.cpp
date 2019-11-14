#include "dialogue_manager/dialogue_manager.hpp"

#include <algorithm>
#include <vector>


/////////////////////////////////////////////////////////////////////////////
//Private Impls

class DialogueImpl
{
public:
    DialogueImpl(std::string name) : name(std::move(name)) {}
    std::string name;
};

class DialogueManagerImpl
{
public:
    std::vector<DialogueImpl> dialogues;
};

class ChoiceImpl
{
public:
    std::string choice;
    DialogueEntry result;
};

class DialogueEntryImpl
{
public:
    std::string entry;
    std::vector<Choice> choices;
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

std::optional<Dialogue> DialogueManager::addDialogue(std::string name, std::string greeting)
{
    return std::optional<Dialogue>(Dialogue(_impl->dialogues.emplace_back(name)));
}

std::optional<Dialogue> DialogueManager::dialogue(const std::string &name)
{
    for(auto& dialogue : _impl->dialogues)
    {
        if(dialogue.name == name)
        {
            return std::optional<Dialogue>(Dialogue(dialogue));
        }
    }

    return std::nullopt;
}

void DialogueManager::removeDialogue(const std::string &name)
{
    _impl->dialogues.erase(std::remove_if(_impl->dialogues.begin(), _impl->dialogues.end(), [&name](const DialogueImpl& di)
    {
        return di.name == name;
    }));
}

/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//Dialogue

Dialogue::Dialogue(DialogueImpl& impl) : _impl(&impl)
{

}

bool Dialogue::operator==(const Dialogue& other) const
{
    return &_impl == &(other._impl);
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
