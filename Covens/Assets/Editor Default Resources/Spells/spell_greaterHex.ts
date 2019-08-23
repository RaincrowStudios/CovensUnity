{
    _id: "spell_greaterHex",
    name: "Greater Hex",
    school: -1,
    cost: 30,
    alignment: 5,
    xp: 30,
    level: 1,
    requiredLevel: 3,
    successChance: 65,
    cooldown: 60,
    self: false,
    target: true,
    canCrit: true,
    effect: {
        buff: false,
        baseSpell: {
            multiplier: 3,
            _id: "spell_hex"
        }
    },
    ingredients: [
        {
            collectible: "coll_onyxRing",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_bloodAgate",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_borage",
            count: 1,
            type: "herb"
        }
    ]
}