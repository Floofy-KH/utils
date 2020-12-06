#pragma once

namespace floofy
{
  struct ID
  {
    explicit ID(size_t id) : _id(id) {}

    ID operator+(size_t rhs) { return ID{ _id + rhs }; }
    ID operator-(size_t rhs) { return ID{ _id - rhs }; }
    ID operator++(int) { return ID{ _id++ }; }
    ID operator--(int) { return ID{ _id-- }; }
    ID &operator++()
    {
      ++_id;
      return *this;
    }
    ID &operator--()
    {
      --_id;
      return *this;
    }

    bool operator>=(const ID &rhs) const { return _id >= rhs._id; }
    bool operator<=(const ID &rhs) const { return _id <= rhs._id; }
    bool operator<(const ID &rhs) const { return _id < rhs._id; }
    bool operator>(const ID &rhs) const { return _id > rhs._id; }
    bool operator==(const ID &rhs) const { return _id == rhs._id; }

    size_t _id;
  };
}