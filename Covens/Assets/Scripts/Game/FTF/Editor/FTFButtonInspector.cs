using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Raincrow.FTF
{
    [CustomEditor(typeof(FTFButtonArea))]
    public class FTFButtonInspector : FTFRectBaseInspector
    {
        public override string FieldName => "button";
    }
}