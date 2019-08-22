{
    _id: "spell_silence",
    name: "Silence",
    school: 1,
    cost: 0,
    level: 2,
    sucessChange: 65,
    self: false,
    canCrit: false,
    target: true,
    cooldown: 120,
    requiredLevel: 2,
    xp: 15,
    alignment: 1,
    restrictions: {
        state: "vulnerable"
    },
    effect: {
        buff: false,
        modifiers: {
            status: ["silenced"]
        },
        duration: "30-60",
        stackable: 1
    }
}