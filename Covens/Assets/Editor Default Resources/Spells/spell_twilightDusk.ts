{
    _id: "spell_twilightDusk",
    name: "Twilight Dustk",
    cost: 30,
    school: -1,
    target: true,
    self: false,
    cooldown: 30,
    xp: 5,
    successChance: 65,
    level: 3,
    requiredLevel: 3,
    alignment: 5,
    canCrit: true,
    damage: {
        min: 80,
        max: 240
    },
    effect: {
        modifiers: {
            power: "#1*caster:level/3",
            resilience: "1*caster:level/3"
        },
        duration: "60*caster:level"
    },
    ingredients: [
        {
            collectible: "coll_silverComb",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_bloodAgate",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_lobelia",
            count: 1,
            type: "herb"
        }
    ]
}