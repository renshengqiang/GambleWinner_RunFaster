include_directories(inc)

set(LIB_SRC 
	src/net_mgr.cpp)

set(LIBRARY_OUTPUT_PATH lib)
add_library(network STATIC ${LIB_SRC})