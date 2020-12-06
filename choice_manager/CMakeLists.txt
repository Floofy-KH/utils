add_library(ChoiceManager src/choice_manager.cpp src/choice_manager.hpp src/choice_manager_api.cpp choice_manager_api.h)

target_include_directories(ChoiceManager PUBLIC ../ PUBLIC ${CMAKE_CURRENT_BINARY_DIR})
target_link_libraries(ChoiceManager PRIVATE Common)
target_link_libraries(ChoiceManager PRIVATE CONAN_PKG::jsonformoderncpp)

set_target_properties(ChoiceManager PROPERTIES CXX_STANDARD 17)

set(DialogueManagerCSBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueManager/bin/x64/$<CONFIG>/netstandard1.4")
set(DialogueManagerCSTestsBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueManagerTests/bin/x64/$<CONFIG>")
set(DialogueEditorBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueEditor/bin/x64/$<CONFIG>")

add_custom_command(
    TARGET ChoiceManager
    POST_BUILD
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:ChoiceManager>" "${DialogueManagerCSBuildPath}/$<TARGET_FILE_NAME:ChoiceManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:ChoiceManager>" "${DialogueManagerCSBuildPath}/$<TARGET_PDB_FILE_NAME:ChoiceManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:ChoiceManager>" "${DialogueManagerCSTestsBuildPath}/$<TARGET_FILE_NAME:ChoiceManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:ChoiceManager>" "${DialogueManagerCSTestsBuildPath}/$<TARGET_PDB_FILE_NAME:ChoiceManager>" 
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:ChoiceManager>" "${DialogueEditorBuildPath}/$<TARGET_FILE_NAME:ChoiceManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:ChoiceManager>" "${DialogueEditorBuildPath}/$<TARGET_PDB_FILE_NAME:ChoiceManager>" 
)

INSTALL(TARGETS ChoiceManager DESTINATION lib)
INSTALL(FILES dialogue_manager_api.h DESTINATION dialogue_manager)

if(BUILD_TESTING)
    add_executable(ChoiceManager_tests src/choice_manager_tests.cpp)

    set_target_properties(ChoiceManager_tests PROPERTIES CXX_STANDARD 17)

    target_link_libraries(ChoiceManager_tests CONAN_PKG::gtest ChoiceManager)

    add_test(NAME ChoiceManager_tests COMMAND ChoiceManager_tests)
endif()