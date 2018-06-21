using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumGender
{
    Undefined,
    Female,
    Male,
}
public enum EnumAlignment
{
    White,
    Gray,
    Shadow,

    All,
    None,
}

[Flags]
public enum EnumWardrobeCategory
{
    None = 0,
    Hat = 1 << 0,
    Hair = 1 << 1,
    Neck = 1 << 2,
    Hand = 1 << 3,
    Tattoo = 1 << 4,
    Dress = 1 << 5,
    Feet = 1 << 6,
    Pants = 1 << 7,
    Wrist = 1 << 8,
    CarryOn = 1 << 9,
    Torso = 1 << 10,
    Necklace = 1 << 11,
    Censor = 1 << 12,
    Hood = 1 << 13,
    Choker = 1 << 14,
    Necklase = 1 << 15,
    Robe = 1 << 16,
    Shirt = 1 << 17,
    Bracelet = 1 << 18,
    Wraps = 1 << 19,
    Gloves = 1 << 20,
    Ring = 1 << 21,
    Boots = 1 << 22,
    Shoes = 1 << 23,
    Body = 1 << 24,
    Censer = 1 << 25,
    Skirt = 1 << 26,
}


public enum EnumEquipmentSlot
{
    Base,
    Head,
    Hair,
    Neck,
    Chest,
    Wrist,
    Hands,
    Finger,
    Waist,
    Legs,
    Feet,
    CarryOn,
    SkinFace,
    SkinShoulder,
    SkinChest,
    SkinArm,
    SpecialSlot,
    BaseHand,
    BaseBody,


    None,
}

public enum HandMode
{
    Relaxed,
    Censer,
}

