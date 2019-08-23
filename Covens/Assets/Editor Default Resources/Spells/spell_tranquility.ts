{
    _id: "spell_tranquility",
    name: "Tranquility",
    cost: 50,
    school: 0,
    target: true,
    self: false,
    canCrit: false,
    level: 5,
    requiredLevel: 5,
    xp: 0,
    cooldown: 60,
    alignment: 0,
    successChance: 65,
    effect: {
        buff: true,
        modifiers: {
            interruptChance: "#2*caster:level"
        },
        stackable: 1,
        duration: 10
    }
}