{
    _id: "spell_grace",
    name: "Grace",
    school: 1,
    cost: 150,
    alignment: 10,
    xp: 15,
    cooldown: 120,
    level: 5,
    target: true,
    canCrit: true,
    successChance: 65,
    self: false,
    requiredLevel: 3,
    restrictions: {
        state: "dead"
    },
    damage: {
        min: "#50*caster:level/1",
        max: "#100*caster:level/1"
    }
}