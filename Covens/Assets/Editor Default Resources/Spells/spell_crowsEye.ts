{
    _id: "spell_crowsEye",
    name: "Crows Eye",
    level: 7,
    requiredLevel: 7,
    xp: 20,
    cost: 300,
    school: 0,
    canCrit: false,
    target: false,
    self: true,
    cooldown: 120,
    successChange: 100,
    alignment: 10,
    restrictions: {
        targetSchool: 0
    },
    effect: {
        buff: true,
        modifiers: {
            status: ["trueSight"],
            trueSightRange: {
                limit: 0, //km
                latitude: 0,
                longitude: 0
            }
        },
        duration: 10800,
        stackable: 1
    }

}