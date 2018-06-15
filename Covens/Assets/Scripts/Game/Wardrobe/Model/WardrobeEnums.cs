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
}

[Flags]
public enum EnumWardrobeCategory
{
    None = 0,
    Hat = 1,
    Hair = 2,
    Neck = 4,
    Hand = 8,
    Tattoo = 16,
    Dress = 32,
    Feet = 64,
    Pants = 128,
    Wrist = 256,
    CarryOn = 512,
    Torso = 1024,
    Necklace = 2048,
    Censor = 4096,
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
    None,
}
/*
public enum EquippableItems
{
    Hat_1,
    Dress_1,
    HighBoots_1,
    Boots_1,
    ShoulderTattoo_1,
    ChestTattoo_1,
    Choker_1,
    RightHandBracelet_1,
    LeftHandBracelet_1,
    Gloves_1,
    Censor_1,
    Hair_1
}*/
