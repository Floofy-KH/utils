add_library(DialogueManager_CPP src/dialogue_manager.cpp dialogue_manager.hpp)
add_library(DialogueManager::CPP ALIAS DialogueManager_CPP)
add_library(DialogueManager_C src/dialogue_manager_api.cpp dialogue_manager_api.h)
add_library(DialogueManager::C ALIAS DialogueManager_C)

include(GenerateExportHeader)
generate_export_header(DialogueManager_CPP)

target_include_directories(DialogueManager_CPP PUBLIC ../ PUBLIC ${CMAKE_CURRENT_BINARY_DIR})
target_compile_definitions(DialogueManager_CPP PRIVATE DialogueManager_EXPORTS)

set_target_properties(DialogueManager_CPP PROPERTIES CXX_STANDARD 17)

target_link_libraries(DialogueManager_C PRIVATE DialogueManager::CPP)
target_include_directories(DialogueManager_C PUBLIC ../)

INSTALL(TARGETS DialogueManager_CPP DialogueManager_C DESTINATION lib)
INSTALL(FILES dialogue_manager_api.h dialogue_manager.hpp DESTINATION dialogue_manager)
INSTALL(FILES ${CMAKE_CURRENT_BINARY_DIR}/dialoguemanager_export.h DESTINATION .)

if(BUILD_TESTING)
    add_executable(DialogueManager_tests src/dialogue_manager_tests.cpp)

    set_target_properties(DialogueManager_tests PROPERTIES CXX_STANDARD 17)

    target_link_libraries(DialogueManager_tests CONAN_PKG::gtest DialogueManager::C)

    add_test(NAME DialogueManager_tests COMMAND DialogueManager_tests)
endif()