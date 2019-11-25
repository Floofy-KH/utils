add_library(DialogueManager src/dialogue_manager.cpp src/dialogue_manager.hpp src/dialogue_manager_api.cpp dialogue_manager_api.h)

target_include_directories(DialogueManager PUBLIC ../ PUBLIC ${CMAKE_CURRENT_BINARY_DIR})

set_target_properties(DialogueManager PROPERTIES CXX_STANDARD 17)

INSTALL(TARGETS DialogueManager DESTINATION lib)
INSTALL(FILES dialogue_manager_api.h DESTINATION dialogue_manager)

if(BUILD_TESTING)
    add_executable(DialogueManager_tests src/dialogue_manager_tests.cpp)

    set_target_properties(DialogueManager_tests PROPERTIES CXX_STANDARD 17)

    target_link_libraries(DialogueManager_tests CONAN_PKG::gtest DialogueManager)

    add_test(NAME DialogueManager_tests COMMAND DialogueManager_tests)
endif()