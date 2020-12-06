#pragma once

#include "common/id.hpp"

#include <vector>

namespace floofy
{

  /////////////////////////////////////////////////////////////////////////////
  //Forward Decls
  class ChoiceManager;
  using ChoiceManagerPtr = ChoiceManager * ;  
  class Choice;
  using ChoicePtr = Choice * ;
  class Dependent;
  using DependentPtr = Dependent * ;
  /////////////////////////////////////////////////////////////////////////////

  class ChoiceManager
  {
  public:
    ChoicePtr addChoice(std::string name);
    bool addChoice(ChoicePtr choice);
    ChoicePtr choice(const std::string &name) const;
    ChoicePtr choice(size_t index) const;
    ChoicePtr removeChoice(const std::string &name);
    size_t numChoices() const;

    bool writeToFile(const std::string &filePath) const;

    static ChoiceManagerPtr readFromFile(const std::string &filePath);
    static ChoiceManagerPtr readContents(const std::string &contents);
    static ChoiceManagerPtr readStream(std::istream& stream);

    std::vector<ChoicePtr> choices;
    std::vector<DependentPtr> dependents;
    private:
    ChoicePtr addChoice(ID id, std::string name);
    size_t m_curChoiceId = 0;
  };

  class Choice // A choice that the player makes. Could be a dialogue choice or an action in-game. 
  {
  public:
    Choice(ID id, std::string name);

    ID id;
    std::string name;
  };

} // namespace floofy