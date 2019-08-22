{
    _id: "spell_sealLight",
    name: "Seal of Light",
    school: 1,
    cost: 200,
    alignment: 5,
    xp: 20,
    level: 1,
    requiredLevel: 10,
    self: true,
    target: true,
    successChance: 65,
    cooldown: 60,
    canCrit: false,
    restrictions: {
        targetSchool: 1
    },
    effect: {
        buff: true,
        modifiers: {
            power: "1*target:degree"
        },
        stackable: 1,
        duration: "60*caster:level/3"
    }
}