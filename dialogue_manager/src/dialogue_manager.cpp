#include "dialogue_manager.hpp"

#include <algorithm>

/////////////////////////////////////////////////////////////////////////////
//DialogueManager

DialoguePtr DialogueManager::addDialogue(std::string name, std::string greeting)
{
    return dialogues.emplace_back(new Dialogue(name));
}

DialoguePtr DialogueManager::dialogue(const std::string &name)
{
    for(auto& dialogue : dialogues)
    {
        if(dialogue->name == name)
        {
            return dialogue;
        }
    }

    return {};
}

void DialogueManager::removeDialogue(const std::string &name)
{
    const auto pred = [&name](const DialoguePtr& other) 
    {
        return name == other->name;
    };

    dialogues.erase(std::remove_if(dialogues.begin(), dialogues.end(), pred));
}

/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//Dialogue

ParticipantPtr Dialogue::addParticipant(std::string name)
{
    return participants.emplace_back(new Participant(name));
}

size_t Dialogue::numParticipants() const
{
    return participants.size();
}

ParticipantPtr Dialogue::participant(size_t index) const
{
    return participants.at(index);
}
 
ParticipantPtr Dialogue::participant(const std::string& name) const
{
    const auto pred = [&name](const ParticipantPtr& other) 
    {
        return name == other->name;
    };

    auto findParticipant = std::find_if(participants.cbegin(), participants.cend(), pred);

    if(findParticipant == participants.end())
    {
        return {};
    }
    else
    {
        return *findParticipant;
    }
}

void Dialogue::removeParticipant(const std::string& name)
{
    const auto pred = [&name](const ParticipantPtr& other) 
    {
        return name == other->name;
    };

    participants.erase(std::remove_if(participants.begin(), participants.end(), pred));
}

DialogueEntryPtr Dialogue::addDialogueEntry(ParticipantPtr activeParticipant, std::string entry)
{
    return entries.emplace_back(new DialogueEntry(entry, activeParticipant));
}

size_t Dialogue::numDialogueEntries() const
{
    return entries.size();
}

DialogueEntryPtr Dialogue::dialogueEntry(size_t index) const
{
    return entries.at(index);
}

void Dialogue::removeDialogueEntry(size_t index)
{
    entries.erase(entries.begin() + index);
}

ChoicePtr Dialogue::addChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst)
{
    auto choice = choices.emplace_back(new Choice(choiceStr, dst));
    src->choices.push_back(choice);

    return choice;
}

size_t Dialogue::numChoices() const
{
    return choices.size();
}

ChoicePtr Dialogue::choice(size_t index) const
{
    return choices.at(index);
}

ChoicePtr Dialogue::choice(const std::string& name) const
{
    const auto pred = [&name](const ChoicePtr& other) 
    {
        return name == other->choice;
    };

    auto findChoice = std::find_if(choices.begin(), choices.end(), pred);
    if(findChoice == choices.end())
    {
        return {};
    }
    else
    {
        return *findChoice;
    }    
}

void Dialogue::removeChoice(const std::string& name)
{
    const auto pred = [&name](const ChoicePtr& other) 
    {
        return name == other->choice;
    };

    choices.erase(std::remove_if(choices.begin(), choices.end(), pred));
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
