{
    _id: "spell_shadowMark",
    name: "Shadow Mark",
    cost: 0,
    school: -1,
    target: true,
    self: false,
    canCrit: false,
    level: 10,
    requiredLevel: 10,
    successChance: 65,
    xp: 15,
    cooldown: 300,
    alignment: 2,
    restrictions: {
        selfSchool: -1
    },
    effect: {
        buff: false,
        modifiers: {
            status: "mariaokiss",
            onDispel: {
                damage: {
                    min: 400,
                    max: 600
                },
                target: true,
                self: true
            }
        },
        duration: "7200*caster:degree"
    }
}