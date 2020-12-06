#pragma once

#define SUCCESS 0;
#define FAILURE 1;

#define CAST_OPERATIONS(PtrTC, PtrTCpp)          \
    inline PtrTC *cast(PtrTCpp *ptr)             \
    {                                            \
        return reinterpret_cast<PtrTC *>(ptr);   \
    }                                            \
    inline PtrTCpp *cast(PtrTC *ptr)             \
    {                                            \
        return reinterpret_cast<PtrTCpp *>(ptr); \
    }