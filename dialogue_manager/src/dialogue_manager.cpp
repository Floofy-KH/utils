#include "dialogue_manager.hpp"

#include <nlohmann/json.hpp>

#include <algorithm>
#include <fstream>

/////////////////////////////////////////////////////////////////////////////
//DialogueManager

DialoguePtr DialogueManager::addDialogue(std::string name)
{
    return dialogues.emplace_back(new Dialogue(name));
}

DialoguePtr DialogueManager::dialogue(const std::string &name) const
{
    for (auto &dialogue : dialogues)
    {
        if (dialogue->name == name)
        {
            return dialogue;
        }
    }

    return {};
}

DialoguePtr DialogueManager::dialogue(size_t index) const
{
    return dialogues.at(index);
}

void DialogueManager::removeDialogue(const std::string &name)
{
    const auto pred = [&name](const DialoguePtr &other) {
        return name == other->name;
    };

    dialogues.erase(std::remove_if(dialogues.begin(), dialogues.end(), pred));
}

size_t DialogueManager::numDialogues() const
{
    return dialogues.size();
}

bool DialogueManager::writeToFile(const std::string &filePath) const
{
    std::ofstream file(filePath);
    if (file.is_open())
    {
        nlohmann::json js;
        {
            //Graphs - Dialogues
            std::vector<nlohmann::json> dialoguesJs;
            for (const auto &dlg : dialogues)
            {
                nlohmann::json dialogueJs;
                dialogueJs["name"] = dlg->name;

                //Graph attributes - Participants
                {
                    std::vector<nlohmann::json> participantsJs;
                    participantsJs.reserve(dlg->participants.size());
                    for (const auto &participant : dlg->participants)
                    {
                        participantsJs.push_back({{"id", participant->id}, {"name", participant->name}});
                    }
                    dialogueJs["participants"] = participantsJs;
                }

                //Nodes - DialogueEntries
                {
                    std::vector<nlohmann::json> entriesJs;
                    entriesJs.reserve(dlg->entries.size());
                    for (size_t i = 0; i < dlg->entries.size(); ++i)
                    {
                        entriesJs.push_back({{"id", dlg->entries[i]->id},
                                             {"entry", dlg->entries[i]->entry},
                                             {"activeParticipant", dlg->entries[i]->activeParticipant->id}});
                    }
                    dialogueJs["entries"] = entriesJs;
                }

                //Edges - Choices
                {
                    std::vector<nlohmann::json> choicesJs;
                    choicesJs.reserve(dlg->choices.size());
                    for (size_t i = 0; i < dlg->choices.size(); ++i)
                    {
                        choicesJs.push_back({{"id", dlg->choices[i]->id},
                                             {"choice", dlg->choices[i]->choice},
                                             {"src", dlg->choices[i]->src->id},
                                             {"dst", dlg->choices[i]->dst->id}});
                    }
                    dialogueJs["choices"] = choicesJs;
                }

                dialoguesJs.emplace_back(std::move(dialogueJs));
            }
            js["dialogues"] = dialoguesJs;

            file << js;

            return true;
        }
    }

    return false;
}

DialogueManagerPtr DialogueManager::readFromFile(const std::string &filePath)
{
    std::ifstream file(filePath);
    if (file.is_open())
    {
        auto mgr = std::make_unique<DialogueManager>();

        nlohmann::json json;
        json << file;

        if (!json.is_object())
        {
            assert(false);
            return nullptr;
        }

        //Dialogues
        auto findDlgs = json.find("dialogues");
        if (findDlgs == json.end() || !findDlgs->is_array())
        {
            assert(false);
            return nullptr;
        }

        for (const auto &dlg : *findDlgs)
        {
            if (!dlg.is_object() || !dlg.contains("name"))
            {
                assert(false);
                return nullptr;
            }

            auto dlgPtr = mgr->addDialogue(dlg["name"]);

            //Participants
            auto findParts = dlg.find("participants");
            if (findParts == dlg.end() || !findParts->is_array())
            {
                assert(false);
                return nullptr;
            }

            for (const auto &part : *findParts)
            {
                if (!part.is_object() || !part.contains("name") || !part.contains("id"))
                {
                    assert(false);
                    return nullptr;
                }

                dlgPtr->addParticipant(part["name"], part["id"]);
            }

            //Entries
            auto findEntries = dlg.find("entries");
            if (findEntries == dlg.end() || !findEntries->is_array())
            {
                assert(false);
                return nullptr;
            }

            for (const auto &entry : *findEntries)
            {
                if (!entry.is_object() || !entry.contains("name") || !entry.contains("id") || !entry.contains("activeParticipant"))
                {
                    assert(false);
                    return nullptr;
                }

                auto part = dlgPtr->participant(entry["activeParticipant"].get<size_t>());
                if (!part)
                {
                    assert(false);
                    return nullptr;
                }

                dlgPtr->addDialogueEntry(part, entry["name"], entry["id"]);
            }

            //Choices
            auto findChoice = dlg.find("choices");
            if (findChoice == dlg.end() || !findChoice->is_array())
            {
                assert(false);
                return nullptr;
            }

            for (const auto &choice : *findChoice)
            {
                if (!choice.is_object() || !choice.contains("name") || !choice.contains("id") || !choice.contains("src") || !choice.contains("dst"))
                {
                    assert(false);
                    return nullptr;
                }

                auto src = dlgPtr->dialogueEntry(choice["src"].get<size_t>());
                auto dst = dlgPtr->dialogueEntry(choice["dst"].get<size_t>());
                if (!src || !dst)
                {
                    assert(false);
                    return nullptr;
                }

                dlgPtr->addChoice(src, choice["choice"], dst, choice["id"]);
            }
        }

        return mgr.release();
    }

    return nullptr;
}

/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//Dialogue

ParticipantPtr Dialogue::addParticipant(std::string name)
{
    return addParticipant(name, _nextParticipantId++);
}

size_t Dialogue::numParticipants() const
{
    return participants.size();
}

ParticipantPtr Dialogue::participant(size_t index) const
{
    return participants.at(index);
}

ParticipantPtr Dialogue::participant(const std::string &name) const
{
    const auto pred = [&name](const ParticipantPtr &other) {
        return name == other->name;
    };

    auto findParticipant = std::find_if(participants.cbegin(), participants.cend(), pred);

    if (findParticipant == participants.end())
    {
        return {};
    }
    else
    {
        return *findParticipant;
    }
}

void Dialogue::removeParticipant(const std::string &name)
{
    const auto pred = [&name](const ParticipantPtr &other) {
        return name == other->name;
    };

    participants.erase(std::remove_if(participants.begin(), participants.end(), pred));
}

DialogueEntryPtr Dialogue::addDialogueEntry(ParticipantPtr activeParticipant, std::string entry)
{
    return addDialogueEntry(activeParticipant, entry, _nextEntryId);
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
    return addChoice(src, choiceStr, dst, _nextChoiceId);
}

size_t Dialogue::numChoices() const
{
    return choices.size();
}

ChoicePtr Dialogue::choice(size_t index) const
{
    return choices.at(index);
}

ChoicePtr Dialogue::choice(const std::string &name) const
{
    const auto pred = [&name](const ChoicePtr &other) {
        return name == other->choice;
    };

    auto findChoice = std::find_if(choices.begin(), choices.end(), pred);
    if (findChoice == choices.end())
    {
        return {};
    }
    else
    {
        return *findChoice;
    }
}

void Dialogue::removeChoice(const std::string &name)
{
    const auto pred = [&name](const ChoicePtr &other) {
        return name == other->choice;
    };

    choices.erase(std::remove_if(choices.begin(), choices.end(), pred));
}

ParticipantPtr Dialogue::addParticipant(std::string name, size_t id)
{
    if (id >= _nextParticipantId)
        _nextParticipantId = id + 1;
    return participants.emplace_back(new Participant(id, name));
}

DialogueEntryPtr Dialogue::addDialogueEntry(ParticipantPtr activeParticipant, std::string entry, size_t id)
{
    if (id >= _nextEntryId)
        _nextEntryId = id + 1;
    return entries.emplace_back(new DialogueEntry(id, entry, activeParticipant));
}

ChoicePtr Dialogue::addChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst, size_t id)
{
    if (id >= _nextChoiceId)
        _nextChoiceId = id + 1;
    auto choice = choices.emplace_back(new Choice(id, src, choiceStr, dst));
    src->choices.push_back(choice);

    return choice;
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
