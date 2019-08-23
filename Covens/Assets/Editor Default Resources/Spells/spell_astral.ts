{
    _id: "spell_astral",
    name: "Astral Mode",
    school: 0,
    cost: 0,
    level: 0,
    sucessChange: 100,
    self: true,
    canCrit: false,
    target: false,
    cooldown: 180,
    requiredLevel: 1,
    xp: 0,
    alignment: 0,
    restrictions: {
        placeOfPower: true
    },
    effect: {
        buff: true,
        modifiers: {
            status: ["silenced", "astral"]
        },
        duration: "20+caster:level",
        stackable: 1
    }
}