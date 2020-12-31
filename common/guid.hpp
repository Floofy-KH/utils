#pragma once

#include <array>
#include <chrono>
#include <string>
#include <random>
#include <limits>
#include <sstream>

namespace floofy
{
  class Guid
  {
  public:

    using GuidT = std::array<uint8_t, 16>;

    Guid()
    {
      unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
      std::default_random_engine generator(seed);
      std::uniform_int_distribution<uint32_t> dist(std::numeric_limits<uint32_t>::min(), std::numeric_limits<uint32_t>::max());
      for(int i=0; i<16; i+=4) // Doing four entries each iteration due to generating 32bit values;
      {
        auto randVal = dist(generator);
        m_value[i] = static_cast<uint8_t>(randVal);
        m_value[i+1] = static_cast<uint8_t>(randVal >> 8);
        m_value[i+2] = static_cast<uint8_t>(randVal >> 16);
        m_value[i+3] = static_cast<uint8_t>(randVal >> 24);
      }
    }

    Guid(GuidT val) : m_value(val)
    {

    }

    Guid(const Guid& guid) : m_value(guid.m_value)
    {

    }

    ~Guid() = default;

    GuidT value() const
    {
      return m_value;
    }

    std::string toString() const
    {
      static std::string templateVal{"%X%X%X%X-%X%X-%X%X-%X%X-%X%X%X%X%X%X"};
      std::string val; 
      val.resize(8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 12);
      sprintf(&val[0], templateVal.c_str(), 
              m_value[0], m_value[1], m_value[2], m_value[3],
              m_value[4], m_value[5], m_value[6], m_value[7],
              m_value[8], m_value[9], m_value[10], m_value[11],
              m_value[12], m_value[13], m_value[14], m_value[15]);
      return val;
    }

  bool operator==(const floofy::Guid& rhs) const
  {
    return m_value == rhs.m_value;
  }

  bool operator!=(const floofy::Guid& rhs) const
  {
    return !(*this == rhs);
  }

  private:
    GuidT m_value;
  };
}