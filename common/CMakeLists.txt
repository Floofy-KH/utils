add_library(Common INTERFACE)

target_include_directories(Common INTERFACE ../)

if(BUILD_TESTING)
    add_executable(Utils_tests src/utils_tests.cpp)

    set_target_properties(Utils_tests PROPERTIES CXX_STANDARD 17)

    target_link_libraries(Utils_tests CONAN_PKG::gtest Common)

    add_test(NAME Utils_tests COMMAND Utils_tests)
endif()