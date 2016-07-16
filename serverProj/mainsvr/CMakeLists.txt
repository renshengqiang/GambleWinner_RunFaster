include_directories(inc)
include_directories(${PROJECT_SOURCE_DIR}/comm/network/inc)

set(APP_SRC
	src/server_main.cpp)
 
add_executable(mainsvr ${APP_SRC})
target_link_libraries(mainsvr network)