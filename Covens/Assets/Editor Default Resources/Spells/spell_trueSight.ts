{
    _id: "spell_trueSight",
    name: "True Sight",
    alignment: 5,
    school: 0,
    xp: 10,
    level: 7,
    requiredLevel: 1,
    self: true,
    target: false,
    cooldown: 120,
    successChance: 80,
    canCrit: false,
    cost: 100,
    restrictions: {
        targetSchool: 0
    },
    effect: {
        buff: true,
        modifiers:{
            status: ["trueSight"],
            trueSightRange: {
                limit: 3000, //meters
                latitude: 0,
                longitude: 0
            }
        },
        duration: 10800,
        stackable: 1
    }
}