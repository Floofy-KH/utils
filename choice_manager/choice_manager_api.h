#pragma once

#include "common/defines_api.h"

struct HChoiceManager;
struct HChoice;
struct HDependent;

#if __cplusplus
extern "C"
{
#endif

EXPORT HChoiceManager* newChoiceManager();
EXPORT _result_t freeChoiceManager(HChoiceManager *mgr);
EXPORT _result_t writeChoices(HChoiceManager *mgr, const char *filePath, _size_t filePathSize);
EXPORT HChoiceManager *readChoicesFromFile(const char *filePath, _size_t filePathSize);
EXPORT HChoiceManager *readChoicesFromContents(const char *contents, _size_t contentsPathSize);

EXPORT HChoice* addNewChoice(HChoiceManager *mgr, const char *name, _size_t nameSize);
EXPORT bool addExistingChoice(HChoiceManager *mgr, HChoice *choice);
EXPORT void removeChoice(HChoiceManager *mgr, const char *name, _size_t nameSize);
EXPORT _size_t numChoices(HChoiceManager *mgr);
EXPORT HChoice *choiceFromName(HChoiceManager *mgr, const char *name, _size_t size);
EXPORT HChoice *choiceFromIndex(HChoiceManager *mgr, _size_t index);
EXPORT void freeChoice(HChoice *choice);

EXPORT void choiceName(HChoice *choice, char **name, _size_t *size);

#if __cplusplus
}
#endif