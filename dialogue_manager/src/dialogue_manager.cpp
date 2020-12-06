#include "dialogue_manager.hpp"

#include <nlohmann/json.hpp>

#include <algorithm>
#include <fstream>
#include <sstream>

namespace
{
    struct FindDlgByNameFunctor
    {
        std::string name;

        bool operator()(const floofy::DialoguePtr &dlg) const
        {
            return name == dlg->name;
        }
    };

    static constexpr int FILE_VERSION = 2;
    constexpr unsigned E_REACTION_VERSION = 1;

    floofy::eReaction ReactionFromInt(int val, unsigned reactionVersion)
    {
        switch (reactionVersion)
        {
        case 1:
            return static_cast<floofy::eReaction>(val);
        }

        assert(false);
        return floofy::eReaction::None;
    }
} // namespace

namespace floofy
{

    /////////////////////////////////////////////////////////////////////////////
    //DialogueManager

    DialoguePtr DialogueManager::addDialogue(std::string name)
    {
        auto findDialogue = std::find_if(dialogues.begin(), dialogues.end(), FindDlgByNameFunctor{name});

        if (findDialogue == dialogues.end())
        {
            return dialogues.emplace_back(new Dialogue(name));
        }

        return nullptr;
    }

    bool DialogueManager::addDialogue(DialoguePtr dlg)
    {
        if (!dlg)
        {
            return false;
        }

        auto findDialogue = std::find_if(dialogues.begin(), dialogues.end(), FindDlgByNameFunctor{dlg->name});
        if (findDialogue != dialogues.end())
        {
            return false;
        }

        return dialogues.emplace_back(dlg);
    }

    DialoguePtr DialogueManager::dialogue(const std::string &name) const
    {
        auto findDialogue = std::find_if(dialogues.begin(), dialogues.end(), FindDlgByNameFunctor{name});
        if (findDialogue == dialogues.end())
        {
            return nullptr;
        }
        return *findDialogue;
    }

    DialoguePtr DialogueManager::dialogue(size_t index) const
    {
        auto iter = dialogues.begin();
        std::advance(iter, index);
        return *iter;
    }

    DialoguePtr DialogueManager::removeDialogue(const std::string &name)
    {
        auto findDialogue = std::find_if(dialogues.begin(), dialogues.end(), FindDlgByNameFunctor{name});
        if (findDialogue == dialogues.end())
        {
            return nullptr;
        }

        auto dlgPtr = *findDialogue;
        dialogues.erase(findDialogue);
        return dlgPtr;
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
                //File Metadata
                js["version"] = FILE_VERSION;
                js["eReactionVersion"] = E_REACTION_VERSION;

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
                            participantsJs.push_back({{"id", participant->id._id}, {"name", participant->name}});
                        }
                        dialogueJs["participants"] = participantsJs;
                    }

                    //Nodes - DialogueEntries
                    {
                        std::vector<nlohmann::json> entriesJs;
                        entriesJs.reserve(dlg->entries.size());
                        for (size_t i = 0; i < dlg->entries.size(); ++i)
                        {
                            entriesJs.push_back({{"id", dlg->entries[i]->id._id},
                                                 {"entry", dlg->entries[i]->entry},
                                                 {"activeParticipant", dlg->entries[i]->activeParticipant->id._id},
                                                 {"position", nlohmann::json{
                                                                  {"x", dlg->entries[i]->viewPosition.x},
                                                                  {"y", dlg->entries[i]->viewPosition.y}}},
                                                 {"lReaction", static_cast<int>(dlg->entries[i]->lReaction)},
                                                 {"rReaction", static_cast<int>(dlg->entries[i]->rReaction)}});
                        }
                        dialogueJs["entries"] = entriesJs;
                    }

                    //Edges - DialogueChoices
                    {
                        std::vector<nlohmann::json> choicesJs;
                        choicesJs.reserve(dlg->choices.size());
                        for (size_t i = 0; i < dlg->choices.size(); ++i)
                        {
                            choicesJs.push_back({{"id", dlg->choices[i]->id._id},
                                                 {"choice", dlg->choices[i]->choice},
                                                 {"src", dlg->choices[i]->src->id._id},
                                                 {"dst", dlg->choices[i]->dst ? dlg->choices[i]->dst->id._id : -1}});
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
            return readStream(file);
        }

        return nullptr;
    }

    DialogueManagerPtr DialogueManager::readContents(const std::string &contents)
    {
        std::istringstream contentsStream(contents);
        return readStream(contentsStream);
    }

    DialogueManagerPtr DialogueManager::readStream(std::istream &stream)
    {
        if (stream.good())
        {
            auto mgr = std::make_unique<DialogueManager>();

            nlohmann::json json;
            json << stream;

            if (!json.is_object())
            {
                assert(false);
                return nullptr;
            }

            int fileVersion = 0;
            auto findVersion = json.find("version");
            if (findVersion != json.end() && findVersion->is_number())
            {
                fileVersion = *findVersion;
            }

            int eReactionVersion = 0;
            if (fileVersion >= 2)
            {
                auto findEReactionVersion = json.find("eReactionVersion");
                if (findEReactionVersion != json.end() && findEReactionVersion->is_number())
                {
                    eReactionVersion = *findEReactionVersion;
                }
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

                    dlgPtr->addParticipant(part["name"], ID{part["id"]});
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
                    if (!entry.is_object() || !entry.contains("entry") || !entry.contains("id") || !entry.contains("activeParticipant"))
                    {
                        assert(false);
                        return nullptr;
                    }

                    auto part = dlgPtr->participant(ID{entry["activeParticipant"]});
                    if (!part)
                    {
                        assert(false);
                        return nullptr;
                    }

                    auto dlgEntry = dlgPtr->addDialogueEntry(part, entry["entry"], ID{entry["id"]});
                    if (fileVersion >= 1)
                    {
                        auto pos = entry["position"];
                        dlgEntry->viewPosition = {pos["x"], pos["y"]};
                    }

                    if (fileVersion >= 2)
                    {
                        dlgEntry->lReaction = ReactionFromInt(entry["lReaction"], eReactionVersion);
                        dlgEntry->rReaction = ReactionFromInt(entry["rReaction"], eReactionVersion);
                    }
                }

                //DialogueChoices
                auto findDialogueChoice = dlg.find("choices");
                if (findDialogueChoice == dlg.end() || !findDialogueChoice->is_array())
                {
                    assert(false);
                    return nullptr;
                }

                for (const auto &choice : *findDialogueChoice)
                {
                    if (!choice.is_object() || !choice.contains("choice") || !choice.contains("id") || !choice.contains("src") || !choice.contains("dst"))
                    {
                        assert(false);
                        return nullptr;
                    }

                    auto src = dlgPtr->dialogueEntry(ID{choice["src"]});
                    auto dst = dlgPtr->dialogueEntry(ID{choice["dst"]});
                    if (!src)
                    {
                        assert(false);
                        return nullptr;
                    }

                    if (dst)
                    {
                        dlgPtr->addDialogueChoice(src, choice["choice"], dst, ID{choice["id"]});
                    }
                    else
                    {
                        dlgPtr->addDialogueChoice(src, choice["choice"], ID{choice["id"]});
                    }
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

    ParticipantPtr Dialogue::participant(ID id) const
    {
        const auto pred = [id](const ParticipantPtr &other) {
            return id == other->id;
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

    DialogueEntryPtr Dialogue::dialogueEntry(ID id) const
    {
        auto find = std::find_if(entries.begin(), entries.end(), [id](const DialogueEntryPtr &entry) {
            return entry->id == id;
        });

        return find == entries.end() ? nullptr : *find;
    }

    void Dialogue::removeDialogueEntry(size_t index)
    {
        entries.erase(entries.begin() + index);
    }

    DialogueChoicePtr Dialogue::addDialogueChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst)
    {
        return addDialogueChoice(src, choiceStr, dst, _nextDialogueChoiceId);
    }

    DialogueChoicePtr Dialogue::addDialogueChoice(DialogueEntryPtr src, std::string choiceStr)
    {
        return addDialogueChoice(src, choiceStr, _nextDialogueChoiceId);
    }

    size_t Dialogue::numDialogueChoices() const
    {
        return choices.size();
    }

    DialogueChoicePtr Dialogue::choice(size_t index) const
    {
        return choices.at(index);
    }

    DialogueChoicePtr Dialogue::choice(const std::string &name) const
    {
        const auto pred = [&name](const DialogueChoicePtr &other) {
            return name == other->choice;
        };

        auto findDialogueChoice = std::find_if(choices.begin(), choices.end(), pred);
        if (findDialogueChoice == choices.end())
        {
            return {};
        }
        else
        {
            return *findDialogueChoice;
        }
    }

    DialogueChoicePtr Dialogue::choice(ID id) const
    {
        const auto pred = [id](const DialogueChoicePtr &other) {
            return id == other->id;
        };

        auto findDialogueChoice = std::find_if(choices.begin(), choices.end(), pred);
        if (findDialogueChoice == choices.end())
        {
            return {};
        }
        else
        {
            return *findDialogueChoice;
        }
    }

    void Dialogue::removeDialogueChoice(const std::string &name)
    {
        const auto pred = [&name](const DialogueChoicePtr &other) {
            return name == other->choice;
        };

        choices.erase(std::remove_if(choices.begin(), choices.end(), pred));
    }

    ParticipantPtr Dialogue::addParticipant(std::string name, ID id)
    {
        if (id >= _nextParticipantId)
            _nextParticipantId = id + 1;
        return participants.emplace_back(new Participant(id, name));
    }

    DialogueEntryPtr Dialogue::addDialogueEntry(ParticipantPtr activeParticipant, std::string entry, ID id)
    {
        if (id >= _nextEntryId)
            _nextEntryId = id + 1;
        return entries.emplace_back(new DialogueEntry(id, entry, activeParticipant));
    }

    DialogueChoicePtr Dialogue::addDialogueChoice(DialogueEntryPtr src, std::string choiceStr, DialogueEntryPtr dst, ID id)
    {
        if (id >= _nextDialogueChoiceId)
            _nextDialogueChoiceId = id + 1;
        auto choice = choices.emplace_back(new DialogueChoice(id, src, choiceStr, dst));
        src->choices.push_back(choice);

        return choice;
    }

    DialogueChoicePtr Dialogue::addDialogueChoice(DialogueEntryPtr src, std::string choiceStr, ID id)
    {
        if (id >= _nextDialogueChoiceId)
            _nextDialogueChoiceId = id + 1;
        auto choice = choices.emplace_back(new DialogueChoice(id, src, choiceStr));
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
    //DialogueChoice

    /////////////////////////////////////////////////////////////////////////////
} // namespace floofy
