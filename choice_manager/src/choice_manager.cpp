#include "choice_manager.hpp"

#include <nlohmann/json.hpp>

#include <algorithm>
#include <fstream>
#include <sstream>

namespace
{
  static constexpr int FILE_VERSION = 1;

  struct FindChoiceByNameFunctor
  {
    std::string name;

    bool operator()(const floofy::ChoicePtr &choice) const
    {
      return name == choice->name;
    }
  };
} // namespace

namespace floofy
{
  /////////////////////////////////////////////////////////////////////////////
  //ChoiceManager
  ChoicePtr ChoiceManager::addChoice(std::string name)
  {
    auto findChoice = std::find_if(choices.begin(), choices.end(), FindChoiceByNameFunctor{name});

    if (findChoice == choices.end())
    {
      return choices.emplace_back(new Choice(ID{m_curChoiceId++}, name));
    }

    return nullptr;
  }

  ChoicePtr ChoiceManager::addChoice(ID id, std::string name)
  {
    auto choice = new Choice(id, name);
    choices.push_back(choice);

    return choice;
  }

  bool ChoiceManager::addChoice(ChoicePtr choice)
  {
    if (!choice)
    {
      return false;
    }

    auto findChoice = std::find_if(choices.begin(), choices.end(), FindChoiceByNameFunctor{choice->name});
    if (findChoice != choices.end())
    {
      return false;
    }

    return choices.emplace_back(choice);
  }

  ChoicePtr ChoiceManager::choice(const std::string &name) const
  {
    auto findChoice = std::find_if(choices.begin(), choices.end(), FindChoiceByNameFunctor{name});
    if (findChoice == choices.end())
    {
      return nullptr;
    }
    return *findChoice;
  }

  ChoicePtr ChoiceManager::choice(size_t index) const
  {
    auto iter = choices.begin();
    std::advance(iter, index);
    return *iter;
  }

  ChoicePtr ChoiceManager::removeChoice(const std::string &name)
  {
    auto findChoice = std::find_if(choices.begin(), choices.end(), FindChoiceByNameFunctor{name});
    if (findChoice == choices.end())
    {
      return nullptr;
    }

    auto depPtr = *findChoice;
    choices.erase(findChoice);
    return depPtr;
  }

  size_t ChoiceManager::numChoices() const
  {
    return choices.size();
  }

  bool ChoiceManager::writeToFile(const std::string &filePath) const
  {
    std::ofstream file(filePath);
    if (file.is_open())
    {
      nlohmann::json js;
      {
        //File Metadata
        js["version"] = FILE_VERSION;

        //Choices
        std::vector<nlohmann::json> choicesJs;
        for (const auto &choice : choices)
        {
          nlohmann::json choiceJs;
          choiceJs["id"] = choice->id._id;
          choiceJs["name"] = choice->name;
          choicesJs.emplace_back(std::move(choiceJs));
        }
        js["choices"] = choicesJs;

        file << js;

        return true;
      }
    }

    return false;
  }

  ChoiceManagerPtr ChoiceManager::readFromFile(const std::string &filePath)
  {
    std::ifstream file(filePath);
    if (file.is_open())
    {
      return readStream(file);
    }

    return nullptr;
  }

  ChoiceManagerPtr ChoiceManager::readContents(const std::string &contents)
  {
    std::istringstream contentsStream(contents);
    return readStream(contentsStream);
  }

  ChoiceManagerPtr ChoiceManager::readStream(std::istream &stream)
  {
    if (stream.good())
    {
      auto mgr = std::make_unique<ChoiceManager>();
      
      if(stream.gcount() <= 0)
        return mgr.release();

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

      //Choices
      auto findChoices = json.find("choices");
      if (findChoices == json.end() || !findChoices->is_array())
      {
        assert(false);
        return nullptr;
      }

      for (const auto &choice : *findChoices)
      {
        if (!choice.is_object() || !choice.contains("name") || !choice.contains("id"))
        {
          assert(false);
          return nullptr;
        }

        mgr->addChoice(ID{choice["id"]}, choice["name"]);
      }

      return mgr.release();
    }

    return nullptr;
  }
  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////
  //Choice
  Choice::Choice(ID id, std::string name)
      : id(id), name(name)
  {
  }
  /////////////////////////////////////////////////////////////////////////////
} // namespace floofy