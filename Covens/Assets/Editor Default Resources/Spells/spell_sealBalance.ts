{
    _id: "spell_sealBalance",
    name: "Seal of Balance",
    school: 0,
    cost: 200,
    alignment: 5,
    xp: 20,
    cooldown: 60,
    target: true,
    self: true,
    successChance: 65,
    requiredLevel: 10,
    canCrit: false,
    restrictions: {
        targetSchool: 0
    },
    effect: {
        buff: true,
        modifiers: {
            power: "1*target:resilience",
            resilience: "1*target:power"
        },
        stackable: 1,
        duration: "60*caster:level/3"
    }
}