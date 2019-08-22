{
    _id: "spell_greaterBless",
    name: "Greater Bless",
    level: 1,
    requiredLevel: 3,
    xp: 30,
    cooldown: 60,
    cost: 30,
    school: 1,
    target: true,
    self: false,
    canCrit: true,
    successChance: 65,
    alignment: 5,
    effect: {
        buff: true,
        baseSpell: {
            multiplier: 3,
            _id: "spell_bless"
        }
    },
    ingredients: [
        {
            collectible: "coll_onyxCirclet",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_betony",
            count: 1,
            type: "herb"
        }
    ]
}