add_library(DialogueManager src/dialogue_manager.cpp dialogue_manager.hpp dialogue_manager_api.h)

include(GenerateExportHeader)
generate_export_header(DialogueManager)

target_include_directories(DialogueManager PUBLIC ../ PUBLIC ${CMAKE_CURRENT_BINARY_DIR})

INSTALL(TARGETS DialogueManager DESTINATION lib)
INSTALL(FILES dialogue_manager_api.h dialogue_manager.hpp DESTINATION dialogue_manager)
INSTALL(FILES ${CMAKE_CURRENT_BINARY_DIR}/dialoguemanager_export.h DESTINATION .)

if(BUILD_TESTS)
    add_executable(DialogueManager_tests src/dialogue_manager_tests.cpp)

    target_link_libraries(DialogueManager_tests CONAN_PKG::gtest DialogueManager)

    add_test(NAME DialogueManager_tests COMMAND DialogueManager_tests)
endif()