{
    _id: "spell_twilightDawn",
    name: "Twilight Dawn",
    cost: 30,
    school: 1,
    target: true,
    self: false,
    canCrit: true,
    xp: 5,
    cooldown: 30,
    level: 3,
    requiredLevel: 3,
    successChance: 65,
    alignment: 5,
    damage: {
        min: 80,
        max: 240
    },
    effect: {
        buff: false,
        modifiers: {
            resilience: "#1*caster:level/3",
            power: "1*caster:level/3"
        },
        duration: "60*caster:level"
    },
    ingredients: [
        {
            collectible: "coll_friedTofu",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_pennyRoyal",
            count: 1,
            type: "herb"
        }
    ]
}