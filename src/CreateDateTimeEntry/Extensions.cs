using System;
using System.Linq.Expressions;
using KeePassLib;

namespace CreateDateTimeEntry
{
  public static class Extensions
  {
    public static PwGroup SetCustomIcon(this PwGroup group, PwDatabase database, int index)
    {
      group.CustomIconUuid = database.CustomIcons[index].Uuid;
      return group;
    }
  }
}
