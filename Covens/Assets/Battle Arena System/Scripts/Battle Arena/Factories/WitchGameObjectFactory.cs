using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        // serialized variables
        [SerializeField] private BattleWitchView _battleWitchViewPrefab;

        // private variables
        private AvatarSpriteUtil _avatarSpriteUtil;

        protected virtual void OnEnable()
        {
            if (_avatarSpriteUtil == null)
            {
                _avatarSpriteUtil = FindObjectOfType<AvatarSpriteUtil>();
            }
        }

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {
            IWitchModel witchModel = character as IWitchModel;

            // Create character
            AbstractCharacterView characterMarker = Instantiate(_battleWitchViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> tex = this.StartCoroutine<Texture>(GenerateAvatar(witchModel));            
            while (tex.ReturnValue == null)
            {
                yield return null;
            }

            characterMarker.ChangeCharacterTexture(tex.ReturnValue);

            yield return characterMarker;
        }

        private IEnumerator<Texture> GenerateAvatar(IWitchModel witchModel)
        {
            bool isMale = (witchModel.Info.Gender == CharacterGender.Male);

            // Convert inventory to equipped apparel
            List<EquippedApparel> equippedApparels = new List<EquippedApparel>();
            foreach (InventoryApparelModel apparel in witchModel.Inventory.Equipped)
            {
                EquippedApparel equippedApparel = new EquippedApparel()
                {
                    id = apparel.Id,
                    assets = new List<string>(apparel.Assets)
                };
                equippedApparels.Add(equippedApparel);
                yield return null;
            }

            Texture generatedAvatar = null;
            bool isGeneratingAvatar = true;
            _avatarSpriteUtil.GenerateAvatar(isMale, equippedApparels, (sprite) =>
            {
                generatedAvatar = sprite.texture;
                isGeneratingAvatar = false;
            });

            // wait for texture to be generated
            while (isGeneratingAvatar)
            {
                yield return null;
            }

            yield return generatedAvatar;
        }
    }
}