using Godot;
using System;

public static class CursorItem
{
    public static ItemStack HeldItem = null;

    public static bool HasItem() => HeldItem != null && HeldItem.Count > 0;

    public static void Clear() => HeldItem = null;
}
