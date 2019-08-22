{
    _id: "spell_sealShadow",
    name: "Seal Shadow",
    school: -1,
    restrictions: {
        targetSchool: -1
    },
    successChance: 65,
    canCrit: false,
    self: true,
    target: true,
    cooldown: 60,
    level: 3,
    requiredLevel: 10,
    cost: 200,
    xp: 20,
    alignment: 5,
    effect: {
        buff: true,
        modifiers: {
            resilience: "#2*caster:degree"
        },
        stackable: 1,
        duration: 60
    }
}