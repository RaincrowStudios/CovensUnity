{
    _id: "spell_greaterDispel",
    name: "Greater Dispell",
    level: 1,
    requiredLevel: 3,
    xp: 30,
    cooldown: 60,
    cost: 30,
    school: 0,
    canCrit: false,
    successChance: 65,
    target: true,
    self: false,
    alignment: 5,
    effect: {
        buff: true,
        dispel: 0
    },
    ingredients: [
        {
            collectible: "coll_malachite",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_beeswax",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_galangal",
            count: 1,
            type: "herb"
        }
    ]
}