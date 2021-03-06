cmake_minimum_required(VERSION 3.16.0) #Don't actually know

project(Utils)

option(Build_DialogueManager "Build DialogueManager project" ON)

set(CONAN_REQUIRES ${CONAN_REQUIRES} jsonformoderncpp/3.7.2@vthiery/stable)
if(BUILD_TESTING)
    set(CONAN_REQUIRES ${CONAN_REQUIRES} gtest/1.8.1@bincrafters/stable)
    enable_testing()
endif()

set(CMAKE_CXX_VISIBILITY_PRESET hidden)
set(CMAKE_VISIBILITY_INLINES_HIDDEN 1)

if(NOT EXISTS "${CMAKE_BINARY_DIR}/conan.cmake")
    message(STATUS "Downloading conan.cmake from https://github.com/conan-io/cmake-conan")
    file(DOWNLOAD "https://github.com/conan-io/cmake-conan/raw/v0.14/conan.cmake"
              "${CMAKE_BINARY_DIR}/conan.cmake")
endif()

include(${CMAKE_BINARY_DIR}/conan.cmake)

conan_cmake_run(REQUIRES ${CONAN_REQUIRES}
             BASIC_SETUP CMAKE_TARGETS
             BUILD missing)

#Shamelessly stolen from conan script above.
#Get the target architecture, currently only MSVC.            
if (MSVC_CXX_ARCHITECTURE_ID MATCHES "64")
    set(TARGET_ARCH x64)
elseif (MSVC_CXX_ARCHITECTURE_ID MATCHES "86")
    set(TARGET_ARCH x86)
else ()
    message(FATAL_ERROR "Conan: Unhandled MSVC architecture [${MSVC_CXX_ARCHITECTURE_ID}]")
endif()

add_subdirectory(common)
if(Build_DialogueManager)
    add_subdirectory(dialogue_manager)
endif()