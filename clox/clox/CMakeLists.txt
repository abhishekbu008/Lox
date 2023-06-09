# CMakeList.txt : CMake project for clox, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (clox "clox.cpp" "clox.h" "headers/common.h" "headers/chunk.h" )

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET clox PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
