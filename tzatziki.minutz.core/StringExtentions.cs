﻿using System;

namespace tzatziki.minutz.core
{
  public static class StringExtentions
  {
    public static string ToSchemaString(this Guid instanceId)
    {
      return $"account_{instanceId.ToString().Replace("-", string.Empty)}";
    }

    public static string ToInstanceString(this Guid instanceId)
    {
      return $"{instanceId.ToString().Replace("-", string.Empty)}";
    }
  }
}