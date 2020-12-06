add_library(DialogueManager src/dialogue_manager.cpp src/dialogue_manager.hpp src/dialogue_manager_api.cpp dialogue_manager_api.h)

target_include_directories(DialogueManager PUBLIC ../ PUBLIC ${CMAKE_CURRENT_BINARY_DIR})
target_link_libraries(DialogueManager PRIVATE Common)
target_link_libraries(DialogueManager PRIVATE CONAN_PKG::jsonformoderncpp)

set_target_properties(DialogueManager PROPERTIES CXX_STANDARD 17)

set(DialogueManagerCSBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueManager/bin/x64/$<CONFIG>/netstandard1.4")
set(DialogueManagerCSTestsBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueManagerTests/bin/x64/$<CONFIG>")
set(DialogueEditorBuildPath  "${CMAKE_SOURCE_DIR}/dialogue_editor/DialogueEditor/bin/x64/$<CONFIG>")

add_custom_command(
    TARGET DialogueManager
    POST_BUILD
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:DialogueManager>" "${DialogueManagerCSBuildPath}/$<TARGET_FILE_NAME:DialogueManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:DialogueManager>" "${DialogueManagerCSBuildPath}/$<TARGET_PDB_FILE_NAME:DialogueManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:DialogueManager>" "${DialogueManagerCSTestsBuildPath}/$<TARGET_FILE_NAME:DialogueManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:DialogueManager>" "${DialogueManagerCSTestsBuildPath}/$<TARGET_PDB_FILE_NAME:DialogueManager>" 
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_FILE:DialogueManager>" "${DialogueEditorBuildPath}/$<TARGET_FILE_NAME:DialogueManager>"
    COMMAND "cmake" "-E" "copy_if_different" "$<TARGET_PDB_FILE:DialogueManager>" "${DialogueEditorBuildPath}/$<TARGET_PDB_FILE_NAME:DialogueManager>" 
)

INSTALL(TARGETS DialogueManager DESTINATION lib)
INSTALL(FILES dialogue_manager_api.h DESTINATION dialogue_manager)

if(BUILD_TESTING)
    add_executable(DialogueManager_tests src/dialogue_manager_tests.cpp)

    set_target_properties(DialogueManager_tests PROPERTIES CXX_STANDARD 17)

    target_link_libraries(DialogueManager_tests CONAN_PKG::gtest DialogueManager)

    add_test(NAME DialogueManager_tests COMMAND DialogueManager_tests)
endif()