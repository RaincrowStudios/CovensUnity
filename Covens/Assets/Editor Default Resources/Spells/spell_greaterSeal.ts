{
    _id: "spell_greaterSeal",
    name: "Greater Seal",
    school: 0,
    target: true,
    self: false,
    cost: 30,
    canCrit: false,
    cooldown: 60,
    alignment: 5,
    xp: 30,
    level: 3,
    successChance: 65,
    requiredLevel: 1,
    effect: {
        buff: true,
        baseSpell: {
            multiplier: 3,
            _id: "spell_seal"
        },
        duration: 180
    },
    ingredients: [
        {
            collectible: "coll_votiveCandle",
            count: 1,
            type: "tool"
        },
        {
            collectibe: "coll_bindweed",
            count: 1,
            type: "herb"
        },
        {
            collectible: "coll_bloodAgate",
            count: 1,
            type: "gem"
        }
    ]
}