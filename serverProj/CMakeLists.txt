cmake_minimum_required(VERSION 2.8)

project(RunFaster CXX)

set(CXX_FLAGS
    -g
    -Wall
    -Werror
    )

set (EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin) 
set (LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/lib)

add_subdirectory(mainsvr)
add_subdirectory(comm)