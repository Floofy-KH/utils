#include "choice_manager/choice_manager_api.h"

#include "choice_manager.hpp"
#include "common/defines.hpp"

using namespace floofy;

namespace
{
  CAST_OPERATIONS(HChoiceManager, ChoiceManager);
  CAST_OPERATIONS(HChoice, Choice);
  CAST_OPERATIONS(HDependent, Dependent);

  void returnString(const std::string &dst, char **buf, _size_t *bufSize)
  {
    if (!buf || !bufSize)
      return;

    *buf = new char [dst.size() + 1];
    
    auto length = dst.copy(*buf, dst.size());
    (*buf)[length] = '\0';
    *bufSize = length;
  }

  void setString(std::string &str, char *buf, _size_t bufSize)
  {
    str.assign(buf, bufSize);
  }
} // namespace


extern "C"
{
  HChoiceManager* newChoiceManager()
  {
    return reinterpret_cast<HChoiceManager *>(new ChoiceManager);
  }

  _result_t freeChoiceManager(HChoiceManager *mgr)
  {
    delete cast(mgr);
    return SUCCESS;
  }

  _result_t writeChoices(HChoiceManager *mgr, const char *filePath, _size_t filePathSize)
  {
    return cast(mgr)->writeToFile(std::string(filePath, filePathSize));
  }

  HChoiceManager *readChoicesFromFile(const char *filePath, _size_t filePathSize)
  {
    return cast(ChoiceManager::readFromFile(std::string(filePath, filePathSize)));
  }

  HChoiceManager *readChoicesFromContents(const char *contents, _size_t contentsPathSize)
  {
    return cast(ChoiceManager::readContents(std::string(contents, contentsPathSize)));
  }

  HChoice* addNewChoice(HChoiceManager *mgr, const char *name, _size_t nameSize)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->addChoice(std::string(name, nameSize)));
  }

  bool addExistingChoice(HChoiceManager *mgr, HChoice *choice)
  {
    auto cppMgr = cast(mgr);
    return cppMgr->addChoice(cast(choice));
  }

  void removeChoice(HChoiceManager *mgr, const char *name, _size_t nameSize)
  {
    auto cppMgr = cast(mgr);
    cppMgr->removeChoice(std::string(name, nameSize));
  }

  _size_t numChoices(HChoiceManager *mgr)
  {
    auto cppMgr = cast(mgr);
    return cppMgr->numChoices();
  }

  HChoice *choiceFromName(HChoiceManager *mgr, const char *name, _size_t size)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->choice(std::string(name, size)));
  }

  HChoice *choiceFromIndex(HChoiceManager *mgr, _size_t index)
  {
    auto cppMgr = cast(mgr);
    return cast(cppMgr->choice(index));
  }

  void freeChoice(HChoice *choice)
  {
    delete cast(choice);
  }

  void choiceName(HChoice *choice, char **name, _size_t *size)
  {
    auto cppChoice = cast(choice);
    returnString(cppChoice->name, name, size);
  }
}