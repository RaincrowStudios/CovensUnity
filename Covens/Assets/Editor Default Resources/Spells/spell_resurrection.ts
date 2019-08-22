{
    _id: "spell_resurrection",
    name: "Resurrection",
    alignment: 10,
    school: -1,
    cost: 100,
    cooldown: 120,
    canCrit: true,
    target: true,
    requiredLevel: 5,
    self: false,
    level: 5,
    successChance: 65,
    xp: 15,
    restrictions: {
        state: "dead"
    },
    damage: {
        min: "#20*caster:level/1",
        max: "#60*caster:level/1"
    },
    ingredients: [
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        }
    ]
}