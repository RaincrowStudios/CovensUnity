{
    _id: "spell_seal",
    name: "Seal",
    school: 0,
    cost: 10,
    canCrit: false,
    successChance: 65,
    cooldown: 20,
    self: false,
    target: true,
    level: 1,
    requiredLevel: 1,
    alignment: 3,
    xp: 2,
    effect: {
        buff: false,
        modifiers: {
            resilience: "1*caster:level/1",
            power: "1*caster:level/1"
        },
        stackable: 3,
        duration: 180
    }
}